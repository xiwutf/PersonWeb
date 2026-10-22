import { describe, expect, it } from 'vitest'
import {
  buildExplainHistory,
  companionError,
  displayBookTitle,
  groupNotesByBook,
  jumpFailureMessage,
  NOTE_MARK_CLASS,
  noteMarkCss,
  noteMarkStyles,
  notePreview,
  selectionQuote,
  clipSurroundingContext,
  type ReadingNoteFeedItem,
} from '../../utils/library-companion'

describe('library companion helpers', () => {
  it('normalizes selected quote length', () => {
    expect(selectionQuote('  hello\n\nworld  ')).toBe('hello world')
    expect(selectionQuote('x'.repeat(5000)).length).toBe(4000)
  })

  it('clips surrounding context around a short quote for AI explain', () => {
    const page = '前言。注意力 > 时间 > 金钱。注意力才是你所拥有的最宝贵的财富。后记。'
    expect(clipSurroundingContext(page, '注意力', 40)).toContain('注意力')
    expect(clipSurroundingContext(page, '注意力', 40)).toContain('时间')
    expect(clipSurroundingContext('短文', '短文', 800)).toBe('短文')
  })

  it('keeps recent conversation turns for follow-up', () => {
    const turns = Array.from({ length: 12 }, (_, index) => ({
      role: (index % 2 === 0 ? 'user' : 'assistant') as 'user' | 'assistant',
      content: `m${index}`,
    }))
    expect(buildExplainHistory(turns)).toHaveLength(10)
    expect(buildExplainHistory(turns)[0].content).toBe('m2')
  })

  it('explains AI and note failures without crashing reading', () => {
    expect(companionError({ data: { error: 'ai_unavailable' }, response: { status: 502 } })).toContain('阅读与笔记不受影响')
    expect(companionError({ data: { error: 'invalid_annotation' } })).toContain('重新选中')
    expect(jumpFailureMessage()).toContain('无法跳转')
    expect(notePreview('一段很长的用户笔记内容用来预览截断处理', 8)).toBe('一段很长的用户笔…')
  })

  it('styles persisted note highlights for the reader body', () => {
    expect(NOTE_MARK_CLASS).toBe('library-note-mark')
    expect(noteMarkCss()).toContain(`.${NOTE_MARK_CLASS}`)
    expect(noteMarkStyles('yellow').fill).toContain('253, 224, 71')
    expect(noteMarkStyles('blue').fill).toContain('147, 197, 253')
  })

  it('shortens marketing-heavy book titles for list display', () => {
    expect(displayBookTitle(
      '穷查理宝典：查理·芒格智慧箴言录 (全新增订本) (价值投资圣经，人生智慧宝典。全新升级版发布！)',
      40,
    )).toBe('穷查理宝典：查理·芒格智慧箴言录')
    expect(displayBookTitle('短书名')).toBe('短书名')
  })

  it('groups feed notes by book while keeping newest books first', () => {
    const feed: ReadingNoteFeedItem[] = [
      { id: 2, bookId: 2, bookTitle: 'B', bookAuthor: '', contentVersion: 1, kind: 'note', cfiRange: 'epubcfi(/6/2!/4)', chapterHref: '', quote: 'q2', note: 'n2', createdAt: '2026-10-14T10:00:00Z', updatedAt: '2026-10-14T10:00:00Z' },
      { id: 1, bookId: 1, bookTitle: 'A', bookAuthor: '作者', contentVersion: 1, kind: 'note', cfiRange: 'epubcfi(/6/2!/4)', chapterHref: '', quote: 'q1', note: 'n1', createdAt: '2026-10-13T10:00:00Z', updatedAt: '2026-10-13T10:00:00Z' },
      { id: 3, bookId: 2, bookTitle: 'B', bookAuthor: '', contentVersion: 1, kind: 'note', cfiRange: 'epubcfi(/6/2!/4)', chapterHref: '', quote: 'q3', note: 'n3', createdAt: '2026-10-12T10:00:00Z', updatedAt: '2026-10-12T10:00:00Z' },
    ]
    const groups = groupNotesByBook(feed)
    expect(groups.map(group => group.bookId)).toEqual([2, 1])
    expect(groups[0].notes.map(note => note.id)).toEqual([2, 3])
  })
})
