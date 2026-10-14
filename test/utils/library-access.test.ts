import { beforeEach, describe, expect, it, vi } from 'vitest'
import { useLibraryAccess } from '../../composables/useLibraryAccess'

const fetchMock = vi.fn()
const session = () => ({ authenticated: true, csrfToken: 'csrf-test', expiresAt: new Date(Date.now() + 30 * 86400000).toISOString() })
const unauthorized = () => ({ response: { status: 401 } })

beforeEach(() => {
  const states = new Map<string, { value: unknown }>()
  vi.stubGlobal('useState', (key: string, init: () => unknown) => {
    if (!states.has(key)) states.set(key, { value: init() })
    return states.get(key)
  })
  vi.stubGlobal('useRuntimeConfig', () => ({ public: { apiBase: 'http://localhost:5234/api' } }))
  vi.stubGlobal('$fetch', fetchMock)
  fetchMock.mockReset()
})

describe('reading cookie access', () => {
  it('protects progress PUT with cookie, csrf and keepalive', async () => {
    fetchMock.mockResolvedValueOnce(session()).mockResolvedValueOnce({})
    const access = useLibraryAccess(); await access.check()
    await access.request('/books/1/progress', 'PUT', { cfi: 'test' }, { keepalive: true })
    expect(fetchMock.mock.calls[1][1]).toMatchObject({ method: 'PUT', credentials: 'include', keepalive: true, headers: { 'X-Reading-CSRF': 'csrf-test' }, retry: 0 })
  })
  it('uses cookies and csrf for multipart upload and supports private binary reads', async () => {
    fetchMock.mockResolvedValueOnce(session()).mockResolvedValueOnce({}).mockResolvedValueOnce(new ArrayBuffer(1))
    const access = useLibraryAccess(); await access.check()
    const form = new FormData(); form.append('file', new Blob(['epub']), 'book.epub')
    await access.request('/books', 'POST', form)
    expect(fetchMock.mock.calls[1][1]).toMatchObject({ credentials: 'include', body: form, headers: { 'X-Reading-CSRF': 'csrf-test' } })
    await access.request('/books/1/content', 'GET', undefined, { responseType: 'arrayBuffer' })
    expect(fetchMock.mock.calls[2][1]).toMatchObject({ credentials: 'include', cache: 'no-store', responseType: 'arrayBuffer' })
  })
  it('restores a valid session without sending a password', async () => {
    fetchMock.mockResolvedValueOnce(session())
    const access = useLibraryAccess()
    await access.check()
    expect(access.authenticated.value).toBe(true)
    expect(fetchMock).toHaveBeenCalledTimes(1)
    expect(fetchMock).toHaveBeenCalledWith(expect.stringContaining('/library/auth/session'), expect.objectContaining({ method: 'GET', credentials: 'include', cache: 'no-store' }))
  })

  it('asks for authorization when session is expired', async () => {
    fetchMock.mockRejectedValueOnce(unauthorized())
    const access = useLibraryAccess()
    await access.check()
    expect(access.authenticated.value).toBe(false)
    expect(access.loading.value).toBe(false)
    expect(access.error.value).toBe('')
  })

  it('surfaces missing reading configuration on session probe', async () => {
    fetchMock.mockRejectedValueOnce({ response: { status: 503 }, data: { error: 'reading_not_configured' } })
    const access = useLibraryAccess()
    await access.check()
    expect(access.error.value).toContain('OwnerKey')
    expect(access.error.value).toContain('AllowedOrigins')
  })

  it('confirms browser cookie acceptance after password login', async () => {
    fetchMock.mockResolvedValueOnce(session()).mockResolvedValueOnce(session())
    const access = useLibraryAccess()
    await access.login('test-password')
    expect(access.authenticated.value).toBe(true)
    expect(fetchMock).toHaveBeenCalledTimes(2)
    expect(fetchMock.mock.calls[0][1].body).toEqual({ password: 'test-password' })
    expect(fetchMock.mock.calls[1][0]).toContain('/auth/session')
  })

  it('reports blocked secure cookies separately from wrong password', async () => {
    fetchMock.mockResolvedValueOnce(session()).mockRejectedValueOnce(unauthorized())
    const access = useLibraryAccess()
    await access.login('correct-password')
    expect(access.authenticated.value).toBe(false)
    expect(access.error.value).toContain('密码已验证')
    expect(access.error.value).toContain('HTTPS')
  })

  it('sends csrf on logout and clears authorization after server success', async () => {
    fetchMock.mockResolvedValueOnce(session()).mockResolvedValueOnce(undefined)
    const access = useLibraryAccess()
    await access.check()
    await access.logout()
    expect(fetchMock.mock.calls[1][1].headers).toEqual({ 'X-Reading-CSRF': 'csrf-test' })
    expect(access.authenticated.value).toBe(false)
  })

  it('does not claim logout when server revocation fails', async () => {
    fetchMock.mockResolvedValueOnce(session()).mockRejectedValueOnce({ response: { status: 503 } })
    const access = useLibraryAccess()
    await access.check()
    await access.logout()
    expect(access.authenticated.value).toBe(true)
    expect(access.error.value).not.toBe('')
  })

  it('renews a valid session close to expiry with csrf', async () => {
    fetchMock.mockResolvedValueOnce({ ...session(), expiresAt: new Date(Date.now() + 86400000).toISOString() }).mockResolvedValueOnce(session())
    const access = useLibraryAccess()
    await access.check()
    expect(fetchMock.mock.calls[1][0]).toContain('/auth/renew')
    expect(fetchMock.mock.calls[1][1].headers).toEqual({ 'X-Reading-CSRF': 'csrf-test' })
  })
})
