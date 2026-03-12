import { H3Event } from 'h3'
import { License, CreateLicenseRequest, ActivateLicenseRequest, VerifyLicenseRequest, VerifyLicenseResponse } from '~/types/payment'
import { query } from '~/server/services/database'

// 生成许可证密钥
export function generateLicenseKey(): string {
  const prefix = 'MOD-'
  const timestamp = Date.now().toString(36)
  const random = Math.random().toString(36).substring(2, 8)
  const checksum = generateChecksum(timestamp + random)
  return `${prefix}${timestamp}-${random}-${checksum}`
}

// 生成校验和
function generateChecksum(input: string): string {
  // 简单的校验和算法，实际可以使用更复杂的算法
  let sum = 0
  for (let i = 0; i < input.length; i++) {
    sum += input.charCodeAt(i)
  }
  return sum.toString(16).substring(0, 4)
}

// 创建许可证
export async function createLicense(licenseData: CreateLicenseRequest): Promise<License> {
  // 生成许可证密钥
  const licenseKey = generateLicenseKey()

  // 计算有效期
  const validFrom = licenseData.validFrom || new Date().toISOString()
  let validUntil: string | undefined

  if (licenseData.type === 'subscription') {
    const days = 30 // 默认30天订阅
    validUntil = new Date(Date.now() + days * 24 * 60 * 60 * 1000).toISOString()
  } else if (licenseData.type === 'trial') {
    const days = 7 // 默认7天试用期
    validUntil = new Date(Date.now() + days * 24 * 60 * 60 * 1000).toISOString()
  }

  const insertData = {
    license_key: licenseKey,
    order_id: licenseData.orderId,
    module_key: licenseData.moduleKey,
    user_id: licenseData.userId,
    type: licenseData.type,
    valid_from: validFrom,
    valid_until: validUntil,
    max_activations: licenseData.maxActivations || 1,
    activations_used: 0,
    status: 'active'
  }

  const result = await query(
    'INSERT INTO `license` SET ?',
    [insertData]
  ) as { insertId: number }

  return {
    id: result.insertId,
    licenseKey,
    orderId: licenseData.orderId,
    moduleKey: licenseData.moduleKey,
    userId: licenseData.userId,
    type: licenseData.type,
    validFrom,
    validUntil,
    maxActivations: licenseData.maxActivations || 1,
    activationsUsed: 0,
    status: 'active',
    deviceFingerprint: null,
    lastActivatedAt: null,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  }
}

// 激活许可证（可选传入 event 以记录 IP 和 User-Agent）
export async function activateLicense(
  activationData: ActivateLicenseRequest,
  event?: H3Event
): Promise<License> {
  // 查找许可证
  const licenses = await query(
    'SELECT * FROM `license` WHERE license_key = ? AND status = ?',
    [activationData.licenseKey, 'active']
  ) as any[]

  if (!licenses[0]) {
    throw new Error('License not found or inactive')
  }

  const license = licenses[0]

  // 检查激活次数限制
  if (license.activations_used >= license.max_activations) {
    throw new Error('Maximum activations exceeded')
  }

  // 检查是否已在该设备激活
  const existingActivations = await query(
    'SELECT * FROM `license_activation` WHERE license_key = ? AND device_id = ?',
    [activationData.licenseKey, activationData.deviceId]
  ) as any[]

  if (existingActivations[0]) {
    return license // 已激活过，直接返回
  }

  // 记录激活（传入 event 时记录 IP 和 User-Agent）
  await query(
    `INSERT INTO \`license_activation\` (
      license_key, device_id, device_name, ip_address, user_agent, activated_at
    ) VALUES (?, ?, ?, ?, ?, ?)`,
    [
      activationData.licenseKey,
      activationData.deviceId,
      activationData.deviceName || null,
      event ? getIPFromRequest(event) : null,
      event ? getUserAgentFromRequest(event) : null,
      new Date().toISOString()
    ]
  )

  // 更新激活次数
  await query(
    'UPDATE `license` SET activations_used = ?, last_activated_at = ? WHERE id = ?',
    [license.activations_used + 1, new Date().toISOString(), license.id]
  )

  // 获取更新后的许可证
  const updatedLicenses = await query(
    'SELECT * FROM `license` WHERE id = ?',
    [license.id]
  ) as any[]

  return {
    ...updatedLicenses[0],
    licenseKey: updatedLicenses[0].license_key,
    moduleKey: updatedLicenses[0].module_key,
    userId: updatedLicenses[0].user_id,
    validFrom: updatedLicenses[0].valid_from,
    validUntil: updatedLicenses[0].valid_until,
    maxActivations: updatedLicenses[0].max_activations,
    activationsUsed: updatedLicenses[0].activations_used,
    deviceFingerprint: updatedLicenses[0].device_fingerprint,
    lastActivatedAt: updatedLicenses[0].last_activated_at,
    createdAt: updatedLicenses[0].created_at,
    updatedAt: updatedLicenses[0].updated_at
  }
}

// 验证许可证
export async function verifyLicense(verifyData: VerifyLicenseRequest): Promise<VerifyLicenseResponse> {
  try {
    const licenses = await query(
      'SELECT * FROM `license` WHERE license_key = ? AND status = ?',
      [verifyData.licenseKey, 'active']
    ) as any[]

    if (!licenses[0]) {
      return {
        isValid: false,
        error: 'License not found or inactive'
      }
    }

    const license = licenses[0]

    // 检查有效期
    if (license.valid_until && new Date(license.valid_until) < new Date()) {
      await query(
        'UPDATE `license` SET status = ? WHERE id = ?',
        ['expired', license.id]
      )

      return {
        isValid: false,
        error: 'License expired'
      }
    }

    // 如果指定了设备ID，检查是否已激活
    if (verifyData.deviceId) {
      const activations = await query(
        'SELECT * FROM `license_activation` WHERE license_key = ? AND device_id = ?',
        [verifyData.licenseKey, verifyData.deviceId]
      ) as any[]

      if (!activations[0]) {
        return {
          isValid: false,
          error: 'License not activated on this device'
        }
      }
    }

    const resultLicense = {
      ...license,
      licenseKey: license.license_key,
      moduleKey: license.module_key,
      userId: license.user_id,
      validFrom: license.valid_from,
      validUntil: license.valid_until,
      maxActivations: license.max_activations,
      activationsUsed: license.activations_used,
      deviceFingerprint: license.device_fingerprint,
      lastActivatedAt: license.last_activated_at,
      createdAt: license.created_at,
      updatedAt: license.updated_at
    }

    return {
      isValid: true,
      license: resultLicense
    }
  } catch (error) {
    console.error('License verification failed:', error)
    return {
      isValid: false,
      error: error instanceof Error ? error.message : 'Verification failed'
    }
  }
}

// 获取许可证状态
export async function getLicenseStatus(licenseKey: string): Promise<License | null> {
  const licenses = await query(
    'SELECT * FROM `license` WHERE license_key = ?',
    [licenseKey]
  ) as any[]

  if (!licenses[0]) return null

  const license = licenses[0]
  return {
    ...license,
    licenseKey: license.license_key,
    moduleKey: license.module_key,
    userId: license.user_id,
    validFrom: license.valid_from,
    validUntil: license.valid_until,
    maxActivations: license.max_activations,
    activationsUsed: license.activations_used,
    deviceFingerprint: license.device_fingerprint,
    lastActivatedAt: license.last_activated_at,
    createdAt: license.created_at,
    updatedAt: license.updated_at
  }
}

// 列出用户的许可证
export async function getUserLicenses(userId: number): Promise<License[]> {
  const licenses = await query(
    `SELECT l.*, o.version as order_version
     FROM \`license\` l
     LEFT JOIN \`order\` o ON l.order_id = o.id
     WHERE l.user_id = ? ORDER BY l.created_at DESC`,
    [userId]
  ) as any[]

  return licenses.map((license: any) => ({
    ...license,
    licenseKey: license.license_key,
    moduleKey: license.module_key,
    version: license.order_version || '1.0.0',
    userId: license.user_id,
    validFrom: license.valid_from,
    validUntil: license.valid_until,
    maxActivations: license.max_activations,
    activationsUsed: license.activations_used,
    deviceFingerprint: license.device_fingerprint,
    lastActivatedAt: license.last_activated_at,
    createdAt: license.created_at,
    updatedAt: license.updated_at
  }))
}

// 撤销许可证
export async function revokeLicense(licenseKey: string): Promise<void> {
  await query(
    'UPDATE `license` SET status = ?, updated_at = ? WHERE license_key = ?',
    ['revoked', new Date().toISOString(), licenseKey]
  )
}

// 更新许可证状态
export async function updateLicenseStatus(
  licenseKey: string,
  status: 'active' | 'expired' | 'revoked'
): Promise<void> {
  await query(
    'UPDATE `license` SET status = ?, updated_at = ? WHERE license_key = ?',
    [status, new Date().toISOString(), licenseKey]
  )
}

// 检查用户是否有模块的许可证
export async function checkUserModuleLicense(userId: number, moduleKey: string): Promise<boolean> {
  const licenses = await query(
    `SELECT * FROM \`license\`
     WHERE user_id = ?
     AND module_key = ?
     AND status = 'active'
     AND (valid_until IS NULL OR valid_until > ?)`,
    [userId, moduleKey, new Date().toISOString()]
  ) as any[]

  return licenses.length > 0
}

// 辅助函数：从请求中获取IP
function getIPFromRequest(request: ActivateLicenseRequest | H3Event): string | undefined {
  if (request && typeof (request as H3Event).node?.req !== 'undefined') {
    const req = (request as H3Event).node?.req
    const forwarded = req?.headers['x-forwarded-for']
    if (typeof forwarded === 'string') return forwarded.split(',')[0]?.trim()
    return req?.socket?.remoteAddress
  }
  return undefined
}

// 辅助函数：从请求中获取User-Agent
function getUserAgentFromRequest(request: ActivateLicenseRequest | H3Event): string | undefined {
  if (request && typeof (request as H3Event).node?.req !== 'undefined') {
    return (request as H3Event).node?.req?.headers['user-agent']
  }
  return undefined
}