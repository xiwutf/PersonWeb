/**
 * 后台 Naive UI Provider 显式再导出（仅客户端）。
 *
 * 生产构建若使用动态 import('naive-ui') 再按属性取值，Rollup 可能过度 tree-shake，
 * 导致 NMessageProvider 等为 undefined → 主内容 vnode 无效、页面空白且 Network 无 404。
 * 通过本文件的静态 export 绑定，保证上述符号进入 chunk。
 */
export {
  NConfigProvider,
  NMessageProvider,
  NDialogProvider,
  NNotificationProvider,
  darkTheme
} from 'naive-ui'
