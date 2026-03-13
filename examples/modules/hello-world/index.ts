import { defineModule } from '@personweb/module-system'
import HelloWorld from './components/HelloWorld.vue'
import HelloCounter from './components/HelloCounter.vue'
import HelloGreeting from './components/HelloGreeting.vue'
import moduleConfig from './module.json'

/**
 * Hello World 示例模块
 *
 * 这个模块演示了以下概念：
 * - 模块元数据配置
 * - 路由定义
 * - 组件注册
 * - 模块配置
 * - 生命周期钩子
 */
export default defineModule({
  // 模块定义
  module: moduleConfig,

  // 依赖项
  dependencies: [],

  // 导出的组件和工具
  exports: {
    HelloWorld,
    HelloCounter,
    HelloGreeting,
    // 工具函数
    greet: createGreeting
  },

  // 生命周期钩子
  async init() {
    console.log('👋 Hello World 模块初始化中...')
    // 模块初始化逻辑
  },

  async onInstall() {
    console.log('👋 Hello World 模块已安装')
    // 安装逻辑
  },

  async onActivate() {
    console.log('👋 Hello World 模块已激活')
    // 激活逻辑
  },

  async onDeactivate() {
    console.log('👋 Hello World 模块已停用')
    // 停用逻辑
  },

  async onUninstall() {
    console.log('👋 Hello World 模块已卸载')
    // 卸载逻辑
  }
})

/**
 * 创建问候语
 * @param name 名字
 * @param greeting 问候语
 * @returns 问候字符串
 */
function createGreeting(name: string = 'World', greeting: string = 'Hello'): string {
  return `${greeting}, ${name}!`
}

/**
 * 格式化日期
 * @param date 日期字符串或 Date 对象
 * @returns 格式化后的日期字符串
 */
function formatDate(date: string | Date): string {
  const d = new Date(date)
  return d.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}