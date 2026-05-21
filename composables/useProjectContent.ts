import { useMarkdown } from '~/composables/useMarkdown'

/** 粗略判断字符串是否已是 HTML 片段 */
export function looksLikeHtml(text: string): boolean {
  const trimmed = text.trim()
  if (!trimmed) return false
  return /^<[a-z][\s\S]*>/i.test(trimmed)
}

/** 移除脚本与内联事件，降低 v-html XSS 风险 */
export function sanitizeProjectHtml(html: string): string {
  return html
    .replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, '')
    .replace(/\son\w+\s*=\s*("[^"]*"|'[^']*'|[^\s>]+)/gi, '')
}

export type ProjectBodySource = {
  contentHtml?: string | null
  content?: string | null
}

/**
 * 解析项目正文为可安全展示的 HTML。
 * 优先级：contentHtml → content（若为 HTML）→ content（Markdown 转 HTML）
 */
export function resolveProjectBodyHtml(source: ProjectBodySource): string {
  const html = source.contentHtml?.trim()
  if (html) {
    return sanitizeProjectHtml(html)
  }

  const raw = source.content?.trim()
  if (!raw) return ''

  if (looksLikeHtml(raw)) {
    return sanitizeProjectHtml(raw)
  }

  const { parse } = useMarkdown()
  return parse(raw)
}

export const useProjectContent = () => ({
  looksLikeHtml,
  sanitizeProjectHtml,
  resolveProjectBodyHtml
})
