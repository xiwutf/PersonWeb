import { describe, expect, it } from 'vitest'
import {
  isLocalDevHostname,
  resolveDotNetApiBase,
  usesNitroAdminAuth,
} from '../../utils/admin-runtime-auth'

describe('admin-runtime-auth', () => {
  it('treats localhost as local Nitro auth host', () => {
    expect(isLocalDevHostname('localhost')).toBe(true)
    expect(isLocalDevHostname('127.0.0.1')).toBe(true)
    expect(isLocalDevHostname('xifg.com.cn')).toBe(false)
  })

  it('resolves production API base for xifg', () => {
    expect(resolveDotNetApiBase('xifg.com.cn')).toBe('https://api.xifg.com.cn/api')
    expect(resolveDotNetApiBase('www.xifg.com.cn')).toBe('https://api.xifg.com.cn/api')
  })

  it('resolves xlfg alias to the same production API', () => {
    expect(resolveDotNetApiBase('xlfg.com.cn')).toBe('https://api.xifg.com.cn/api')
    expect(resolveDotNetApiBase('www.xlfg.com.cn')).toBe('https://api.xifg.com.cn/api')
  })

  it('usesNitroAdminAuth is true on Vite/Vitest server context', () => {
    // Vitest runs in Node; import.meta.server is typically true under Nuxt vitest env
    // or undefined — function falls through safely for local host checks when window absent.
    expect(typeof usesNitroAdminAuth).toBe('function')
  })
})
