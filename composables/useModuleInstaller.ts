/**
 * 模块安装器 Composable
 * 处理模块的下载、安装、激活等流程
 */

import type { ModuleManifest, ModuleDefinition, ModuleInstance } from '~/types/module'
import { useModuleCore } from './useModuleCore'
import { useModuleManager } from './useModuleManager'

/**
 * 模块安装器
 */
export const useModuleInstaller = () => {
  const api = useApi()
  const {
    moduleRegistry,
    moduleInstances,
    moduleStatus,
    installModule,
    activateModule,
    unregisterModule
  } = useModuleCore()

  const { checkModuleDependencies } = useModuleManager()

  /**
   * 安装流程状态
   */
  const installStatus = ref<{
    moduleKey: string
    status: 'idle' | 'downloading' | 'validating' | 'installing' | 'activating' | 'completed' | 'error'
    progress: number
    message: string
    error?: string
  }>({
    moduleKey: '',
    status: 'idle',
    progress: 0,
    message: ''
  })

  /**
   * 开始安装模块
   */
  const startInstallation = async (moduleKey: string, options?: {
    config?: Record<string, any>
    activate?: boolean
    overwrite?: boolean
  }): Promise<boolean> => {
    try {
      // 设置安装状态
      installStatus.value = {
        moduleKey,
        status: 'downloading',
        progress: 0,
        message: '正在下载模块...'
      }

      // 1. 下载模块
      installStatus.value.status = 'downloading'
      installStatus.value.progress = 10
      installStatus.value.message = '正在下载模块...'

      const { downloadUrl, manifest } = await downloadModule(moduleKey)

      // 2. 验证模块
      installStatus.value.status = 'validating'
      installStatus.value.progress = 30
      installStatus.value.message = '正在验证模块...'

      const validation = await validateModuleManifest(manifest)
      if (!validation.valid) {
        throw new Error(`模块验证失败: ${validation.errors.join(', ')}`)
      }

      // 3. 检查依赖
      installStatus.value.status = 'validating'
      installStatus.value.progress = 50
      installStatus.value.message = '正在检查依赖...'

      const deps = checkModuleDependencies(moduleKey)
      if (!deps.valid) {
        if (deps.missing.length > 0) {
          throw new Error(`缺少依赖: ${deps.missing.join(', ')}`)
        }
      }

      // 4. 注册模块
      installStatus.value.status = 'installing'
      installStatus.value.progress = 70
      installStatus.value.message = '正在注册模块...'

      const registered = await registerModule(manifest)
      if (!registered) {
        throw new Error('模块注册失败')
      }

      // 5. 安装模块
      installStatus.value.status = 'installing'
      installStatus.value.progress = 90
      installStatus.value.message = '正在安装模块...'

      const installed = await installModule(moduleKey, {
        config: options?.config || {},
        activate: options?.activate ?? true,
        path: `/modules/${moduleKey}`
      })

      if (!installed) {
        throw new Error('模块安装失败')
      }

      // 6. 激活模块
      if (options?.activate) {
        installStatus.value.status = 'activating'
        installStatus.value.progress = 95
        installStatus.value.message = '正在激活模块...'

        await activateModule(moduleKey)
      }

      // 完成
      installStatus.value.status = 'completed'
      installStatus.value.progress = 100
      installStatus.value.message = '模块安装完成'

      return true
    } catch (e) {
      installStatus.value.status = 'error'
      installStatus.value.error = e instanceof Error ? e.message : 'Unknown error'
      console.error('Installation failed:', e)
      return false
    }
  }

  /**
   * 下载模块
   */
  const downloadModule = async (moduleKey: string, version?: string): Promise<{
    downloadUrl: string
    manifest: ModuleManifest
  }> => {
    try {
      const params = version ? { version } : {}
      const response = await api.post(`/ModuleStore/${moduleKey}/download`, {
        params
      })

      if (response.success) {
        return {
          downloadUrl: response.data.downloadUrl,
          manifest: response.data.manifest
        }
      } else {
        throw new Error(response.message || 'Download failed')
      }
    } catch (e) {
      console.error('Download failed:', e)
      throw e
    }
  }

  /**
   * 验证模块清单
   */
  const validateModuleManifest = async (manifest: ModuleManifest): Promise<{
    valid: boolean
    errors: string[]
  }> => {
    const errors: string[] = []

    // 验证基础信息
    if (!manifest.module.meta.key) {
      errors.push('缺少模块标识')
    }

    if (!manifest.module.meta.name) {
      errors.push('缺少模块名称')
    }

    if (!manifest.module.meta.version) {
      errors.push('缺少版本号')
    }

    // 验证路由
    if (manifest.module.routes) {
      for (const route of manifest.module.routes) {
        if (!route.path) {
          errors.push('路由缺少path')
        }
      }
    }

    // 验证组件
    if (manifest.module.components) {
      for (const component of manifest.module.components) {
        if (!component.name || !component.path) {
          errors.push('组件缺少name或path')
        }
      }
    }

    return {
      valid: errors.length === 0,
      errors
    }
  }

  /**
   * 取消安装
   */
  const cancelInstallation = async (): Promise<boolean> => {
    if (installStatus.value.status === 'idle') {
      return true
    }

    try {
      const moduleKey = installStatus.value.moduleKey

      // 根据当前状态执行取消操作
      if (installStatus.value.status === 'downloading' || installStatus.value.status === 'validating') {
        // 清理临时文件
        await cleanupTempFiles(moduleKey)
      } else if (installStatus.value.status === 'installing' || installStatus.value.status === 'activating') {
        // 卸载已安装的部分
        await uninstallModule(moduleKey)
      }

      // 注销模块
      await unregisterModule(moduleKey)

      // 重置状态
      installStatus.value = {
        moduleKey: '',
        status: 'idle',
        progress: 0,
        message: ''
      }

      return true
    } catch (e) {
      console.error('Failed to cancel installation:', e)
      return false
    }
  }

  /**
   * 清理临时文件
   */
  const cleanupTempFiles = async (moduleKey: string): Promise<void> => {
    try {
      await api.post(`/Module/${moduleKey}/cleanup`)
    } catch (e) {
      console.error('Failed to cleanup temp files:', e)
    }
  }

  /**
   * 获取安装状态
   */
  const getInstallStatus = (): typeof installStatus.value => {
    return { ...installStatus.value }
  }

  /**
   * 监听安装进度
   */
  const onInstallProgress = (callback: (progress: number, message: string) => void): () => void => {
    // 这里应该使用WebSocket或Server-Sent Events
    // 模拟实现
    const interval = setInterval(() => {
      if (installStatus.value.status !== 'idle' && installStatus.value.status !== 'completed') {
        callback(installStatus.value.progress, installStatus.value.message)
      }
    }, 1000)

    return () => clearInterval(interval)
  }

  /**
   * 批量安装模块
   */
  const installMultipleModules = async (moduleKeys: string[], options?: {
    config?: Record<string, Record<string, any>>
    activate?: boolean
    parallel?: number
  }): Promise<{
    success: string[]
    failed: Array<{
      moduleKey: string
      error: string
      progress: number
    }>
  }> => {
    const success: string[] = []
    const failed: Array<{
      moduleKey: string
      error: string
      progress: number
    }> = []

    // 控制并行数量
    const maxParallel = options?.parallel || 2
    const queue = [...moduleKeys]
    const processing = new Set<string>()

    const processQueue = async () => {
      while (queue.length > 0 && processing.size < maxParallel) {
        const moduleKey = queue.shift()!
        processing.add(moduleKey)

        try {
          await startInstallation(moduleKey, {
            config: options?.config?.[moduleKey],
            activate: options?.activate
          })
          success.push(moduleKey)
        } catch (e) {
          failed.push({
            moduleKey,
            error: e instanceof Error ? e.message : 'Unknown error',
            progress: installStatus.value.progress
          })
        } finally {
          processing.delete(moduleKey)
        }
      }
    }

    // 开始处理队列
    await processQueue()

    // 等待所有任务完成
    while (processing.size > 0) {
      await new Promise(resolve => setTimeout(resolve, 100))
    }

    return { success, failed }
  }

  /**
   * 预安装检查
   */
  const preInstallCheck = async (moduleKey: string): Promise<{
    valid: boolean
    warnings: string[]
    errors: string[]
  }> => {
    const warnings: string[] = []
    const errors: string[] = []

    try {
      // 1. 检查是否已安装
      if (moduleInstances.value.has(moduleKey)) {
        const instance = moduleInstances.value.get(moduleKey)!
        if (instance.status === 'active') {
          warnings.push('模块已安装并激活，将重新安装')
        } else {
          warnings.push('模块已安装但未激活，将重新安装')
        }
      }

      // 2. 检查依赖
      const deps = checkModuleDependencies(moduleKey)
      if (!deps.valid) {
        if (deps.missing.length > 0) {
          errors.push(`缺少依赖: ${deps.missing.join(', ')}`)
        }
        if (deps.conflicts.length > 0) {
          warnings.push(`存在版本冲突: ${deps.conflicts.map(c => `${c.module}(${c.version} vs ${c.required})`).join(', ')}`)
        }
      }

      // 3. 检查权限
      // TODO: 检查模块权限

      // 4. 检查兼容性
      const manifest = await getModuleDetail(moduleKey)
      if (manifest) {
        const compatibility = manifest.compatibility
        if (compatibility) {
          // TODO: 检查Nuxt版本、Node版本等
        }
      }

      return {
        valid: errors.length === 0,
        warnings,
        errors
      }
    } catch (e) {
      errors.push(`预检查失败: ${e instanceof Error ? e.message : 'Unknown error'}`)
      return {
        valid: false,
        warnings,
        errors
      }
    }
  }

  return {
    // 状态
    installStatus: readonly(installStatus),

    // 安装流程
    startInstallation,
    cancelInstallation,
    getInstallStatus,

    // 工具
    downloadModule,
    validateModuleManifest,
    preInstallCheck,

    // 批量操作
    installMultipleModules,

    // 监听
    onInstallProgress
  }
}