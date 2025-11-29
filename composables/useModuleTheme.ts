/**
 * 模块主题管理组合式函数
 * 
 * 职责：
 * - 管理各个模块的独立主题配置
 * - 提供模块主题的读取和设置能力
 * - 支持模块主题跟随全局主题（null 表示跟随全局）
 * 
 * 工作原理：
 * - 模块主题存储在全局状态中，由插件在启动时从后端加载
 * - 模块组件通过 useModuleTheme(moduleId) 获取当前模块的主题
 * - 如果模块主题为 null，表示跟随全局主题（使用 <html data-theme> 的变量）
 * - 如果模块主题不为 null，会在模块根元素上设置 data-module-theme 属性，CSS 层可以基于此做局部覆盖
 */

/**
 * 设置模块主题配置
 * 由插件在启动时调用，将从后端获取的模块主题配置保存到全局状态
 * 
 * @param moduleThemesMap 模块主题映射，格式为 { moduleId: themeKey | null }
 */
export function setModuleThemes(moduleThemesMap: Record<string, string | null>) {
  const state = useState<Record<string, string | null>>('module-themes', () => ({}))
  state.value = { ...moduleThemesMap }
}

/**
 * 使用模块主题
 * 在模块组件中调用，获取当前模块的主题配置
 * 
 * @param moduleId 模块唯一标识，例如 "home_hero", "ai_lab"
 * @returns 返回模块主题的响应式引用
 */
export function useModuleTheme(moduleId: string) {
  // 使用全局状态存储模块主题配置
  // 格式：{ moduleId: themeKey | null }
  // null 表示跟随全局主题
  const moduleThemes = useState<Record<string, string | null>>('module-themes', () => ({}))

  // 计算当前模块的主题
  // 如果模块没有配置主题（或为 null），返回 null 表示跟随全局
  const moduleTheme = computed(() => {
    return moduleThemes.value[moduleId] ?? null
  })

  return {
    /**
     * 当前模块的主题 key
     * - null: 表示跟随全局主题（使用 <html data-theme> 的变量）
     * - 非 null: 表示使用独立主题，会在模块根元素上设置 data-module-theme 属性
     */
    moduleTheme: readonly(moduleTheme)
  }
}

