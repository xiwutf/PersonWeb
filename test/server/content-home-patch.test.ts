import { readFileSync, writeFileSync } from 'node:fs'
import { resolve } from 'node:path'
import { afterEach, beforeEach, describe, expect, it } from 'vitest'
import { readLifeHome, readWorkHome } from '../../server/utils/content-files'
import { patchLifeHomeField, patchWorkHomeField, replaceLifeHomeLines } from '../../server/utils/content-home-patch'

const lifeHome = resolve(__dirname, '../../content/life/home.yml')
const workHome = resolve(__dirname, '../../content/work/home.yml')

let lifeBackup = ''
let workBackup = ''

describe('content home patch helpers', () => {
  beforeEach(() => {
    lifeBackup = readFileSync(lifeHome, 'utf8')
    workBackup = readFileSync(workHome, 'utf8')
  })

  afterEach(() => {
    writeFileSync(lifeHome, lifeBackup, 'utf8')
    writeFileSync(workHome, workBackup, 'utf8')
  })

  it('patches life hero.name and returns reader shape', () => {
    const result = patchLifeHomeField('hero.name', '测试名')
    expect(result.hero.name).toBe('测试名')
    expect(readLifeHome().hero.name).toBe('测试名')
  })

  it('replaces hero.lines as a whole list', () => {
    const result = replaceLifeHomeLines(['第一句', '第二句'])
    expect(result.hero.lines).toEqual(['第一句', '第二句'])
  })

  it('rejects illegal life path', () => {
    expect(() => patchLifeHomeField('hero', 'x')).toThrow(/not allowed/i)
  })

  it('patches work brand.tagline via API path', () => {
    const result = patchWorkHomeField('brand.tagline', '测试标语')
    expect(result.brand.tagline).toBe('测试标语')
    expect(readWorkHome().brand.tagline).toBe('测试标语')
  })

  it('maps hero.englishName to YAML english_name', () => {
    const result = patchWorkHomeField('hero.englishName', 'TestEn')
    expect(result.hero.englishName).toBe('TestEn')
    expect(readFileSync(workHome, 'utf8')).toContain('english_name: TestEn')
  })

  it('route sources call checkAuth', async () => {
    const lifeSrc = readFileSync(
      resolve(__dirname, '../../server/api/content/life/home.patch.ts'),
      'utf8',
    )
    const workSrc = readFileSync(
      resolve(__dirname, '../../server/api/content/work/home.patch.ts'),
      'utf8',
    )
    expect(lifeSrc).toContain('checkAuth(event)')
    const lifeLinesSrc = readFileSync(
      resolve(__dirname, '../../server/api/content/life/home/lines.put.ts'),
      'utf8',
    )
    const lifeNowSrc = readFileSync(
      resolve(__dirname, '../../server/api/content/life/now.put.ts'),
      'utf8',
    )
    const lifeMomentsSrc = readFileSync(
      resolve(__dirname, '../../server/api/content/life/moments.put.ts'),
      'utf8',
    )
    const lifeNotesSrc = readFileSync(
      resolve(__dirname, '../../server/api/content/life/notes.post.ts'),
      'utf8',
    )
    expect(lifeLinesSrc).toContain('checkAuth(event)')
    expect(lifeNowSrc).toContain('checkAuth(event)')
    expect(lifeMomentsSrc).toContain('checkAuth(event)')
    expect(lifeNotesSrc).toContain('checkAuth(event)')
    expect(workSrc).toContain('checkAuth(event)')
  })
})
