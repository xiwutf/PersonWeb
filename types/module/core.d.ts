/**
 * 模块系统核心接口
 */

import type { ModuleDefinition, ModuleManifest, ModuleInstance, ModuleStatus, ModuleFilter } from './index'

// 模块加载器接口
export interface IModuleLoader {
  /** 加载模块 */
  load(moduleKey: string): Promise<ModuleManifest | null>
  /** 卸载模块 */
  unload(moduleKey: string): Promise<void>
  /** 获取已加载模块 */
  getLoadedModules(): Map<string, ModuleManifest>
}

// 模块管理器接口
export interface IModuleManager {
  // 基础操作
  install(moduleKey: string, options?: any): Promise<ModuleInstance>
  uninstall(moduleKey: string): Promise<boolean>
  activate(moduleKey: string): Promise<boolean>
  deactivate(moduleKey: string): Promise<boolean>

  // 查询操作
  getModule(moduleKey: string): ModuleInstance | null
  listModules(filter?: ModuleFilter): ModuleInstance[]
  getStatus(moduleKey: string): ModuleStatus

  // 配置管理
  updateConfig(moduleKey: string, config: Record<string, any>): Promise<boolean>
  getConfig(moduleKey: string): Record<string, any>

  // 生命周期
  executeHook(moduleKey: string, hookName: string, ...args: any[]): Promise<any>
}

// 模块存储接口
export interface IModuleStorage {
  // 模块实例
  save(instance: ModuleInstance): Promise<boolean>
  load(moduleKey: string): Promise<ModuleInstance | null>
  delete(moduleKey: string): Promise<boolean>
  list(filter?: ModuleFilter): Promise<ModuleInstance[]>

  // 配置存储
  saveConfig(moduleKey: string, config: Record<string, any>): Promise<boolean>
  loadConfig(moduleKey: string): Promise<Record<string, any> | null>
}

// 模块验证器接口
export interface IModuleValidator {
  // 验证模块定义
  validateDefinition(definition: ModuleDefinition): Promise<ValidationResult>
  // 验证模块依赖
  validateDependencies(definition: ModuleDefinition, installedModules: string[]): Promise<DependencyResult>
  // 验证模块权限
  validatePermissions(permissions: ModulePermission[]): Promise<PermissionResult>
}

// 验证结果
export interface ValidationResult {
  valid: boolean
  errors: ValidationError[]
  warnings: ValidationWarning[]
}

export interface ValidationError {
  field: string
  message: string
  code: string
}

export interface ValidationWarning {
  field: string
  message: string
  code: string
}

// 依赖验证结果
export interface DependencyResult {
  valid: boolean
  missing: string[]
  conflicts: Array<{
    module: string
    version: string
    required: string
  }>
}

// 权限验证结果
export interface PermissionResult {
  valid: boolean
  missing: string[]
  excess: string[]
}

// 模块事件总线接口
export interface IModuleEventBus {
  on(event: string, handler: (event: ModuleEvent) => void): void
  off(event: string, handler: (event: ModuleEvent) => void): void
  emit(event: string, data?: any): void
  once(event: string, handler: (event: ModuleEvent) => void): void
}

// 模块插件接口
export interface IModulePlugin {
  name: string
  version: string
  install(bus: IModuleEventBus): void
  uninstall(bus: IModuleEventBus): void
}

// 模块工具接口
export interface IModuleTools {
  // 路由工具
  createRoute(path: string, component?: string): ModuleRoute
  // 组件工具
  createComponent(name: string, path: string): ModuleComponent
  // 配置工具
  createConfig(key: string, value: any): ModuleConfig
  // 权限工具
  createPermission(key: string, name: string, level: string): ModulePermission
}

// 模块构建器接口
export interface IModuleBuilder {
  // 创建模块
  create(name: string, options: ModuleCreateOptions): Promise<ModuleManifest>
  // 打包模块
  build(manifest: ModuleManifest): Promise<string>
  // 发布模块
  publish(manifest: ModuleManifest, repository: string): Promise<string>
}

export interface ModuleCreateOptions {
  category: string
  author: string
  license?: string
  dependencies?: string[]
  routes?: ModuleRoute[]
  components?: ModuleComponent[]
  configs?: ModuleConfig[]
  permissions?: ModulePermission[]
}

// 模块市场接口
export interface IModuleMarket {
  // 搜索模块
  search(query: string, category?: string): Promise<ModuleMarketItem[]>
  // 获取模块详情
  getDetail(moduleKey: string): Promise<ModuleMarketItem | null>
  // 下载模块
  download(moduleKey: string, version?: string): Promise<ModuleManifest>
  // 安装模块
  install(moduleKey: string, options?: any): Promise<ModuleInstance>
}

export interface ModuleMarketItem {
  key: string
  name: string
  version: string
  description: string
  author: string
  category: string
  price?: number
  downloads: number
  rating: number
  screenshots?: string[]
  tags: string[]
  dependencies?: string[]
}