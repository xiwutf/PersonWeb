import { describe, expect, it } from 'vitest'
import { adminMenuPaths } from '../../constants/admin/menu'
import {
  isAdminNavActive,
  resolveActiveAdminNavPath,
} from '../../utils/admin-nav-active'

describe('Admin nav active matcher', () => {
  function activesOn(route: string) {
    return adminMenuPaths.filter((path) => isAdminNavActive(route, path, adminMenuPaths))
  }

  it('marks only 网站概览 on /admin', () => {
    expect(activesOn('/admin')).toEqual(['/admin'])
    expect(resolveActiveAdminNavPath('/admin', adminMenuPaths)).toBe('/admin')
  })

  it('does not keep 网站概览 active on nested admin routes', () => {
    expect(isAdminNavActive('/admin/visitors', '/admin', adminMenuPaths)).toBe(false)
    expect(activesOn('/admin/visitors')).toEqual(['/admin/visitors'])
    expect(activesOn('/admin/content')).toEqual(['/admin/content'])
    expect(activesOn('/admin/analytics')).toEqual(['/admin/analytics'])
  })

  it('prefers the longest matching menu path', () => {
    expect(activesOn('/admin/ai/logs')).toEqual(['/admin/ai/logs'])
    expect(isAdminNavActive('/admin/ai/logs', '/admin/ai', adminMenuPaths)).toBe(false)
    expect(activesOn('/admin/ai')).toEqual(['/admin/ai'])
  })
})
