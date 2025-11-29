/**
 * 模块列表配置
 * 
 * 用途：
 * - 定义项目中所有支持独立主题的模块
 * - 用于后台 UI 展示模块列表
 * - 用于前端渲染模块主题时一致引用
 * 
 * 注意：
 * - 模块 ID 使用小写 + 下划线风格
 * - 后端 module_theme:{id} 中的 {id} 要与这里的 id 完全一致
 */

export interface ModuleDefinition {
  /** 模块唯一标识，如 "home_hero" */
  id: string
  /** 模块显示名称，用于后台 UI */
  name: string
  /** 模块描述（可选） */
  description?: string
}

/**
 * 模块列表配置
 * 后续可以继续追加新模块
 */
export const MODULE_DEFINITIONS: ModuleDefinition[] = [
  {
    id: 'home_hero',
    name: '首页头部 Hero 区域',
    description: '首页主视觉区域，包含个人介绍和主要导航'
  },
  {
    id: 'home_tools',
    name: '首页工具/功能区',
    description: '首页展示工具和功能模块的区域'
  },
  {
    id: 'ai_lab',
    name: 'AI 实验室模块',
    description: 'AI 实验室页面，包含 AI 相关功能和实验'
  },
  {
    id: 'blog_list',
    name: '博客列表模块',
    description: '博客文章列表展示区域'
  },
  {
    id: 'projects_list',
    name: '项目列表模块',
    description: '项目展示列表区域'
  }
]

/**
 * 根据模块 ID 获取模块定义
 */
export function getModuleDefinition(moduleId: string): ModuleDefinition | undefined {
  return MODULE_DEFINITIONS.find(m => m.id === moduleId)
}

/**
 * 获取所有模块 ID 列表
 */
export function getAllModuleIds(): string[] {
  return MODULE_DEFINITIONS.map(m => m.id)
}

