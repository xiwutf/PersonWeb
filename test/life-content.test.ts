import { describe, expect, it, beforeEach, afterEach } from 'vitest'
import { existsSync, readFileSync, unlinkSync, writeFileSync } from 'node:fs'
import { resolve } from 'node:path'
import {
  isLifeNoteSlug,
  isSafeContentSlug,
  LIFE_RESERVED_MARKDOWN_SLUGS,
} from '../constants/life-content'
import {
  createLifeNote,
  parseYamlSafe,
  readLifeHome,
  readLifeMargin,
  readLifeMoments,
  readLifeNow,
  readMarkdownCollection,
  writeLifeMoments,
  writeLifeNow,
} from '../server/utils/content-files'

describe('Life content slug safety', () => {
  it('rejects path traversal and empty slugs', () => {
    expect(isSafeContentSlug('../blog/secret')).toBe(false)
    expect(isSafeContentSlug('foo/bar')).toBe(false)
    expect(isSafeContentSlug('..')).toBe(false)
    expect(isSafeContentSlug('')).toBe(false)
    expect(isSafeContentSlug('  hello')).toBe(false)
    expect(isSafeContentSlug('weekend-ride')).toBe(true)
  })

  it('never treats reserved page files as notes', () => {
    for (const slug of LIFE_RESERVED_MARKDOWN_SLUGS) {
      expect(isLifeNoteSlug(slug)).toBe(false)
    }
    expect(isLifeNoteSlug('profile')).toBe(false)
  })
})

describe('Life markdown collection', () => {
  it('excludes profile.md from the notes list', () => {
    const notes = readMarkdownCollection('life')
    expect(notes.some(item => item.slug === 'profile')).toBe(false)
    expect(notes.some(item => item.path === '/life/profile')).toBe(false)
    expect(notes.some(item => item._path === '/life/profile')).toBe(false)
  })
})

describe('Life home content', () => {
  it('keeps magazine sections content-driven', () => {
    const home = readLifeHome()
    expect(home.hero.name).toBe('溪午听风')
    expect(home.hero.lines.length).toBeGreaterThan(0)
    expect(home.sections.now.number).toBe('02')
    expect(home.sections.margin.number).toBe('05')
    expect(home.sections.about.number).toBe('06')
    expect(home.sections.about.title).toBe('关于我')
    expect(home.sections.margin.title).toBe('摘句')
    expect(home.closing).toBeTruthy()

    const now = readLifeNow()
    expect(now.items.length).toBeGreaterThanOrEqual(3)
    expect(now.items.every(item => item.title && item.description)).toBe(true)
  })

  it('loads margin quotes with optional notes', () => {
    const items = readLifeMargin()
    expect(items.length).toBeGreaterThan(10)
    expect(items.every(item => item.text)).toBe(true)
    expect(items.some(item => item.featured)).toBe(true)
    expect(items.some(item => item.note)).toBe(true)
    expect(items.some(item => item.group === '日常')).toBe(true)
  })
})

describe('Life YAML parsing', () => {
  it('returns null for invalid YAML instead of throwing', () => {
    expect(parseYamlSafe('not: [unterminated')).toBeNull()
    expect(parseYamlSafe('[\n- unterminated')).toBeNull()
  })

  it('sorts moments by date descending', () => {
    const moments = readLifeMoments()
    expect(Array.isArray(moments)).toBe(true)
    const dates = moments.map(item => item.date)
    const sorted = [...dates].sort((left, right) => new Date(right).getTime() - new Date(left).getTime())
    expect(dates).toEqual(sorted)
  })
})

const root = resolve(__dirname, '..')
const nowFile = resolve(root, 'content/life/now.yml')
const momentsFile = resolve(root, 'content/life/moments.yml')

describe('Life list content writes', () => {
  let nowBackup = ''
  let momentsBackup = ''
  const createdNotes: string[] = []

  beforeEach(() => {
    nowBackup = readFileSync(nowFile, 'utf8')
    momentsBackup = readFileSync(momentsFile, 'utf8')
  })

  afterEach(() => {
    writeFileSync(nowFile, nowBackup, 'utf8')
    writeFileSync(momentsFile, momentsBackup, 'utf8')
    for (const file of createdNotes.splice(0)) {
      if (existsSync(file)) unlinkSync(file)
    }
  })

  it('replaces now items and keeps title/description', () => {
    const saved = writeLifeNow([
      { category: '阅读', title: '阅读', description: '最近在看一本闲书。', icon: 'leaf' },
    ])
    expect(saved.items).toHaveLength(1)
    expect(saved.items[0].title).toBe('阅读')
    expect(readLifeNow().items[0].description).toContain('闲书')
  })

  it('drops now items missing description', () => {
    const saved = writeLifeNow([
      { category: '空', title: '空', description: '' },
      { category: '游泳', title: '游泳', description: '去游几圈。', icon: 'sneaker' },
    ])
    expect(saved.items.map(item => item.title)).toEqual(['游泳'])
  })

  it('writes moments sorted by date descending', () => {
    const saved = writeLifeMoments([
      { date: '2026-01-01', content: '较早的一条' },
      { date: '2026-09-09', content: '较新的一条' },
    ])
    expect(saved.map(item => item.content)).toEqual(['较新的一条', '较早的一条'])
    expect(readLifeMoments()[0].date).toBe('2026-09-09')
  })

  it('creates a life note markdown file', () => {
    const note = createLifeNote({
      title: '测试随笔',
      description: '摘要',
      content: '这是正文。',
      date: '2026-09-09',
    })
    expect(note?.title).toBe('测试随笔')
    expect(note?.slug).toBeTruthy()
    createdNotes.push(resolve(root, 'content/life', `${note!.slug}.md`))
    expect(existsSync(createdNotes[0])).toBe(true)
    expect(readFileSync(createdNotes[0], 'utf8')).toContain('这是正文。')
  })
})
