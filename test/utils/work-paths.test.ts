import { describe, expect, it } from 'vitest'
import {
  nestLegacyWorkPath,
  workBlogPath,
  workProjectPath,
  WORK_ROUTES,
} from '../../utils/work-paths'

describe('nestLegacyWorkPath', () => {
  it('nests Work root columns under /work', () => {
    expect(nestLegacyWorkPath('/blog')).toBe('/work/blog')
    expect(nestLegacyWorkPath('/blog/hello')).toBe('/work/blog/hello')
    expect(nestLegacyWorkPath('/projects/123')).toBe('/work/projects/123')
    expect(nestLegacyWorkPath('/about')).toBe('/work/about')
    expect(nestLegacyWorkPath('/contact?from=nav')).toBe('/work/contact?from=nav')
  })

  it('keeps already nested Work URLs and other worlds', () => {
    expect(nestLegacyWorkPath('/work')).toBe('/work')
    expect(nestLegacyWorkPath('/work/blog/hello')).toBe('/work/blog/hello')
    expect(nestLegacyWorkPath('/life/about')).toBe('/life/about')
    expect(nestLegacyWorkPath('/admin/projects')).toBe('/admin/projects')
    expect(nestLegacyWorkPath('/search')).toBe('/search')
  })

  it('rewrites legacy detail aliases', () => {
    expect(nestLegacyWorkPath('/projects/detail-old')).toBe('/work/projects')
    expect(nestLegacyWorkPath('/tools/detail-foo')).toBe('/work/tools/foo')
  })

  it('exposes helpers for blog and project canonicals', () => {
    expect(workBlogPath('hello')).toBe('/work/blog/hello')
    expect(workProjectPath('abc')).toBe('/work/projects/abc')
    expect(WORK_ROUTES.home).toBe('/work')
  })
})
