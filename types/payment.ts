/**
 * 支付和许可证系统类型定义
 */

export interface Order {
  id: number
  orderNumber: string
  userId: number
  moduleKey: string
  version: string
  type: 'module' | 'subscription' | 'trial'
  price: number
  currency: string
  status: 'pending' | 'paid' | 'cancelled' | 'refunded' | 'expired'
  paymentMethod?: string
  paymentGateway?: string
  transactionId?: string
  licenseKey?: string
  validFrom?: string
  validUntil?: string
  maxActivations: number
  activationsUsed: number
  createdAt: string
  updatedAt: string
}

export interface License {
  id: number
  licenseKey: string
  orderId: number
  moduleKey: string
  /** 购买的模块版本，从 order 表关联获取 */
  version?: string
  userId: number
  type: 'permanent' | 'subscription' | 'trial'
  validFrom: string
  validUntil?: string
  maxActivations: number
  activationsUsed: number
  status: 'active' | 'expired' | 'revoked'
  deviceFingerprint?: string
  lastActivatedAt?: string
  createdAt: string
  updatedAt: string
}

export interface LicenseActivation {
  id: number
  licenseKey: string
  deviceId: string
  deviceName?: string
  ipAddress?: string
  userAgent?: string
  activatedAt: string
}

export interface PaymentTransaction {
  id: number
  orderId: number
  gateway: string
  gatewayTransactionId: string
  amount: number
  currency: string
  status: 'pending' | 'success' | 'failed' | 'cancelled'
  responseData?: any
  errorMessage?: string
  createdAt: string
  updatedAt: string
}

export interface Refund {
  id: number
  orderId: number
  transactionId: string
  amount: number
  reason?: string
  status: 'pending' | 'success' | 'failed'
  processedAt?: string
  createdAt: string
}

export interface SubscriptionPlan {
  id: number
  name: string
  key: string
  description?: string
  priceMonthly?: number
  priceYearly?: number
  modulesIncluded?: string[]
  features?: string[]
  maxDownloads: number
  supportLevel: 'basic' | 'standard' | 'premium'
  trialDays: number
  isActive: boolean
  sortOrder: number
  createdAt: string
  updatedAt: string
}

export interface UserSubscription {
  id: number
  userId: number
  planKey: string
  status: 'active' | 'cancelled' | 'expired'
  billingCycle: 'monthly' | 'yearly'
  currentPeriodStart: string
  currentPeriodEnd: string
  cancelledAt?: string
  expiresAt?: string
  createdAt: string
  updatedAt: string
}

export interface Coupon {
  id: number
  code: string
  type: 'fixed' | 'percentage'
  value: number
  maxUses?: number
  uses: number
  validFrom?: string
  validUntil?: string
  moduleKey?: string
  minAmount?: number
  description?: string
  isActive: boolean
  createdAt: string
}

export interface CreateOrderRequest {
  moduleKey: string
  version: string
  type: 'module' | 'subscription' | 'trial'
  /** 订单金额（元），必填，用于创建订单和支付 */
  price: number
  currency?: string
  couponCode?: string
  paymentMethod?: string
  paymentGateway?: string
}

export interface CreateLicenseRequest {
  orderId: number
  moduleKey: string
  userId: number
  type: 'permanent' | 'subscription' | 'trial'
  validFrom?: string
  validUntil?: string
  maxActivations?: number
}

export interface ActivateLicenseRequest {
  licenseKey: string
  deviceId: string
  deviceName?: string
}

export interface VerifyLicenseRequest {
  licenseKey: string
  deviceId?: string
}

export interface VerifyLicenseResponse {
  isValid: boolean
  license?: License
  error?: string
}

export interface PaymentGatewayConfig {
  provider: 'alipay' | 'wechat' | 'stripe' | 'paypal'
  apiKey: string
  apiSecret?: string
  merchantId?: string
  sandbox: boolean
  webhookSecret?: string
}

export interface PaymentRequest {
  orderId: number
  amount: number
  currency: string
  paymentMethod: string
  paymentGateway: string
  returnUrl: string
  cancelUrl: string
}

export interface PaymentResponse {
  success: boolean
  paymentUrl?: string
  transactionId?: string
  qrCode?: string
  errorMessage?: string
}

export interface WebhookPayload {
  orderId: number
  transactionId: string
  status: 'success' | 'failed' | 'cancelled'
  amount: number
  currency: string
  timestamp: string
  signature: string
  data?: any
}