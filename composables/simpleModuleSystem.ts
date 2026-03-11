/**
 * 简化版模块系统 Composable
 * 用于管理模块的启用/禁用、加载和配置
 */

import type { ModuleDefinition, ModuleConfig, ModuleManifest, ModuleRoute } from '~/types/module'

const moduleRegistry = new Map<string, ModuleManifest>()
const enabledModules = ref<Set<string>>(new Set())
const moduleConfigs = ref<Map<string, Record<string, any>>>(new Map())
const configLoadingStates = ref<Map<string, Promise<void>>>(new Map())

/**
 * 模块系统管理
 */
export const useSimpleModuleSystem = () => {
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
   * 同步获取模块配置
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
   * 加载模块配置
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
   * 启用模块
   */
  const enableModule = async (moduleKey: string) => {
    try {
      await api.post(`/Module/${moduleKey}/enable`)
      enabledModules.value.add(moduleKey)
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
      await api.post(`/Module/${moduleKey}/disable`)
      enabledModules.value.delete(moduleKey)
    } catch (e) {
      console.error(`禁用模块 ${moduleKey} 失败`, e)
      throw e
    }
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
    getModuleConfigSync,
    setModuleConfig,
    enableModule,
    disableModule
  }
}