/**
 * 模块管理器 Composable
 * 提供高级的模块管理功能，包括安装、卸载、配置等
 */

import type { ModuleDefinition, ModuleManifest, ModuleInstance, ModuleStatus, ModuleFilter } from '~/types/module'
import { useModuleCore } from './useModuleCore'
import { useModuleTools } from './useModuleCore'

/**
 * 模块管理器
 */
export const useModuleManager = () => {
  const {
    moduleInstances,
    moduleStatus,
    installModule,
    uninstallModule,
    activateModule,
    deactivateModule,
    updateModuleConfig,
    isModuleInstalled,
    isModuleActive,
    onModuleEvent
  } = useModuleCore()

  const { createRoute, createComponent, createConfig, createPermission } = useModuleTools()

  /**
   * 批量安装模块
   */
  const installModules = async (moduleKeys: string[], options?: {
    config?: Record<string, Record<string, any>>
    activate?: boolean
  }): Promise<{ success: string[]; failed: Array<{ key: string; error: string }> }> => {
    const success: string[] = []
    const failed: Array<{ key: string; error: string }> = []

    for (const moduleKey of moduleKeys) {
      try {
        const config = options?.config?.[moduleKey] || {}
        await installModule(moduleKey, {
          config,
          activate: options?.activate ?? true
        })
        success.push(moduleKey)
      } catch (e) {
        failed.push({
          key: moduleKey,
          error: e instanceof Error ? e.message : 'Unknown error'
        })
      }
    }

    return { success, failed }
  }

  /**
   * 批量卸载模块
   */
  const uninstallModules = async (moduleKeys: string[]): Promise<{ success: string[]; failed: Array<{ key: string; error: string }> }> => {
    const success: string[] = []
    const failed: Array<{ key: string; error: string }> = []

    for (const moduleKey of moduleKeys) {
      try {
        await uninstallModule(moduleKey)
        success.push(moduleKey)
      } catch (e) {
        failed.push({
          key: moduleKey,
          error: e instanceof Error ? e.message : 'Unknown error'
        })
      }
    }

    return { success, failed }
  }

  /**
   * 导出模块配置
   */
  const exportModuleConfig = (moduleKey: string): string => {
    const instance = moduleInstances.value.get(moduleKey)
    if (!instance) {
      throw new Error(`Module ${moduleKey} not installed`)
    }

    return JSON.stringify({
      moduleKey,
      version: instance.definition.meta.version,
      config: instance.config,
      exportedAt: new Date().toISOString()
    }, null, 2)
  }

  /**
   * 导入模块配置
   */
  const importModuleConfig = async (moduleKey: string, configJson: string): Promise<boolean> => {
    try {
      const config = JSON.parse(configJson)

      // 验证配置格式
      if (!config.moduleKey || !config.config) {
        throw new Error('Invalid config format')
      }

      if (config.moduleKey !== moduleKey) {
        throw new Error('Module key mismatch')
      }

      return await updateModuleConfig(moduleKey, config.config)
    } catch (e) {
      console.error('Failed to import module config:', e)
      return false
    }
  }

  /**
   * 重置模块配置
   */
  const resetModuleConfig = async (moduleKey: string): Promise<boolean> => {
    const instance = moduleInstances.value.get(moduleKey)
    if (!instance) {
      throw new Error(`Module ${moduleKey} not installed`)
    }

    // 获取默认配置
    const defaultConfig: Record<string, any> = {}
    if (instance.definition.configs) {
      instance.definition.configs.forEach(config => {
        defaultConfig[config.configKey] = config.defaultValue
      })
    }

    return await updateModuleConfig(moduleKey, defaultConfig)
  }

  /**
   * 检查模块依赖
   */
  const checkModuleDependencies = (moduleKey: string): {
    valid: boolean
    missing: string[]
    conflicts: Array<{ module: string; version: string; required: string }>
  } => {
    const instance = moduleInstances.value.get(moduleKey)
    if (!instance) {
      return {
        valid: false,
        missing: [moduleKey],
        conflicts: []
      }
    }

    const missing: string[] = []
    const conflicts: Array<{ module: string; version: string; required: string }> = []

    if (instance.definition.meta.dependencies) {
      for (const dep of instance.definition.meta.dependencies) {
        const depInstance = moduleInstances.value.get(dep)
        if (!depInstance) {
          missing.push(dep)
        } else {
          // 检查版本兼容性
          if (depInstance.definition.meta.version !== instance.definition.meta.dependenciesVersion?.[dep]) {
            conflicts.push({
              module: dep,
              version: depInstance.definition.meta.version,
              required: instance.definition.meta.dependenciesVersion?.[dep] || 'any'
            })
          }
        }
      }
    }

    return {
      valid: missing.length === 0 && conflicts.length === 0,
      missing,
      conflicts
    }
  }

  /**
   * 获取模块更新信息
   */
  const checkModuleUpdates = async (moduleKey: string): Promise<{
    hasUpdate: boolean
    currentVersion: string
    latestVersion: string
    updateInfo?: {
      changelog: string
      downloadUrl: string
      critical: boolean
    }
  }> => {
    // 这里应该调用后端API检查更新
    const instance = moduleInstances.value.get(moduleKey)
    if (!instance) {
      throw new Error(`Module ${moduleKey} not installed`)
    }

    // 模拟更新检查
    return {
      hasUpdate: Math.random() > 0.7,
      currentVersion: instance.definition.meta.version,
      latestVersion: '1.1.0',
      updateInfo: Math.random() > 0.7 ? {
        changelog: 'Fixed bugs and improved performance',
        downloadUrl: '/api/modules/download/' + moduleKey,
        critical: false
      } : undefined
    }
  }

  /**
   * 更新模块
   */
  const updateModule = async (moduleKey: string, version?: string): Promise<boolean> => {
    try {
      const updateInfo = await checkModuleUpdates(moduleKey)
      if (!updateInfo.hasUpdate) {
        console.log(`Module ${moduleKey} is already up to date`)
        return true
      }

      // 先卸载旧版本
      await uninstallModule(moduleKey)

      // 安装新版本
      return await installModule(moduleKey, {
        activate: true,
        config: moduleInstances.value.get(moduleKey)?.config || {}
      })
    } catch (e) {
      console.error('Failed to update module:', e)
      return false
    }
  }

  /**
   * 获取模块统计信息
   */
  const getModuleStats = () => {
    const total = moduleInstances.value.size
    const active = Array.from(moduleStatus.value.values()).filter(status => status === ModuleStatus.ACTIVE).length
    const inactive = total - active

    const byCategory = new Map<string, number>()
    moduleInstances.value.forEach(instance => {
      const category = instance.definition.meta.category
      byCategory.set(category, (byCategory.get(category) || 0) + 1)
    })

    return {
      total,
      active,
      inactive,
      byCategory: Object.fromEntries(byCategory)
    }
  }

  /**
   * 搜索模块
   */
  const searchModules = (query: string, filters?: {
    category?: string
    freeOnly?: boolean
    installedOnly?: boolean
  }): ModuleInstance[] => {
    const allInstances = Array.from(moduleInstances.value.values())

    return allInstances.filter(instance => {
      // 文本搜索
      if (query) {
        const searchableText = [
          instance.definition.meta.name,
          instance.definition.meta.description,
          ...instance.definition.meta.tags
        ].join(' ').toLowerCase()

        if (!searchableText.includes(query.toLowerCase())) {
          return false
        }
      }

      // 分类过滤
      if (filters?.category && instance.definition.meta.category !== filters.category) {
        return false
      }

      // 免费过滤
      if (filters?.freeOnly && instance.definition.meta.price) {
        return false
      }

      // 已安装过滤
      if (filters?.installedOnly && !moduleInstances.value.has(instance.definition.meta.key)) {
        return false
      }

      return true
    })
  }

  /**
   * 监控模块加载状态
   */
  const onModuleLoad = (callback: (moduleKey: string, status: ModuleStatus) => void): () => void => {
    return onModuleEvent('module:loaded', (event: any) => {
      callback(event.moduleKey, event.status)
    })
  }

  /**
   * 监控模块错误
   */
  const onModuleError = (callback: (moduleKey: string, error: string) => void): () => void => {
    return onModuleEvent('module:error', (event: any) => {
      callback(event.moduleKey, event.error)
    })
  }

  return {
    // 基础操作
    installModule,
    uninstallModule,
    activateModule,
    deactivateModule,
    isModuleInstalled,
    isModuleActive,

    // 批量操作
    installModules,
    uninstallModules,

    // 配置管理
    exportModuleConfig,
    importModuleConfig,
    resetModuleConfig,

    // 依赖管理
    checkModuleDependencies,

    // 更新管理
    checkModuleUpdates,
    updateModule,

    // 查询和搜索
    searchModules,
    getModuleStats,

    // 监控
    onModuleLoad,
    onModuleError,

    // 工具
    createRoute,
    createComponent,
    createConfig,
    createPermission
  }
}