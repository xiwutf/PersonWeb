/**
 * 模块系统 Composable
 * 用于管理模块的启用/禁用、加载和配置
 */

import type { ModuleDefinition, ModuleConfig, ModuleManifest } from '~/types/module'

const moduleRegistry = new Map<string, ModuleManifest>()
const enabledModules = ref<Set<string>>(new Set())
const moduleConfigs = ref<Map<string, Record<string, any>>>(new Map())

/**
 * 模块系统管理
 */
export const useModuleSystem = () => {
  const api = useApi()

  /**
   * 检查模块是否启用
   */
  const isModuleEnabled = (moduleKey: string): boolean => {
    return enabledModules.value.has(moduleKey)
  }

  /**
   * 获取所有启用的模块
   */
  const getEnabledModules = (): string[] => {
    return Array.from(enabledModules.value)
  }

  /**
   * 获取模块定义
   */
  const getModule = (moduleKey: string): ModuleManifest | null => {
    return moduleRegistry.get(moduleKey) || null
  }

  /**
   * 获取模块配置
   */
  const getModuleConfig = (moduleKey: string, configKey?: string): any => {
    const configs = moduleConfigs.value.get(moduleKey)
    if (!configs) return null

    if (configKey) {
      return configs[configKey]
    }
    return configs
  }

  /**
   * 设置模块配置
   */
  const setModuleConfig = async (moduleKey: string, configKey: string, value: any) => {
    try {
      await api.post('/Module/config', {
        moduleKey,
        configKey,
        configValue: value
      })

      if (!moduleConfigs.value.has(moduleKey)) {
        moduleConfigs.value.set(moduleKey, {})
      }
      const configs = moduleConfigs.value.get(moduleKey)!
      configs[configKey] = value
    } catch (e) {
      console.error(`设置模块 ${moduleKey} 配置失败`, e)
    }
  }

  /**
   * 加载模块定义
   */
  const loadModule = async (moduleKey: string): Promise<ModuleManifest | null> => {
    // 如果已注册，直接返回
    if (moduleRegistry.has(moduleKey)) {
      return moduleRegistry.get(moduleKey)!
    }

    try {
      // 从后端获取模块定义
      const module = await api.get<ModuleDefinition>(`/Module/${moduleKey}`)
      if (!module || !module.isEnabled) {
        return null
      }

      // 转换为 Manifest
      const manifest: ModuleManifest = {
        moduleKey: module.moduleKey,
        moduleName: module.moduleName,
        version: module.moduleVersion,
        description: module.description,
        author: module.author,
        icon: module.icon,
        category: module.category,
        dependencies: module.dependencies || [],
        routes: parseRoutes(module.routes),
        components: parseComponents(module.components),
        permissions: module.permissions,
        configSchema: module.configSchema
      }

      // 注册模块
      moduleRegistry.set(moduleKey, manifest)

      // 加载模块配置
      await loadModuleConfigs(moduleKey)

      return manifest
    } catch (e) {
      console.error(`加载模块 ${moduleKey} 失败`, e)
      return null
    }
  }

  /**
   * 加载模块配置
   */
  const loadModuleConfigs = async (moduleKey: string) => {
    try {
      const configs = await api.get<ModuleConfig[]>(`/Module/${moduleKey}/configs`)
      if (configs && Array.isArray(configs)) {
        const configMap: Record<string, any> = {}
        configs.forEach(config => {
          try {
            configMap[config.configKey] = typeof config.configValue === 'string'
              ? JSON.parse(config.configValue)
              : config.configValue
          } catch (e) {
            configMap[config.configKey] = config.configValue
          }
        })
        moduleConfigs.value.set(moduleKey, configMap)
      }
    } catch (e) {
      console.warn(`加载模块 ${moduleKey} 配置失败`, e)
    }
  }

  /**
   * 加载所有启用的模块
   */
  const loadEnabledModules = async () => {
    try {
      const modules = await api.get<ModuleDefinition[]>('/Module/enabled')
      if (modules && Array.isArray(modules)) {
        enabledModules.value.clear()
        
        for (const module of modules) {
          enabledModules.value.add(module.moduleKey)
          
          // 注册模块
          const manifest: ModuleManifest = {
            moduleKey: module.moduleKey,
            moduleName: module.moduleName,
            version: module.moduleVersion,
            description: module.description,
            author: module.author,
            icon: module.icon,
            category: module.category,
            dependencies: module.dependencies || [],
            routes: parseRoutes(module.routes),
            components: parseComponents(module.components),
            permissions: module.permissions,
            configSchema: module.configSchema
          }
          
          moduleRegistry.set(module.moduleKey, manifest)
          
          // 加载配置
          await loadModuleConfigs(module.moduleKey)
        }
      }
    } catch (e) {
      console.error('加载启用模块失败', e)
    }
  }

  /**
   * 启用模块
   */
  const enableModule = async (moduleKey: string) => {
    try {
      await api.post(`/Module/${moduleKey}/enable`)
      enabledModules.value.add(moduleKey)
      
      // 加载模块
      await loadModule(moduleKey)
    } catch (e) {
      console.error(`启用模块 ${moduleKey} 失败`, e)
      throw e
    }
  }

  /**
   * 禁用模块
   */
  const disableModule = async (moduleKey: string) => {
    try {
      const module = moduleRegistry.get(moduleKey)
      if (module?.isCore) {
        throw new Error('核心模块不能禁用')
      }

      await api.post(`/Module/${moduleKey}/disable`)
      enabledModules.value.delete(moduleKey)
    } catch (e) {
      console.error(`禁用模块 ${moduleKey} 失败`, e)
      throw e
    }
  }

  /**
   * 检查路由是否属于启用的模块
   */
  const isRouteEnabled = (path: string): boolean => {
    // 核心路由始终启用
    if (path === '/' || path === '/about') {
      return true
    }

    for (const [moduleKey, manifest] of moduleRegistry.entries()) {
      if (!enabledModules.value.has(moduleKey)) continue

      if (manifest.routes) {
        for (const route of manifest.routes) {
          if (matchRoute(path, route)) {
            return true
          }
        }
      }
    }

    return false
  }

  /**
   * 检查组件是否属于启用的模块
   */
  const isComponentEnabled = (componentName: string): boolean => {
    for (const [moduleKey, manifest] of moduleRegistry.entries()) {
      if (!enabledModules.value.has(moduleKey)) continue

      if (manifest.components) {
        const component = manifest.components.find(c => c.name === componentName)
        if (component) {
          return true
        }
      }
    }

    return false
  }

  /**
   * 获取模块的路由列表
   */
  const getModuleRoutes = (moduleKey: string): string[] => {
    const manifest = moduleRegistry.get(moduleKey)
    if (!manifest || !manifest.routes) {
      return []
    }

    return manifest.routes.map(route => route.path)
  }

  /**
   * 获取模块的组件列表
   */
  const getModuleComponents = (moduleKey: string): string[] => {
    const manifest = moduleRegistry.get(moduleKey)
    if (!manifest || !manifest.components) {
      return []
    }

    return manifest.components.map(comp => comp.name)
  }

  return {
    // 状态
    enabledModules: readonly(enabledModules),
    moduleRegistry: readonly(moduleRegistry),
    moduleConfigs: readonly(moduleConfigs),

    // 方法
    isModuleEnabled,
    getEnabledModules,
    getModule,
    getModuleConfig,
    setModuleConfig,
    loadModule,
    loadEnabledModules,
    enableModule,
    disableModule,
    isRouteEnabled,
    isComponentEnabled,
    getModuleRoutes,
    getModuleComponents
  }
}

/**
 * 解析路由配置
 */
function parseRoutes(routes?: string | string[]): any[] {
  if (!routes) return []
  
  if (typeof routes === 'string') {
    try {
      return JSON.parse(routes)
    } catch {
      return []
    }
  }
  
  return routes
}

/**
 * 解析组件配置
 */
function parseComponents(components?: string | string[]): any[] {
  if (!components) return []
  
  if (typeof components === 'string') {
    try {
      return JSON.parse(components)
    } catch {
      return []
    }
  }
  
  return components.map(name => ({ name, path: `components/${name}.vue` }))
}

/**
 * 匹配路由
 */
function matchRoute(path: string, route: any): boolean {
  if (route.path === path) {
    return true
  }

  // 支持动态路由匹配
  const routePattern = route.path
    .replace(/\[([^\]]+)\]/g, '([^/]+)')
    .replace(/\*\*/g, '.*')
    .replace(/\*/g, '[^/]*')

  const regex = new RegExp(`^${routePattern}$`)
  return regex.test(path)
}

