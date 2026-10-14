import { describe, expect, it } from 'vitest'
import { flattenToc, swipeDirection, protectEpubDocument, libraryError } from '../../utils/library-reader'
describe('reading interactions and document safety', () => {
  it('keeps nested chapter order and targets', () => {
    expect(flattenToc([{ label: ' Chapter ', href: 'a.xhtml', subitems: [{ label: 'Part', href: 'a.xhtml#p' }] }])).toEqual([
      { label: 'Chapter', href: 'a.xhtml', depth: 0 }, { label: 'Part', href: 'a.xhtml#p', depth: 1 },
    ])
  })
  it('requires an intentional horizontal gesture', () => {
    expect(swipeDirection(-100, 5)).toBe('next'); expect(swipeDirection(100, 5)).toBe('prev')
    expect(swipeDirection(30, 0)).toBeNull(); expect(swipeDirection(100, 100)).toBeNull()
  })
  it('blocks scripts and remote resources before rendering', () => {
    const doc = new DOMParser().parseFromString('<html><head><base href="/OPS/chapter.xhtml"/><title>Book</title></head><body>Text</body></html>', 'text/html')
    protectEpubDocument(doc)
    expect(doc.head.firstElementChild?.getAttribute('http-equiv')).toBe('Content-Security-Policy')
    const csp = doc.head.firstElementChild?.getAttribute('content')
    expect(csp).toContain("script-src 'none'"); expect(csp).toContain("connect-src 'none'")
    expect(csp).toContain('img-src blob: data:'); expect(csp).not.toContain('https:')
    // epub.js resolves internal links using this attribute; CSP still blocks browser base changes.
    expect(doc.querySelector('base')?.getAttribute('href')).toBe('/OPS/chapter.xhtml')
  })
  it('explains rejected formats and file limits', () => {
    expect(libraryError({ response: { status: 422 } })).toContain('安全校验')
    expect(libraryError({ response: { status: 413 } })).toContain('100 MiB')
  })
  it('explains MOBI conversion availability, failure and resource limits', () => {
    expect(libraryError({ data: { error: 'converter_unavailable' } })).toContain('Calibre')
    expect(libraryError({ data: { error: 'conversion_busy' } })).toContain('稍后')
    expect(libraryError({ data: { error: 'conversion_failed' } })).toContain('DRM')
    expect(libraryError({ data: { error: 'conversion_timeout' } })).toContain('超时')
    expect(libraryError({ data: { error: 'conversion_limit' } })).toContain('限制')
    expect(libraryError({ data: { error: 'conversion_rejected' } }, 'mobi')).toContain('MOBI 已转换')
    expect(libraryError({ response: { status: 422 } }, 'mobi')).toContain('MOBI 已转换')
  })
})
