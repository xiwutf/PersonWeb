/**
 * 模块配置 Composable
 * 用于获取和设置模块配置
 */

import type { ModuleConfig } from '~/types/module'
import { useSimpleModuleSystem } from './simpleModuleSystem'

const moduleConfigs = ref<Map<string, Record<string, any>>>(new Map())

/**
 * 获取模块配置
 */
export const useModuleConfig = (moduleKey: string) => {
  const { getModuleConfig, getModuleConfigSync, setModuleConfig } = useSimpleModuleSystem()

  /**
   * 获取模块配置（异步）
   */
  const fetchConfig = async () => {
    try {
      const config = await getModuleConfig(moduleKey)
      if (config && !moduleConfigs.value.has(moduleKey)) {
        moduleConfigs.value.set(moduleKey, config)
      }
      return config
    } catch (e) {
      console.error(`Failed to fetch config for module ${moduleKey}:`, e)
      return null
    }
  }

  /**
   * 获取配置项
   */
  const getConfig = async (key?: string) => {
    if (!moduleConfigs.value.has(moduleKey)) {
      await fetchConfig()
    }

    const config = moduleConfigs.value.get(moduleKey)
    if (!config) return null

    if (key) {
      return config[key]
    }
    return config
  }

  /**
   * 设置配置项
   */
  const setConfig = async (key: string, value: any) => {
    try {
      await setModuleConfig(moduleKey, key, value)

      // 更新本地缓存
      if (!moduleConfigs.value.has(moduleKey)) {
        moduleConfigs.value.set(moduleKey, {})
      }

      moduleConfigs.value.get(moduleKey)![key] = value

      return true
    } catch (e) {
      console.error(`Failed to set config for module ${moduleKey}:`, e)
      return false
    }
  }

  /**
   * 更新多个配置项
   */
  const updateConfig = async (updates: Record<string, any>) => {
    try {
      await $fetch(`/api/module/${moduleKey}/config`, {
        method: 'POST',
        body: {
          config: updates
        }
      })

      // 更新本地缓存
      if (!moduleConfigs.value.has(moduleKey)) {
        moduleConfigs.value.set(moduleKey, {})
      }

      const currentConfig = moduleConfigs.value.get(moduleKey)!
      Object.assign(currentConfig, updates)

      return true
    } catch (e) {
      console.error(`Failed to update config for module ${moduleKey}:`, e)
      return false
    }
  }

  /**
   * 获取所有配置
   */
  const getAllConfig = async () => {
    if (!moduleConfigs.value.has(moduleKey)) {
      await fetchConfig()
    }

    return moduleConfigs.value.get(moduleKey) || {}
  }

  /**
   * 重置配置为默认值
   */
  const resetConfig = async () => {
    try {
      await $fetch(`/api/module/${moduleKey}/config/reset`, {
        method: 'POST'
      })

      moduleConfigs.value.delete(moduleKey)

      return true
    } catch (e) {
      console.error(`Failed to reset config for module ${moduleKey}:`, e)
      return false
    }
  }

  return {
    // 状态
    configs: readonly(moduleConfigs),

    // 方法
    fetchConfig,
    getConfig,
    setConfig,
    updateConfig,
    getAllConfig,
    resetConfig
  }
}