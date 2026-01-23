/**
 * 模块系统 Composable
 * 用于管理模块的启用/禁用、加载和配置
 */

import type { ModuleDefinition, ModuleConfig, ModuleManifest, ModuleRoute } from '~/types/module'

const moduleRegistry = new Map<string, ModuleManifest>()
const enabledModules = ref<Set<string>>(new Set())
const moduleConfigs = ref<Map<string, Record<string, any>>>(new Map())
// 记录配置加载状态，避免重复请求
const configLoadingStates = ref<Map<string, Promise<void>>>(new Map())

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
   * 获取模块配置（懒加载）
   * 如果配置未加载，会自动加载
   */
  const getModuleConfig = async (moduleKey: string, configKey?: string): Promise<any> => {
    // 如果配置已加载，直接返回
    if (moduleConfigs.value.has(moduleKey)) {
      const configs = moduleConfigs.value.get(moduleKey)!
      if (configKey) {
        return configs[configKey]
      }
      return configs
    }

    // 如果正在加载中，等待加载完成
    const loadingPromise = configLoadingStates.value.get(moduleKey)
    if (loadingPromise) {
      await loadingPromise
      const configs = moduleConfigs.value.get(moduleKey)
      if (configs) {
        if (configKey) {
          return configs[configKey]
        }
        return configs
      }
      return null
    }

    // 首次访问，触发懒加载
    await loadModuleConfigs(moduleKey)
    const configs = moduleConfigs.value.get(moduleKey)
    if (configs) {
      if (configKey) {
        return configs[configKey]
      }
      return configs
    }
    return null
  }

  /**
   * 同步获取模块配置（不触发懒加载）
   * 仅用于已加载的配置，如果未加载返回 null
   */
  const getModuleConfigSync = (moduleKey: string, configKey?: string): any => {
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
   * 加载模块配置（带缓存和防重复请求）
   */
  const loadModuleConfigs = async (moduleKey: string) => {
    // 如果已加载，直接返回
    if (moduleConfigs.value.has(moduleKey)) {
      return
    }

    // 如果正在加载中，返回现有的 Promise
    const existingPromise = configLoadingStates.value.get(moduleKey)
    if (existingPromise) {
      return existingPromise
    }

    // 创建新的加载 Promise
    const loadingPromise = (async () => {
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
      } finally {
        // 加载完成后移除加载状态
        configLoadingStates.value.delete(moduleKey)
      }
    })()

    // 记录加载状态
    configLoadingStates.value.set(moduleKey, loadingPromise)
    await loadingPromise
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
          
          // 不再在启动时加载配置，改为按需懒加载
          // 配置会在首次调用 getModuleConfig() 时自动加载
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

    // /tools 路由始终启用，不依赖模块系统
    if (path === '/tools' || path.startsWith('/tools/')) {
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
    getModuleConfig, // 异步懒加载版本
    getModuleConfigSync, // 同步版本（仅用于已加载的配置）
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
 * 将字符串数组转换为对象数组，确保每个路由都有 path 属性
 */
function parseRoutes(routes?: string | string[]): ModuleRoute[] {
  if (!routes) return []
  
  let parsed: any[] = []
  
  if (typeof routes === 'string') {
    try {
      parsed = JSON.parse(routes)
    } catch {
      return []
    }
  } else {
    parsed = routes
  }
  
  // 将字符串数组转换为对象数组
  return parsed.map(route => {
    if (typeof route === 'string') {
      return { path: route }
    }
    // 如果已经是对象，确保有 path 属性
    if (route && typeof route === 'object' && route.path) {
      return route
    }
    // 如果对象没有 path 属性，尝试使用其他可能的属性
    return { path: route.path || route.route || String(route) }
  })
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
 * 支持字符串和对象两种格式的路由
 */
function matchRoute(path: string, route: any): boolean {
  // 支持字符串类型的路由（向后兼容）
  const routePath = typeof route === 'string' ? route : (route?.path || '')
  
  if (!routePath) {
    return false
  }
  
  // 精确匹配
  if (routePath === path) {
    return true
  }

  // 支持动态路由匹配
  // 将 [slug] 转换为正则表达式 ([^/]+)
  const routePattern = routePath
    .replace(/\[([^\]]+)\]/g, '([^/]+)')
    .replace(/\*\*/g, '.*')
    .replace(/\*/g, '[^/]*')

  const regex = new RegExp(`^${routePattern}$`)
  return regex.test(path)
}

