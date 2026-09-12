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
    expect(activesOn('/admin/visitors')).toEqual([])
    expect(activesOn('/admin/analytics')).toEqual(['/admin/analytics'])
    expect(activesOn('/admin/consultations')).toEqual(['/admin/consultations'])
  })

  it('keeps AI 中心 active for nested AI tools pages', () => {
    expect(activesOn('/admin/ai/logs')).toEqual(['/admin/ai'])
    expect(activesOn('/admin/ai/support-config')).toEqual(['/admin/ai'])
    expect(activesOn('/admin/ai')).toEqual(['/admin/ai'])
  })
})
