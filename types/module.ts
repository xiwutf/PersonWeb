/**
 * 模块系统类型定义
 */

export interface ModuleDefinition {
  id: number
  moduleKey: string
  moduleName: string
  moduleVersion: string
  description?: string
  author?: string
  icon?: string
  category?: string
  dependencies?: string[]
  routes?: string[]
  components?: string[]
  permissions?: Record<string, any>
  configSchema?: Record<string, any>
  isEnabled: boolean
  isCore: boolean
  sort: number
  createdAt: string
  updatedAt: string
}

export interface ModuleConfig {
  id: number
  moduleKey: string
  configKey: string
  configValue: any
  description?: string
  createdAt: string
  updatedAt: string
}

export interface ModuleRoute {
  path: string
  name?: string
  component?: string
  meta?: Record<string, any>
  children?: ModuleRoute[]
}

export interface ModuleComponent {
  name: string
  path: string
  global?: boolean
  lazy?: boolean
}

export interface ModuleManifest {
  moduleKey: string
  moduleName: string
  version: string
  description?: string
  author?: string
  icon?: string
  category?: string
  dependencies?: string[]
  routes?: ModuleRoute[]
  components?: ModuleComponent[]
  permissions?: Record<string, any>
  configSchema?: Record<string, any>
  entry?: string
}

export interface ModulePermission {
  key: string
  name: string
  description: string
  level: 'basic' | 'advanced' | 'admin'
}

export interface ConfigSchemaProperty {
  type: 'string' | 'number' | 'boolean' | 'object' | 'array'
  required?: boolean
  default?: any
  description?: string
  min?: number
  max?: number
  pattern?: string
  enum?: string[]
}

export interface CreateModuleRequest {
  moduleKey: string
  moduleName: string
  description?: string
  author?: string
  icon?: string
  category: string
  dependencies?: string[]
  routes?: ModuleRoute[]
  components?: ModuleComponent[]
  permissions?: ModulePermission[]
  configSchema?: Record<string, ConfigSchemaProperty>
  sort?: number
}

export interface UpdateModuleRequest extends Partial<CreateModuleRequest> {
  version?: string
  isEnabled?: boolean
}

export interface ModuleDependencyGraph {
  [key: string]: {
    dependsOn: string[]
    dependents: string[]
  }
}

export interface ModuleStats {
  totalModules: number
  enabledModules: number
  disabledModules: number
  coreModules: number
  categoryDistribution: Record<string, number>
  recentDownloads: number
  avgRating: number
}

export interface ModuleVersion {
  id: number
  moduleKey: string
  version: string
  packageUrl: string
  packageSize: number
  checksum: string
  changelog?: string
  isLatest: boolean
  isStable: boolean
  publishedAt: string
  createdBy?: string
  createdAt: string
  updatedAt: string
}

export interface ModuleDownload {
  id: number
  moduleKey: string
  version: string
  userId?: number
  ipAddress: string
  userAgent?: string
  downloadedAt: string
}

export interface ModuleReview {
  id: number
  moduleKey: string
  version: string
  userId: number
  rating: number
  title?: string
  content?: string
  isVerified: boolean
  createdAt: string
  updatedAt: string
}

export interface ModuleCompatibility {
  id: number
  moduleKey: string
  version: string
  nuxtVersionMin?: string
  nuxtVersionMax?: string
  nodeVersionMin?: string
  browserCompatibility?: {
    chrome?: string
    firefox?: string
    safari?: string
    edge?: string
  }
  dependencies?: {
    [key: string]: string
  }
  createdAt: string
  updatedAt: string
}

export interface UploadModuleRequest {
  moduleKey: string
  version: string
  file: File
  changelog?: string
  isStable?: boolean
}

export interface CreateVersionRequest {
  moduleKey: string
  version: string
  changelog?: string
  isStable?: boolean
}

export interface VersionDiff {
  added: string[]
  removed: string[]
  changed: string[]
  renamed: string[]
}

export interface ModuleRating {
  averageRating: number
  totalReviews: number
  distribution: {
    5: number
    4: number
    3: number
    2: number
    1: number
  }
}

