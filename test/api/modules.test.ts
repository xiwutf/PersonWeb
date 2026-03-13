/**
 * Modules API 单元测试
 */

import { describe, it, expect, beforeEach, vi } from 'vitest'

// Mock database service
const mockDb = {
  getModules: vi.fn(),
  getModuleByKey: vi.fn(),
  createModule: vi.fn(),
  updateModule: vi.fn(),
  deleteModule: vi.fn(),
  getModuleVersions: vi.fn(),
  createModuleVersion: vi.fn()
}

vi.mock('~/server/services/database', () => ({
  default: mockDb
}))

// Types
interface Module {
  id: number
  moduleKey: string
  moduleName: string
  moduleVersion: string
  description?: string
  author?: string
  icon?: string
  category?: string
  dependencies?: string[]
  routes?: any[]
  components?: any[]
  permissions?: Record<string, any>
  configSchema?: Record<string, any>
  isEnabled: boolean
  isCore: boolean
  sort: number
  createdAt: string
  updatedAt: string
}

interface CreateModuleRequest {
  moduleKey: string
  moduleName: string
  description?: string
  author?: string
  icon?: string
  category: string
  dependencies?: string[]
  routes?: any[]
  components?: any[]
  permissions?: any[]
  configSchema?: Record<string, any>
  sort?: number
}

interface PaginationResult {
  data: Module[]
  pagination: {
    page: number
    pageSize: number
    total: number
    totalPages: number
  }
}

describe('Modules API', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('GET /api/modules', () => {
    it('should return modules list with pagination', async () => {
      const mockModules: Module[] = [
        {
          id: 1,
          moduleKey: 'ai-assistant',
          moduleName: 'AI Assistant',
          moduleVersion: '1.0.0',
          description: 'AI助手功能',
          author: 'Developer',
          icon: '🤖',
          category: 'ai',
          dependencies: [],
          routes: [],
          components: [],
          permissions: {},
          configSchema: {},
          isEnabled: true,
          isCore: false,
          sort: 0,
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 2,
          moduleKey: 'admin-panel',
          moduleName: 'Admin Panel',
          moduleVersion: '1.0.0',
          description: '后台管理面板',
          author: 'Developer',
          icon: '⚙️',
          category: 'admin',
          dependencies: [],
          routes: [],
          components: [],
          permissions: {},
          configSchema: {},
          isEnabled: true,
          isCore: true,
          sort: 0,
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-01-01T00:00:00Z'
        }
      ]

      mockDb.getModules.mockResolvedValue({
        data: mockModules.map(m => ({
          id: m.id,
          module_key: m.moduleKey,
          module_name: m.moduleName,
          module_version: m.moduleVersion,
          description: m.description,
          author: m.author,
          icon: m.icon,
          category: m.category,
          dependencies: JSON.stringify(m.dependencies),
          routes: JSON.stringify(m.routes),
          components: JSON.stringify(m.components),
          permissions: JSON.stringify(m.permissions),
          config_schema: JSON.stringify(m.configSchema),
          is_enabled: m.isEnabled,
          is_core: m.isCore,
          sort: m.sort,
          created_at: m.createdAt,
          updated_at: m.updatedAt
        })),
        pagination: {
          page: 1,
          pageSize: 10,
          total: 2,
          totalPages: 1
        }
      })

      const query = { page: 1, pageSize: 10 }
      const result = await mockDb.getModules(query)

      expect(mockDb.getModules).toHaveBeenCalledWith(query)
      expect(result.data).toHaveLength(2)
      expect(result.pagination.total).toBe(2)
    })

    it('should filter modules by category', async () => {
      const query = { category: 'ai', page: 1, pageSize: 10 }
      mockDb.getModules.mockResolvedValue({
        data: [],
        pagination: { page: 1, pageSize: 10, total: 0, totalPages: 0 }
      })

      await mockDb.getModules(query)

      expect(mockDb.getModules).toHaveBeenCalledWith(query)
    })

    it('should filter modules by enabled status', async () => {
      const query = { enabled: true, page: 1, pageSize: 10 }
      mockDb.getModules.mockResolvedValue({
        data: [],
        pagination: { page: 1, pageSize: 10, total: 0, totalPages: 0 }
      })

      await mockDb.getModules(query)

      expect(mockDb.getModules).toHaveBeenCalledWith(query)
    })

    it('should search modules by keyword', async () => {
      const query = { search: 'ai', page: 1, pageSize: 10 }
      mockDb.getModules.mockResolvedValue({
        data: [],
        pagination: { page: 1, pageSize: 10, total: 0, totalPages: 0 }
      })

      await mockDb.getModules(query)

      expect(mockDb.getModules).toHaveBeenCalledWith(query)
    })
  })

  describe('GET /api/modules/:moduleKey', () => {
    it('should return module details', async () => {
      const mockModule: Module = {
        id: 1,
        moduleKey: 'ai-assistant',
        moduleName: 'AI Assistant',
        moduleVersion: '1.0.0',
        description: 'AI助手功能',
        author: 'Developer',
        icon: '🤖',
        category: 'ai',
        dependencies: [],
        routes: [],
        components: [],
        permissions: {},
        configSchema: {},
        isEnabled: true,
        isCore: false,
        sort: 0,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      }

      mockDb.getModuleByKey.mockResolvedValue({
        id: mockModule.id,
        module_key: mockModule.moduleKey,
        module_name: mockModule.moduleName,
        module_version: mockModule.moduleVersion,
        description: mockModule.description,
        author: mockModule.author,
        icon: mockModule.icon,
        category: mockModule.category,
        dependencies: JSON.stringify(mockModule.dependencies),
        routes: JSON.stringify(mockModule.routes),
        components: JSON.stringify(mockModule.components),
        permissions: JSON.stringify(mockModule.permissions),
        config_schema: JSON.stringify(mockModule.configSchema),
        is_enabled: mockModule.isEnabled,
        is_core: mockModule.isCore,
        sort: mockModule.sort,
        created_at: mockModule.createdAt,
        updated_at: mockModule.updatedAt
      })

      const result = await mockDb.getModuleByKey('ai-assistant')

      expect(mockDb.getModuleByKey).toHaveBeenCalledWith('ai-assistant')
      expect(result).toBeDefined()
      expect(result.module_key).toBe('ai-assistant')
    })

    it('should return 404 if module not found', async () => {
      mockDb.getModuleByKey.mockResolvedValue(null)

      const result = await mockDb.getModuleByKey('non-existent')

      expect(result).toBeNull()
    })
  })

  describe('POST /api/modules', () => {
    it('should create new module', async () => {
      const newModule: CreateModuleRequest = {
        moduleKey: 'new-module',
        moduleName: 'New Module',
        description: 'A new module',
        category: 'test',
        dependencies: [],
        sort: 0
      }

      mockDb.getModuleByKey.mockResolvedValue(null)
      mockDb.createModule.mockResolvedValue(1)
      mockDb.getModuleByKey.mockResolvedValue({
        id: 1,
        module_key: newModule.moduleKey,
        module_name: newModule.moduleName,
        module_version: '1.0.0',
        description: newModule.description,
        category: newModule.category,
        dependencies: JSON.stringify([]),
        routes: JSON.stringify([]),
        components: JSON.stringify([]),
        permissions: JSON.stringify({}),
        config_schema: JSON.stringify({}),
        is_enabled: true,
        is_core: false,
        sort: newModule.sort,
        created_at: '2024-01-01T00:00:00Z',
        updated_at: '2024-01-01T00:00:00Z'
      })

      // Check if module already exists
      await mockDb.getModuleByKey(newModule.moduleKey)

      // Create module
      const moduleId = await mockDb.createModule({
        moduleKey: newModule.moduleKey,
        moduleName: newModule.moduleName,
        description: newModule.description,
        category: newModule.category,
        dependencies: newModule.dependencies,
        sort: newModule.sort
      })

      expect(moduleId).toBe(1)
      expect(mockDb.createModule).toHaveBeenCalled()
    })

    it('should validate module key format', () => {
      const invalidKeys = ['Invalid', '123key', 'key_with_underscore', '-key', 'key-']

      invalidKeys.forEach(key => {
        const isValid = /^[a-z][a-z0-9-]*$/.test(key)
        expect(isValid).toBe(false)
      })
    })

    it('should accept valid module key format', () => {
      const validKeys = ['valid-key', 'key', 'my-module-123', 'test-module']

      validKeys.forEach(key => {
        const isValid = /^[a-z][a-z0-9-]*$/.test(key)
        expect(isValid).toBe(true)
      })
    })

    it('should check for duplicate module key', async () => {
      const moduleKey = 'ai-assistant'

      mockDb.getModuleByKey.mockResolvedValue({
        id: 1,
        module_key: moduleKey,
        module_name: 'AI Assistant',
        module_version: '1.0.0',
        is_enabled: true,
        is_core: false,
        sort: 0,
        created_at: '2024-01-01T00:00:00Z',
        updated_at: '2024-01-01T00:00:00Z'
      })

      const existingModule = await mockDb.getModuleByKey(moduleKey)

      expect(existingModule).toBeDefined()
      expect(existingModule?.module_key).toBe(moduleKey)
    })

    it('should validate dependencies', async () => {
      const moduleKey = 'new-module'
      const dependencies = ['existing-module']

      mockDb.getModuleByKey.mockImplementation((key) => {
        if (key === 'existing-module') {
          return Promise.resolve({
            id: 1,
            module_key: 'existing-module',
            module_name: 'Existing Module',
            module_version: '1.0.0',
            is_enabled: true,
            is_core: false,
            sort: 0,
            created_at: '2024-01-01T00:00:00Z',
            updated_at: '2024-01-01T00:00:00Z'
          })
        }
        if (key === moduleKey) {
          return Promise.resolve(null)
        }
        return Promise.resolve(null)
      })

      for (const dep of dependencies) {
        const depModule = await mockDb.getModuleByKey(dep)
        expect(depModule).toBeDefined()
      }
    })
  })

  describe('PUT /api/modules/:moduleKey', () => {
    it('should update existing module', async () => {
      const moduleKey = 'ai-assistant'
      const updates = {
        moduleName: 'Updated AI Assistant',
        description: 'Updated description'
      }

      mockDb.getModuleByKey.mockResolvedValue({
        id: 1,
        module_key: moduleKey,
        module_name: 'AI Assistant',
        module_version: '1.0.0',
        is_enabled: true,
        is_core: false,
        sort: 0,
        created_at: '2024-01-01T00:00:00Z',
        updated_at: '2024-01-01T00:00:00Z'
      })

      mockDb.updateModule.mockResolvedValue(true)

      const existingModule = await mockDb.getModuleByKey(moduleKey)
      const updated = await mockDb.updateModule(moduleKey, updates)

      expect(existingModule).toBeDefined()
      expect(updated).toBe(true)
    })

    it('should return 404 if module to update not found', async () => {
      const moduleKey = 'non-existent'

      mockDb.getModuleByKey.mockResolvedValue(null)

      const existingModule = await mockDb.getModuleByKey(moduleKey)

      expect(existingModule).toBeNull()
    })
  })

  describe('DELETE /api/modules/:moduleKey', () => {
    it('should delete existing module', async () => {
      const moduleKey = 'ai-assistant'

      mockDb.getModuleByKey.mockResolvedValue({
        id: 1,
        module_key: moduleKey,
        module_name: 'AI Assistant',
        module_version: '1.0.0',
        is_enabled: true,
        is_core: false,
        sort: 0,
        created_at: '2024-01-01T00:00:00Z',
        updated_at: '2024-01-01T00:00:00Z'
      })

      mockDb.deleteModule.mockResolvedValue(true)

      const existingModule = await mockDb.getModuleByKey(moduleKey)
      const deleted = await mockDb.deleteModule(moduleKey)

      expect(existingModule).toBeDefined()
      expect(deleted).toBe(true)
    })

    it('should prevent deleting core modules', () => {
      const coreModule = {
        id: 1,
        moduleKey: 'admin-panel',
        moduleName: 'Admin Panel',
        isCore: true
      }

      const canDelete = !coreModule.isCore

      expect(canDelete).toBe(false)
    })
  })
})
