/**
 * 模块系统插件
 * 在客户端初始化模块系统
 */

export default defineNuxtPlugin(() => {
  // 初始化模块系统
  const { loadEnabledModules } = useModuleSystem()

  // 页面加载时加载启用的模块
  if (process.client) {
    loadEnabledModules()
  }
})