import { existsSync, readFileSync } from 'node:fs'
import { relative, resolve } from 'node:path'
import { describe, expect, it } from 'vitest'
import { adminMenu, adminMenuPaths } from '../../constants/admin/menu'

const root = resolve(__dirname, '../..')

const readSrc = (relativePath: string) =>
  readFileSync(resolve(root, relativePath), 'utf8')

const adminPagesRoot = resolve(root, 'pages/admin')

const collectVueFiles = (dir: string): string[] => {
  if (!existsSync(dir)) return []
  const entries = require('node:fs').readdirSync(dir, { withFileTypes: true })
  return entries.flatMap((entry: { isDirectory: () => boolean, name: string }) => {
    const full = resolve(dir, entry.name)
    if (entry.isDirectory()) return collectVueFiles(full)
    if (entry.name.endsWith('.vue')) return [full]
    return []
  })
}

describe('Admin operations console guards (Phase 3)', () => {
  it('does not ship legacy CMS edit routes', () => {
    expect(existsSync(resolve(adminPagesRoot, 'articles/edit/index.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'articles/edit/[id].vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'projects/edit/[[id]].vue'))).toBe(false)
  })

  it('does not ship Nitro plugin module CMS pages', () => {
    expect(existsSync(resolve(adminPagesRoot, 'modules/index.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'modules/upload.vue'))).toBe(false)
  })

  it('does not ship markdown CMS editor components', () => {
    expect(existsSync(resolve(root, 'components/admin/MarkdownEditor.vue'))).toBe(false)
    expect(existsSync(resolve(root, 'components/admin/SimpleMarkdownEditor.vue'))).toBe(false)
  })

  it('does not ship removed content ops observation pages', () => {
    expect(existsSync(resolve(adminPagesRoot, 'content-hub.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'articles/index.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'projects/index.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'categories.vue'))).toBe(false)
  })

  it('does not ship removed asset management module', () => {
    expect(existsSync(resolve(adminPagesRoot, 'asset-management/index.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'investment.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'dca-plan.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'price-alert.vue'))).toBe(false)
    expect(adminMenuPaths).not.toContain('/admin/asset-management')
  })

  it('keeps cognition admin page but removes menu entry (Life owns public route)', () => {
    expect(existsSync(resolve(adminPagesRoot, 'cognition/index.vue'))).toBe(true)
    expect(adminMenuPaths).not.toContain('/admin/cognition')
  })

  it('does not ship removed site content ops (tools / friend-links admin)', () => {
    expect(existsSync(resolve(adminPagesRoot, 'content.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'tools.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'friend-links.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'friend-links/edit/[[id]].vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'toolbox/[id]/analytics.vue'))).toBe(false)
    expect(existsSync(resolve(root, 'components/admin/content/AdminToolsPanel.vue'))).toBe(false)
    expect(existsSync(resolve(root, 'components/admin/content/AdminFriendLinksPanel.vue'))).toBe(false)
    expect(existsSync(resolve(root, 'components/admin/dashboard/SiteContentCard.vue'))).toBe(false)
  })

  it('ai content preview does not POST Articles CMS payload', () => {
    const src = readSrc('pages/admin/ai/content.vue')
    expect(src).not.toMatch(/POST.*Articles|api\.post\(['"]\/Articles/)
    expect(src).not.toMatch(/articles\/edit/)
    expect(src).toMatch(/复制 Markdown/)
  })

  it('admin menu does not reference deleted CMS routes', () => {
    const menuSrc = readSrc('constants/admin/menu.ts')
    expect(menuSrc).not.toMatch(/articles\/edit/)
    expect(menuSrc).not.toMatch(/projects\/edit/)
    expect(menuSrc).not.toMatch(/\/admin\/modules/)
    expect(menuSrc).not.toMatch(/AI 内容/)
  })

  it('menu paths map to existing admin pages (no orphan menu entries)', () => {
    const vueFiles = collectVueFiles(adminPagesRoot)
    const pagePaths = new Set(
      vueFiles.map((file) => {
        const rel = file.replace(/\\/g, '/').split('pages/admin/')[1]?.replace(/\.vue$/, '') || ''
        if (rel === 'index') return '/admin'
        if (rel.endsWith('/index')) return `/admin/${rel.replace(/\/index$/, '')}`
        return `/admin/${rel}`
      }),
    )

    for (const path of adminMenuPaths) {
      const hasExact = pagePaths.has(path)
      const hasChild = [...pagePaths].some(p => p.startsWith(`${path}/`))
      expect(hasExact || hasChild, `menu path missing page: ${path}`).toBe(true)
    }
  })

  it('site content ops stay off sidebar and off disk', () => {
    expect(existsSync(resolve(adminPagesRoot, 'content.vue'))).toBe(false)
    expect(adminMenuPaths).not.toContain('/admin/content')
    expect(adminMenuPaths).not.toContain('/admin/tools')
  })

  it('preserves work/life content SoT outside admin CMS', () => {
    expect(existsSync(resolve(root, 'content/work/home.yml'))).toBe(true)
    expect(existsSync(resolve(root, 'content/life/home.yml'))).toBe(true)
  })

  it('admin layout does not wrap page shell in ClientOnly', () => {
    const src = readSrc('layouts/admin.vue')
    const mainShell = src.match(/admin-main-scroll">([\s\S]*?)<\/div>\s*<\/main>/)?.[1] ?? ''
    expect(mainShell).toMatch(/admin-page-shell/)
    expect(mainShell).not.toMatch(/ClientOnly/)
  })

  it('admin pages do not disable SSR (avoids blank main content)', () => {
    const vueFiles = collectVueFiles(adminPagesRoot)
    const offenders: string[] = []
    for (const file of vueFiles) {
      const src = readFileSync(file, 'utf8')
      if (/ssr:\s*false/.test(src)) {
        offenders.push(relative(root, file))
      }
    }
    expect(offenders).toEqual([])
  })
})

describe('Admin menu structure', () => {
  it('uses a data-first slim sidebar', () => {
    const labels = adminMenu.map(g => g.label)
    expect(labels).toEqual(['数据', '互动'])
    expect(adminMenuPaths).toEqual([
      '/admin',
      '/admin/analytics',
      '/admin/ai',
      '/admin/visitor-messages',
      '/admin/consultations',
    ])
    expect(adminMenuPaths).not.toContain('/admin/content')
  })

  it('keeps deep analytics routes off the sidebar', () => {
    expect(adminMenuPaths).not.toContain('/admin/visitors')
    expect(adminMenuPaths).not.toContain('/admin/projects/stats')
    expect(adminMenuPaths).not.toContain('/admin/ai/logs')
    expect(adminMenuPaths).not.toContain('/admin/ai/support-config')
    expect(adminMenuPaths).not.toContain('/admin/orders')
    expect(adminMenuPaths).not.toContain('/admin/thoughts')
    expect(adminMenuPaths).not.toContain('/admin/content')
  })

  it('does not ship removed personal workspace modules', () => {
    expect(existsSync(resolve(adminPagesRoot, 'intelligence/index.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'side-projects/index.vue'))).toBe(false)
    expect(existsSync(resolve(root, 'components/NotificationBell.vue'))).toBe(false)
  })
})

describe('Admin legacy cleanup guards (Phase 3.1)', () => {
  it('removes toolbox admin pages entirely', () => {
    expect(existsSync(resolve(adminPagesRoot, 'toolbox/index.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'toolbox/[id]/analytics.vue'))).toBe(false)
  })

  it('keeps orders page as direct link only (off sidebar)', () => {
    expect(adminMenuPaths.filter(p => p.includes('order'))).toEqual([])
    expect(existsSync(resolve(adminPagesRoot, 'commercial/orders.vue'))).toBe(false)
    expect(existsSync(resolve(adminPagesRoot, 'orders.vue'))).toBe(true)
  })

  it('ModuleCard does not link to removed system settings routes', () => {
    const src = readSrc('components/ModuleCard.vue')
    expect(src).not.toMatch(/\/admin\/settings/)
  })

  it('admin sources do not link to dead toolbox index or duplicate orders', () => {
    const sources = [
      'constants/admin/menu.ts',
      'pages/admin/index.vue',
      'components/ModuleCard.vue',
    ]
    for (const file of sources) {
      const src = readSrc(file)
      expect(src, file).not.toMatch(/\/admin\/toolbox['"`]/)
      expect(src, file).not.toMatch(/commercial\/orders/)
    }
  })

  it('admin sources do not link to removed content ops', () => {
    const sources = [
      'constants/admin/menu.ts',
      'pages/admin/index.vue',
      'pages/admin/ai/index.vue',
    ]
    for (const file of sources) {
      const src = readSrc(file)
      expect(src, file).not.toMatch(/\/admin\/content/)
      expect(src, file).not.toMatch(/\/admin\/toolbox/)
    }
  })
})
