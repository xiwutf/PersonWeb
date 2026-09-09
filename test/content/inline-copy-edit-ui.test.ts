import { describe, expect, it } from 'vitest'
import { readFileSync } from 'node:fs'
import { resolve } from 'node:path'

const root = resolve(__dirname, '../..')

describe('inline copy edit UI guards', () => {
  it('life and work pages mount AdminEditBanner and InlineEditableText', () => {
    const life = readFileSync(resolve(root, 'pages/life/index.vue'), 'utf8')
    const work = readFileSync(resolve(root, 'pages/work/index.vue'), 'utf8')
    expect(life).toContain('AdminEditBanner')
    expect(life).toContain('InlineEditableText')
    expect(life).toContain('LifeAdminAddButton')
    expect(life).toContain('LifeComposer')
    expect(work).toContain('AdminEditBanner')
    expect(work).toContain('InlineEditableText')
  })

  it('banner only renders when isAdmin', () => {
    const src = readFileSync(resolve(root, 'components/content/AdminEditBanner.vue'), 'utf8')
    expect(src).toContain('v-if="isAdmin"')
    expect(src).toContain('已登录 · 点击文案可编辑 · 列表可新增 / 删除')
  })

  it('editable chrome class only applied for admin session', () => {
    const src = readFileSync(resolve(root, 'components/content/InlineEditableText.vue'), 'utf8')
    expect(src).toContain("if (isAdmin.value) classes.push('inline-edit--editable')")
    expect(src).toContain('useAdminSession')
  })
})
