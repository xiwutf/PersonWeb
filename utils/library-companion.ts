export type CompanionTurn = { role: 'user' | 'assistant'; content: string }
export type ReadingNote = {
  id: number
  bookId: number
  contentVersion: number
  kind: string
  cfiRange: string
  chapterHref: string
  quote: string
  note: string
  color: string
  requestId: string
  revision: number
  createdAt: string
  updatedAt: string
}

export type ReadingNoteFeedItem = {
  id: number
  bookId: number
  bookTitle: string
  bookAuthor: string
  contentVersion: number
  kind: string
  cfiRange: string
  chapterHref: string
  quote: string
  note: string
  createdAt: string
  updatedAt: string
}

export type ReadingNotesByBook = {
  bookId: number
  bookTitle: string
  bookAuthor: string
  notes: ReadingNoteFeedItem[]
}

/** epub.js highlight class for persisted private notes. */
export const NOTE_MARK_CLASS = 'library-note-mark'

export function noteMarkStyles(color = 'yellow'): Record<string, string> {
  const fill = color === 'green' ? 'rgba(134, 239, 172, 0.45)'
    : color === 'blue' ? 'rgba(147, 197, 253, 0.45)'
      : color === 'pink' ? 'rgba(249, 168, 212, 0.45)'
        : 'rgba(253, 224, 71, 0.45)'
  return { fill, 'fill-opacity': '1', 'mix-blend-mode': 'multiply' }
}

export function noteMarkCss(): string {
  return `.${NOTE_MARK_CLASS},g.${NOTE_MARK_CLASS}{cursor:pointer}g.${NOTE_MARK_CLASS}:hover rect{opacity:.85}`
}

export function groupNotesByBook(items: ReadingNoteFeedItem[]): ReadingNotesByBook[] {
  const groups = new Map<number, ReadingNotesByBook>()
  for (const item of items) {
    let group = groups.get(item.bookId)
    if (!group) {
      group = {
        bookId: item.bookId,
        bookTitle: item.bookTitle || '未命名书籍',
        bookAuthor: item.bookAuthor || '',
        notes: [],
      }
      groups.set(item.bookId, group)
    }
    group.notes.push(item)
  }
  return [...groups.values()].sort((left, right) => {
    const leftTime = Date.parse(left.notes[0]?.createdAt || '') || 0
    const rightTime = Date.parse(right.notes[0]?.createdAt || '') || 0
    return rightTime - leftTime
  })
}

export function selectionQuote(text: string, max = 4000): string {
  return text.replace(/\s+/g, ' ').trim().slice(0, max)
}

/** Clip surrounding text around a quote for AI explain (notes still store quote only). */
export function clipSurroundingContext(fullText: string, quote: string, max = 800): string {
  const normalized = selectionQuote(fullText, 20000)
  const selected = selectionQuote(quote, 4000)
  if (!selected) return ''
  if (!normalized) return selected.slice(0, max)
  if (normalized.length <= max) return normalized
  const index = normalized.indexOf(selected)
  if (index < 0) return normalized.slice(0, max)
  const pad = Math.max(0, Math.floor((max - selected.length) / 2))
  const start = Math.max(0, index - pad)
  const end = Math.min(normalized.length, index + selected.length + pad)
  let slice = normalized.slice(start, end)
  if (start > 0) slice = `…${slice}`
  if (end < normalized.length) slice = `${slice}…`
  return slice
}

/** Prefer the nearest block around the browser selection; fall back to padded window. */
export function selectionSurroundingContext(selection: Selection | null | undefined, max = 800): string {
  if (!selection || selection.rangeCount === 0) return ''
  const quote = selectionQuote(selection.toString())
  if (!quote) return ''
  const range = selection.getRangeAt(0)
  let node: Node | null = range.commonAncestorContainer
  if (node.nodeType === Node.TEXT_NODE) node = node.parentElement
  const element = node as Element | null
  const block = element?.closest?.(
    'p, li, blockquote, h1, h2, h3, h4, h5, h6, td, th, section, article, figcaption',
  ) as Element | null
  if (block?.textContent) {
    const blockText = selectionQuote(block.textContent, Math.max(max, quote.length + 80))
    if (blockText.includes(quote) && blockText.length > quote.length)
      return clipSurroundingContext(blockText, quote, max)
  }
  const root = element?.closest?.('body') || element?.ownerDocument?.body
  return clipSurroundingContext(root?.textContent || quote, quote, max)
}

export function companionError(cause: unknown): string {
  const error = cause as { response?: { status?: number }; statusCode?: number; data?: { error?: string; message?: string } }
  const code = error.data?.error
  const status = error.response?.status ?? error.statusCode
  if (code === 'ai_unavailable' || status === 502) return 'AI 暂时不可用，阅读与笔记不受影响，请稍后重试。'
  if (code === 'invalid_annotation') return '笔记内容或位置无效，请重新选中原文后再保存。'
  if (code === 'content_version_mismatch') return '书籍版本已变化，请重新打开后再保存笔记。'
  if (status === 401) return '阅读授权已过期，请重新输入密码。'
  return error.data?.message || '操作失败，请稍后重试。'
}

export function jumpFailureMessage(): string {
  return '无法跳转到这条笔记的原文位置，可能因排版变化失效。笔记内容仍可查看。'
}

export function buildExplainHistory(turns: CompanionTurn[]): CompanionTurn[] {
  return turns.slice(-10).map(turn => ({ role: turn.role, content: turn.content.slice(0, 8000) }))
}

export function notePreview(text: string, max = 72): string {
  const value = text.replace(/\s+/g, ' ').trim()
  return value.length <= max ? value : `${value.slice(0, max)}…`
}

/** List/card titles: drop trailing marketing parentheses, then clamp length. */
export function displayBookTitle(title: string, max = 28): string {
  const raw = (title || '').replace(/\s+/g, ' ').trim() || '未命名书籍'
  const trimmed = raw.replace(/(?:\s*[（(][^（）()]*[）)])+$/u, '').trim()
  return notePreview(trimmed || raw, max)
}
