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

