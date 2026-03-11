/**
 * 模块系统核心 Composable
 * 提供模块注册、管理、生命周期等核心功能
 */

import type {
  ModuleDefinition,
  ModuleManifest,
  ModuleInstance,
  ModuleStatus,
  ModuleFilter,
  ModuleRegistry
} from '~/types/module'

// 模块注册表
const moduleRegistry = new Map<string, ModuleManifest>()
const moduleInstances = ref<Map<string, ModuleInstance>>(new Map())
const moduleStatus = ref<Map<string, ModuleStatus>>(new Map())
const eventBus = ref<any>(null)

// 初始化事件总线
const initEventBus = () => {
  if (!eventBus.value) {
    eventBus.value = {
      listeners: new Map(),
      on(event: string, handler: Function) {
        if (!this.listeners.has(event)) {
          this.listeners.set(event, new Set())
        }
        this.listeners.get(event).add(handler)
      },
      off(event: string, handler: Function) {
        const handlers = this.listeners.get(event)
        if (handlers) {
          handlers.delete(handler)
        }
      },
      emit(event: string, data?: any) {
        const handlers = this.listeners.get(event)
        if (handlers) {
          handlers.forEach(handler => {
            try {
              handler({ type: event, data, timestamp: Date.now() })
            } catch (e) {
              console.error(`Module event handler error: ${event}`, e)
            }
          })
        }
      },
      once(event: string, handler: Function) {
        const onceHandler = (data: any) => {
          handler(data)
          this.off(event, onceHandler)
        }
        this.on(event, onceHandler)
      }
    }
  }
  return eventBus.value
}

/**
 * 模块系统核心管理
 */
export const useModuleCore = () => {
  const api = useApi()
  const { isModuleEnabled } = useModuleSystem()

  /**
   * 注册模块
   */
  const registerModule = async (manifest: ModuleManifest): Promise<boolean> => {
    try {
      // 验证模块定义
      const validation = await validateModuleDefinition(manifest.module)
      if (!validation.valid) {
        console.error('Module validation failed:', validation.errors)
        return false
      }

      // 检查是否已注册
      if (moduleRegistry.has(manifest.module.meta.key)) {
        console.warn(`Module ${manifest.module.meta.key} already registered`)
        return false
      }

      // 注册模块
      moduleRegistry.set(manifest.module.meta.key, manifest)

      // 触发注册事件
      const bus = initEventBus()
      bus.emit('module:registered', {
        moduleKey: manifest.module.meta.key,
        module: manifest
      })

      return true
    } catch (e) {
      console.error('Failed to register module:', e)
      return false
    }
  }

  /**
   * 注销模块
   */
  const unregisterModule = async (moduleKey: string): Promise<boolean> => {
    try {
      // 检查是否已安装
      const instance = moduleInstances.value.get(moduleKey)
      if (instance) {
        throw new Error(`Cannot unregister installed module ${moduleKey}`)
      }

      // 从注册表移除
      moduleRegistry.delete(moduleKey)

      // 触发注销事件
      const bus = initEventBus()
      bus.emit('module:unregistered', { moduleKey })

      return true
    } catch (e) {
      console.error('Failed to unregister module:', e)
      return false
    }
  }

  /**
   * 获取已注册模块
   */
  const getRegisteredModule = (moduleKey: string): ModuleManifest | null => {
    return moduleRegistry.get(moduleKey) || null
  }

  /**
   * 获取所有已注册模块
   */
  const getRegisteredModules = (): ModuleManifest[] => {
    return Array.from(moduleRegistry.values())
  }

  /**
   * 安装模块
   */
  const installModule = async (moduleKey: string, options?: {
    config?: Record<string, any>
    activate?: boolean
    path?: string
  }): Promise<ModuleInstance | null> => {
    try {
      // 获取模块定义
      const manifest = moduleRegistry.get(moduleKey)
      if (!manifest) {
        throw new Error(`Module ${moduleKey} not found in registry`)
      }

      // 检查是否已安装
      if (moduleInstances.value.has(moduleKey)) {
        console.warn(`Module ${moduleKey} already installed`)
        return moduleInstances.value.get(moduleKey)!
      }

      // 调用后端API安装
      const response = await api.post('/Module/install', {
        moduleKey,
        options: {
          activate: options?.activate ?? true,
          config: options?.config || {},
          path: options?.path
        }
      })

      if (!response.success) {
        throw new Error(response.message || 'Installation failed')
      }

      // 创建模块实例
      const instance: ModuleInstance = {
        definition: manifest.module,
        path: response.path || `/modules/${moduleKey}`,
        status: response.activated ? ModuleStatus.ACTIVE : ModuleStatus.INSTALLED,
        activatedAt: response.activated ? new Date() : undefined,
        config: response.config || {},
        error: response.error
      }

      // 保存实例
      moduleInstances.value.set(moduleKey, instance)
      moduleStatus.value.set(moduleKey, instance.status)

      // 触发安装事件
      const bus = initEventBus()
      bus.emit('module:installed', {
        moduleKey,
        instance
      })

      return instance
    } catch (e) {
      console.error('Failed to install module:', e)
      return null
    }
  }

  /**
   * 卸载模块
   */
  const uninstallModule = async (moduleKey: string): Promise<boolean> => {
    try {
      // 检查是否已安装
      const instance = moduleInstances.value.get(moduleKey)
      if (!instance) {
        console.warn(`Module ${moduleKey} not installed`)
        return false
      }

      // 检查是否是核心模块
      if (instance.definition.meta.isCore) {
        throw new Error('Cannot uninstall core module')
      }

      // 调用后端API卸载
      const response = await api.post(`/Module/${moduleKey}/uninstall`)

      if (!response.success) {
        throw new Error(response.message || 'Uninstallation failed')
      }

      // 从实例列表移除
      moduleInstances.value.delete(moduleKey)
      moduleStatus.value.delete(moduleKey)

      // 触发卸载事件
      const bus = initEventBus()
      bus.emit('module:uninstalled', { moduleKey })

      return true
    } catch (e) {
      console.error('Failed to uninstall module:', e)
      return false
    }
  }

  /**
   * 激活模块
   */
  const activateModule = async (moduleKey: string): Promise<boolean> => {
    try {
      // 检查模块是否已安装
      const instance = moduleInstances.value.get(moduleKey)
      if (!instance) {
        throw new Error(`Module ${moduleKey} not installed`)
      }

      // 检查模块是否已激活
      if (instance.status === ModuleStatus.ACTIVE) {
        console.warn(`Module ${moduleKey} already active`)
        return true
      }

      // 调用后端API激活
      const response = await api.post(`/Module/${moduleKey}/activate`)

      if (!response.success) {
        throw new Error(response.message || 'Activation failed')
      }

      // 更新状态
      instance.status = ModuleStatus.ACTIVE
      instance.activatedAt = new Date()
      moduleStatus.value.set(moduleKey, ModuleStatus.ACTIVE)

      // 触发激活事件
      const bus = initEventBus()
      bus.emit('module:activated', {
        moduleKey,
        instance
      })

      return true
    } catch (e) {
      console.error('Failed to activate module:', e)
      return false
    }
  }

  /**
   * 停用模块
   */
  const deactivateModule = async (moduleKey: string): Promise<boolean> => {
    try {
      // 检查模块是否已安装
      const instance = moduleInstances.value.get(moduleKey)
      if (!instance) {
        throw new Error(`Module ${moduleKey} not installed`)
      }

      // 检查模块是否已停用
      if (instance.status === ModuleStatus.INACTIVE) {
        console.warn(`Module ${moduleKey} already inactive`)
        return true
      }

      // 调用后端API停用
      const response = await api.post(`/Module/${moduleKey}/deactivate`)

      if (!response.success) {
        throw new Error(response.message || 'Deactivation failed')
      }

      // 更新状态
      instance.status = ModuleStatus.INACTIVE
      instance.activatedAt = undefined
      moduleStatus.value.set(moduleKey, ModuleStatus.INACTIVE)

      // 触发停用事件
      const bus = initEventBus()
      bus.emit('module:deactivated', {
        moduleKey,
        instance
      })

      return true
    } catch (e) {
      console.error('Failed to deactivate module:', e)
      return false
    }
  }

  /**
   * 获取模块实例
   */
  const getModuleInstance = (moduleKey: string): ModuleInstance | null => {
    return moduleInstances.value.get(moduleKey) || null
  }

  /**
   * 获取所有模块实例
   */
  const getModuleInstances = (): ModuleInstance[] => {
    return Array.from(moduleInstances.values())
  }

  /**
   * 按条件筛选模块
   */
  const filterModules = (filter?: ModuleFilter): ModuleInstance[] => {
    return getModuleInstances().filter(instance => {
      if (filter?.category && instance.definition.meta.category !== filter.category) {
        return false
      }

      if (filter?.author && instance.definition.meta.author !== filter.author) {
        return false
      }

      if (filter?.keyword) {
        const keyword = filter.keyword.toLowerCase()
        const name = instance.definition.meta.name.toLowerCase()
        const description = instance.definition.meta.description.toLowerCase()
        const tags = instance.definition.meta.tags.map(tag => tag.toLowerCase())

        if (!name.includes(keyword) &&
            !description.includes(keyword) &&
            !tags.some(tag => tag.includes(keyword))) {
          return false
        }
      }

      if (filter?.free !== undefined && instance.definition.meta.price !== filter.free) {
        return false
      }

      if (filter?.installed !== undefined) {
        if (filter.installed && !moduleInstances.value.has(instance.definition.meta.key)) {
          return false
        }
        if (!filter.installed && moduleInstances.value.has(instance.definition.meta.key)) {
          return false
        }
      }

      if (filter?.active !== undefined) {
        const isActive = moduleStatus.value.get(instance.definition.meta.key) === ModuleStatus.ACTIVE
        if (filter.active !== isActive) {
          return false
        }
      }

      return true
    })
  }

  /**
   * 更新模块配置
   */
  const updateModuleConfig = async (moduleKey: string, config: Record<string, any>): Promise<boolean> => {
    try {
      const instance = moduleInstances.value.get(moduleKey)
      if (!instance) {
        throw new Error(`Module ${moduleKey} not installed`)
      }

      // 调用后端API更新配置
      const response = await api.post(`/Module/${moduleKey}/config`, {
        config
      })

      if (!response.success) {
        throw new Error(response.message || 'Config update failed')
      }

      // 更新实例配置
      instance.config = { ...instance.config, ...config }

      // 触发配置更新事件
      const bus = initEventBus()
      bus.emit('module:configUpdated', {
        moduleKey,
        config
      })

      return true
    } catch (e) {
      console.error('Failed to update module config:', e)
      return false
    }
  }

  /**
   * 执行模块生命周期钩子
   */
  const executeLifecycleHook = async (moduleKey: string, hookName: 'onInstall' | 'onUninstall' | 'onActivate' | 'onDeactivate', ...args: any[]): Promise<any> => {
    try {
      const instance = moduleInstances.value.get(moduleKey)
      if (!instance) {
        throw new Error(`Module ${moduleKey} not installed`)
      }

      const hook = instance.definition.lifecycle?.[hookName]
      if (!hook) {
        return null
      }

      return await hook(...args)
    } catch (e) {
      console.error(`Failed to execute lifecycle hook ${hookName}:`, e)
      return null
    }
  }

  /**
   * 验证模块定义
   */
  const validateModuleDefinition = async (definition: ModuleDefinition): Promise<{ valid: boolean; errors: string[] }> => {
    const errors: string[] = []

    // 基础信息验证
    if (!definition.meta.key) {
      errors.push('Module key is required')
    }

    if (!definition.meta.name) {
      errors.push('Module name is required')
    }

    if (!definition.meta.version) {
      errors.push('Module version is required')
    }

    if (!definition.meta.author) {
      errors.push('Module author is required')
    }

    // 依赖验证
    if (definition.meta.dependencies) {
      for (const dep of definition.meta.dependencies) {
        const depModule = moduleRegistry.get(dep)
        if (!depModule) {
          errors.push(`Dependency ${dep} not found`)
        }
      }
    }

    return {
      valid: errors.length === 0,
      errors
    }
  }

  /**
   * 监听模块事件
   */
  const onModuleEvent = (event: string, handler: (event: any) => void): () => void => {
    const bus = initEventBus()
    bus.on(event, handler)

    // 返回取消监听函数
    return () => {
      bus.off(event, handler)
    }
  }

  /**
   * 获取模块状态
   */
  const getModuleStatus = (moduleKey: string): ModuleStatus | null => {
    return moduleStatus.value.get(moduleKey) || null
  }

  /**
   * 检查模块是否已安装
   */
  const isModuleInstalled = (moduleKey: string): boolean => {
    return moduleInstances.value.has(moduleKey)
  }

  /**
   * 检查模块是否已激活
   */
  const isModuleActive = (moduleKey: string): boolean => {
    return moduleStatus.value.get(moduleKey) === ModuleStatus.ACTIVE
  }

  return {
    // 注册表
    moduleRegistry: readonly(moduleRegistry),
    moduleInstances: readonly(moduleInstances),
    moduleStatus: readonly(moduleStatus),

    // 方法
    registerModule,
    unregisterModule,
    getRegisteredModule,
    getRegisteredModules,

    // 安装管理
    installModule,
    uninstallModule,
    activateModule,
    deactivateModule,

    // 查询
    getModuleInstance,
    getModuleInstances,
    filterModules,
    getModuleStatus,
    isModuleInstalled,
    isModuleActive,

    // 配置
    updateModuleConfig,

    // 生命周期
    executeLifecycleHook,

    // 验证
    validateModuleDefinition,

    // 事件
    onModuleEvent,

    // 工具
    initEventBus
  }
}

/**
 * 模块工具函数
 */
export const useModuleTools = () => {
  /**
   * 创建路由配置
   */
  const createRoute = (path: string, name?: string, component?: string): any => {
    return {
      path,
      name,
      component,
      meta: {
        title: name,
        order: 999
      }
    }
  }

  /**
   * 创建组件配置
   */
  const createComponent = (name: string, path: string): any => {
    return {
      name,
      path,
      global: true
    }
  }

  /**
   * 创建配置项
   */
  const createConfig = (key: string, value: any, options?: {
    type?: string
    description?: string
    required?: boolean
    defaultValue?: any
    validation?: any
  }): any => {
    return {
      configKey: key,
      configValue: value,
      configType: options?.type || 'string',
      description: options?.description,
      required: options?.required || false,
      defaultValue: options?.defaultValue,
      validation: options?.validation
    }
  }

  /**
   * 创建权限配置
   */
  const createPermission = (key: string, name: string, level: string): any => {
    return {
      key,
      name,
      description: `${name} permission`,
      level
    }
  }

  return {
    createRoute,
    createComponent,
    createConfig,
    createPermission
  }
}