export type LibraryBook = { id: number; title: string; author: string; contentVersion: number; lastReadAt: string | null; progression: number | null }
export type LibraryResponse<T> = { code: number; message: string; data: T }
export type TocItem = { label: string; href: string; subitems?: TocItem[] }
export function flattenToc(items: TocItem[], depth = 0): { label: string; href: string; depth: number }[] {
  return items.flatMap(item => [{ label: item.label.trim(), href: item.href, depth }, ...flattenToc(item.subitems || [], depth + 1)])
}
export function swipeDirection(dx: number, dy: number): 'next' | 'prev' | null {
  return Math.abs(dx) >= 60 && Math.abs(dx) > Math.abs(dy) * 1.5 ? (dx < 0 ? 'next' : 'prev') : null
}

/** Reader footer: chapter page + whole-book percent when known. */
export function readerFooterLabel(chapter: string, bookPercent: number | null): string {
  const label = chapter || '阅读中'
  if (bookPercent == null || !Number.isFinite(bookPercent)) return label
  return `${label} · 全书 ${Math.round(Math.max(0, Math.min(1, bookPercent)) * 100)}%`
}
export function libraryError(cause: unknown, kind: 'epub' | 'mobi' | 'auto' = 'auto'): string {
  const error = cause as {
    response?: { status?: number; _data?: { message?: string; error?: string } }
    data?: { message?: string; error?: string }
    statusCode?: number
  }
  const payload = error.data ?? error.response?._data
  const code = payload?.error
  const status = error.response?.status ?? error.statusCode
  if (code === 'converter_unavailable' || code === 'conversion_config') return '服务端尚未配置可用的 Calibre 转换工具。请安装 Calibre，或在 ReadingLibrary:CalibreExecutable 指定 ebook-convert。EPUB 仍可直接上传。'
  if (code === 'conversion_busy') return '另一本 MOBI 正在转换，请稍后重试。'
  if (code === 'conversion_timeout') return 'MOBI 转换超时，请尝试较小的文件。'
  if (code === 'conversion_limit') return 'MOBI 转换超过资源或输出大小限制，请尝试较小的文件。'
  if (code === 'conversion_failed' || code === 'invalid_mobi') return '这本 MOBI 无法转换，文件可能损坏或有 DRM 加密。'
  if (code === 'conversion_rejected' || (status === 422 && kind === 'mobi')) {
    return 'MOBI 已转换，但生成的 EPUB 未通过安全校验（常见于嵌入字体、复杂样式或 SVG）。可用 Calibre 导出更简单的 EPUB 后再上传，或直接上传 EPUB。'
  }
  if (status === 401) return '阅读授权已过期，请重新输入密码。'
  if (status === 413) return '文件超过 100 MiB，或超过服务器上传限制，请选择较小的文件。'
  if (status === 422) return '这本 EPUB 未通过格式或安全校验。当前支持无 DRM、可重排的 EPUB 2/3；复杂样式、嵌入字体及通用 SVG 暂不支持。'
  if (status === 404) return '书籍不存在或文件不可用，请返回书架。'
  return '加载失败，请检查连接后重试。'
}

// Runs before iframe rendering. Blob resources from the validated archive remain usable.
export function protectEpubDocument(document: Document) {
  const meta = document.createElement('meta')
  meta.setAttribute('http-equiv', 'Content-Security-Policy')
  meta.setAttribute('content', "default-src 'none'; img-src blob: data:; style-src 'unsafe-inline' blob:; font-src 'none'; connect-src 'none'; script-src 'none'; object-src 'none'; base-uri 'none'; form-action 'none'")
  document.head.prepend(meta)
}
