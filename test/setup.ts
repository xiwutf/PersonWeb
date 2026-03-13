/**
 * Vitest 测试环境设置文件
 */

import { beforeEach, vi } from 'vitest'

// Mock Nuxt composables
vi.mock('#app', () => ({
  definePageMeta: () => {},
  useRoute: () => ({
    path: '/',
    params: {},
    query: {},
    name: 'index'
  }),
  useRouter: () => ({
    push: vi.fn(),
    replace: vi.fn(),
    go: vi.fn(),
    back: vi.fn(),
    forward: vi.fn()
  }),
  useNuxtApp: () => ({
    payload: {},
    provide: vi.fn()
  }),
  useState: (key: string, init?: any) => {
    const state = ref(init ? init() : undefined)
    return state
  },
  useCookie: (name: string) => ({
    value: '',
    remove: vi.fn()
  }),
  navigateTo: vi.fn()
}))

// Mock Vue composables
vi.mock('vue', async () => {
  const actual = await vi.importActual('vue')
  return {
    ...actual,
    onMounted: vi.fn((fn) => fn()),
    onUnmounted: vi.fn(),
    watch: vi.fn(),
    computed: (fn: any) => ({ get value() { return fn() } }),
    ref: (val: any) => ({ value: val }),
    reactive: (val: any) => val,
    nextTick: vi.fn(() => Promise.resolve()),
    getCurrentInstance: vi.fn(() => null)
  }
})

// Mock $fetch
vi.mock('ofetch', () => ({
  $fetch: vi.fn()
}))

// 全局测试配置
beforeEach(() => {
  // 清理所有 mock
  vi.clearAllMocks()
})

// 扩展 Vitest 环境
declare global {
  const vi: typeof import('vitest')['vi']
}
