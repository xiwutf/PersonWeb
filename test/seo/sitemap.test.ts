import { describe, expect, it } from 'vitest'
import {
  canonicalizePath,
  isPublicIndexablePath,
  uniqPaths,
  buildSitemapXml,
  STATIC_PATHS,
  collectArticlePathsFromGit,
  diffArticleSitemapPaths,
} from '../../scripts/lib/sitemap-builder.js'
import { resolve } from 'node:path'

describe('sitemap builder', () => {
  it('excludes admin and api paths', () => {
    const paths = uniqPaths([
      ...STATIC_PATHS,
      '/admin/login',
      '/api/Articles',
      '/dashboard',
      '/work/blog/real-post',
    ])

    expect(paths.some((p) => p.startsWith('/admin'))).toBe(false)
    expect(paths.some((p) => p.startsWith('/api'))).toBe(false)
    expect(paths).toContain('/work/blog/real-post')
    expect(paths).toContain('/work')
  })

  it('canonicalizes legacy tools detail routes', () => {
    expect(canonicalizePath('/tools/detail-foo')).toBe('/work/tools/foo')
    expect(canonicalizePath('/projects/detail-old')).toBe('/work/projects')
    expect(canonicalizePath('/blog/hello')).toBe('/work/blog/hello')
    expect(canonicalizePath('/about')).toBe('/work/about')
    expect(canonicalizePath('/ai')).toBe('/work/ai')
    expect(canonicalizePath('/ai/chat-bot')).toBe('/work/ai/chat-bot')
    expect(canonicalizePath('/lab')).toBe('/work/lab')
  })

  it('deduplicates URLs', () => {
    const { paths } = buildSitemapXml('https://xifg.com.cn', [
      '/work/blog/a',
      '/work/blog/a/',
      '/work/blog/a',
    ])
    expect(paths.filter((p) => p.startsWith('/work/blog/a')).length).toBe(1)
  })

  it('includes at least one dynamic URL when provided', () => {
    const { xml, paths } = buildSitemapXml('https://xifg.com.cn', [
      ...STATIC_PATHS,
      '/work/projects/11111111-1111-1111-1111-111111111111',
    ])
    expect(paths).toContain('/work/projects/11111111-1111-1111-1111-111111111111')
    expect(xml).toContain('https://xifg.com.cn/work/projects/11111111-1111-1111-1111-111111111111')
    expect(xml).not.toContain('/admin/')
    expect(xml).not.toContain('/api/')
  })

  it('marks private paths as non-indexable', () => {
    expect(isPublicIndexablePath('/order/create')).toBe(false)
    expect(isPublicIndexablePath('/payment/cancel')).toBe(false)
  })

  it('can collect Git article paths and diff against API set', () => {
    const gitPaths = collectArticlePathsFromGit(resolve(__dirname, '../../content/articles'))
    expect(gitPaths.every((p: string) => p.startsWith('/work/blog/'))).toBe(true)
    const diff = diffArticleSitemapPaths(gitPaths, gitPaths)
    expect(diff.equal).toBe(true)
  })
})
