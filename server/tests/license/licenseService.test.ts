import { describe, it, expect, beforeEach, vi } from 'vitest'
import {
  generateLicenseKey,
  createLicense,
  activateLicense,
  verifyLicense,
  checkUserModuleLicense
} from '~/server/services/license'

// Mock dependencies
vi.mock('~/server/services/database', () => ({
  useNitroDatabase: () => ({
    db: vi.fn()
  })
}))

describe('License Service', () => {
  describe('generateLicenseKey', () => {
    it('should generate valid license keys', () => {
      const key1 = generateLicenseKey()
      const key2 = generateLicenseKey()

      // Should have different checksums
      expect(key1).not.toBe(key2)
      expect(key1).toMatch(/^MOD-[a-z0-9]+-[a-z0-9]{4,8}-[a-z0-9]{4}$/)
      expect(key2).toMatch(/^MOD-[a-z0-9]+-[a-z0-9]{4,8}-[a-z0-9]{4}$/)
    })

    it('should generate unique keys', () => {
      const keys = new Set()
      for (let i = 0; i < 100; i++) {
        keys.add(generateLicenseKey())
      }
      expect(keys.size).toBe(100) // All keys should be unique
    })
  })

  describe('createLicense', () => {
    it('should create a permanent license', async () => {
      const licenseData = {
        orderId: 1,
        moduleKey: 'test-module',
        userId: 1,
        type: 'permanent' as const
      }

      const license = await createLicense(licenseData)

      expect(license.moduleKey).toBe('test-module')
      expect(license.userId).toBe(1)
      expect(license.type).toBe('permanent')
      expect(license.status).toBe('active')
      expect(license.validUntil).toBeUndefined()
      expect(license.licenseKey).toMatch(/^MOD-[a-z0-9]+-[a-z0-9]{4,8}-[a-z0-9]{4}$/)
    })

    it('should create a subscription license with 30-day validity', async () => {
      const licenseData = {
        orderId: 2,
        moduleKey: 'test-module',
        userId: 2,
        type: 'subscription' as const
      }

      const beforeCreation = new Date()
      const license = await createLicense(licenseData)
      const afterCreation = new Date()

      expect(license.type).toBe('subscription')
      expect(license.validUntil).toBeDefined()

      // Check that validUntil is about 30 days from now
      const validUntil = new Date(license.validUntil!)
      const expectedMin = new Date(beforeCreation.getTime() + 29 * 24 * 60 * 60 * 1000)
      const expectedMax = new Date(afterCreation.getTime() + 31 * 24 * 60 * 60 * 1000)

      expect(validUntil.getTime()).toBeGreaterThan(expectedMin.getTime())
      expect(validUntil.getTime()).toBeLessThan(expectedMax.getTime())
    })

    it('should create a trial license with 7-day validity', async () => {
      const licenseData = {
        orderId: 3,
        moduleKey: 'test-module',
        userId: 3,
        type: 'trial' as const
      }

      const beforeCreation = new Date()
      const license = await createLicense(licenseData)
      const afterCreation = new Date()

      expect(license.type).toBe('trial')
      expect(license.validUntil).toBeDefined()

      // Check that validUntil is about 7 days from now
      const validUntil = new Date(license.validUntil!)
      const expectedMin = new Date(beforeCreation.getTime() + 6 * 24 * 60 * 60 * 1000)
      const expectedMax = new Date(afterCreation.getTime() + 8 * 24 * 60 * 60 * 1000)

      expect(validUntil.getTime()).toBeGreaterThan(expectedMin.getTime())
      expect(validUntil.getTime()).toBeLessThan(expectedMax.getTime())
    })
  })

  describe('verifyLicense', () => {
    it('should verify an active license', async () => {
      // Mock database response
      const mockLicense = {
        id: 1,
        licenseKey: 'TEST-KEY-1234',
        moduleKey: 'test-module',
        userId: 1,
        type: 'permanent' as const,
        status: 'active',
        validUntil: null,
        activationsUsed: 0,
        maxActivations: 1
      }

      vi.mocked(useNitroDatabase).mockReturnValue({
        db: vi.fn().mockReturnValue({
          where: vi.fn().mockReturnValue({
            where: vi.fn().mockReturnValue({
              first: vi.fn().mockResolvedValue(mockLicense)
            })
          })
        })
      } as any)

      const result = await verifyLicense({ licenseKey: 'TEST-KEY-1234' })

      expect(result.isValid).toBe(true)
      expect(result.license).toEqual(mockLicense)
    })

    it('should return false for expired license', async () => {
      const expiredLicense = {
        id: 1,
        licenseKey: 'EXPIRED-KEY-1234',
        moduleKey: 'test-module',
        userId: 1,
        type: 'trial' as const,
        status: 'active',
        validUntil: '2020-01-01T00:00:00.000Z', // Past date
        activationsUsed: 0,
        maxActivations: 1
      }

      vi.mocked(useNitroDatabase).mockReturnValue({
        db: vi.fn().mockReturnValue({
          where: vi.fn().mockReturnValue({
            where: vi.fn().mockReturnValue({
              first: vi.fn().mockResolvedValue(expiredLicense)
            })
          })
        })
      } as any)

      const result = await verifyLicense({ licenseKey: 'EXPIRED-KEY-1234' })

      expect(result.isValid).toBe(false)
      expect(result.error).toBe('License expired')
    })

    it('should return false for invalid license key', async () => {
      vi.mocked(useNitroDatabase).mockReturnValue({
        db: vi.fn().mockReturnValue({
          where: vi.fn().mockReturnValue({
            where: vi.fn().mockReturnValue({
              first: vi.fn().mockResolvedValue(null)
            })
          })
        })
      } as any)

      const result = await verifyLicense({ licenseKey: 'INVALID-KEY' })

      expect(result.isValid).toBe(false)
      expect(result.error).toBe('License not found or inactive')
    })
  })

  describe('activateLicense', () => {
    it('should activate license on new device', async () => {
      const mockLicense = {
        id: 1,
        licenseKey: 'ACTIVATE-KEY-1234',
        moduleKey: 'test-module',
        userId: 1,
        type: 'permanent' as const,
        status: 'active',
        validUntil: null,
        activationsUsed: 0,
        maxActivations: 3
      }

      vi.mocked(useNitroDatabase).mockReturnValue({
        db: vi.fn()
          .mockReturnValueOnce({
            where: vi.fn().mockReturnValue({
              first: vi.fn().mockResolvedValue(mockLicense)
            })
          })
          .mockReturnValueOnce({
            where: vi.fn().mockReturnValue({
              first: vi.fn().mockResolvedValue(null) // No existing activation
            })
          })
          .mockReturnValueOnce({
            insert: vi.fn().mockResolvedValue({})
          })
          .mockReturnValueOnce({
            where: vi.fn().mockReturnValue({
              update: vi.fn().mockResolvedValue({})
            })
          })
          .mockReturnValueOnce({
            where: vi.fn().mockReturnValue({
              first: vi.fn().mockResolvedValue(mockLicense)
            })
          })
      } as any)

      const activationRequest = {
        licenseKey: 'ACTIVATE-KEY-1234',
        deviceId: 'device-123',
        deviceName: 'My Laptop'
      }

      const result = await activateLicense(activationRequest)

      expect(result.activationsUsed).toBe(1)
      expect(result.lastActivatedAt).toBeDefined()
    })

    it('should fail to activate if max activations exceeded', async () => {
      const mockLicense = {
        id: 1,
        licenseKey: 'MAX-ACTIVATIONS-KEY',
        moduleKey: 'test-module',
        userId: 1,
        type: 'permanent' as const,
        status: 'active',
        validUntil: null,
        activationsUsed: 3,
        maxActivations: 3
      }

      vi.mocked(useNitroDatabase).mockReturnValue({
        db: vi.fn().mockReturnValue({
          where: vi.fn().mockReturnValue({
            first: vi.fn().mockResolvedValue(mockLicense)
          })
        })
      } as any)

      const activationRequest = {
        licenseKey: 'MAX-ACTIVATIONS-KEY',
        deviceId: 'device-456'
      }

      await expect(activateLicense(activationRequest)).rejects.toThrow('Maximum activations exceeded')
    })
  })

  describe('checkUserModuleLicense', () => {
    it('should return true if user has active license', async () => {
      vi.mocked(useNitroDatabase).mockReturnValue({
        db: vi.fn().mockReturnValue({
          where: vi.fn().mockReturnValue({
            where: vi.fn().mockReturnValue({
              where: vi.fn().mockReturnValue({
                first: vi.fn().mockResolvedValue({
                  id: 1,
                  status: 'active'
                })
              })
            })
          })
        })
      } as any)

      const hasLicense = await checkUserModuleLicense(1, 'test-module')
      expect(hasLicense).toBe(true)
    })

    it('should return false if user has no license', async () => {
      vi.mocked(useNitroDatabase).mockReturnValue({
        db: vi.fn().mockReturnValue({
          where: vi.fn().mockReturnValue({
            where: vi.fn().mockReturnValue({
              where: vi.fn().mockReturnValue({
                first: vi.fn().mockResolvedValue(null)
              })
            })
          })
        })
      } as any)

      const hasLicense = await checkUserModuleLicense(2, 'test-module')
      expect(hasLicense).toBe(false)
    })
  })
})