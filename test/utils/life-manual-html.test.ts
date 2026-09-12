import { describe, expect, it } from 'vitest'
import { enhanceManualHtml } from '../../utils/life-manual-html'

describe('enhanceManualHtml', () => {
  it('wraps bare prose when there is no h2', () => {
    const html = enhanceManualHtml('<p>hello</p>')
    expect(html).toContain('life-manual-prose')
    expect(html).toContain('<p>hello</p>')
  })

  it('splits h2 blocks into chapter cards', () => {
    const html = enhanceManualHtml('<p>intro</p><h2>一、开头</h2><p>a</p><h2>二、继续</h2><p>b</p>')
    expect(html).toContain('life-manual-intro')
    expect(html.match(/life-manual-chapter/g)?.length).toBe(2)
    expect(html).toContain('一、开头')
    expect(html).toContain('二、继续')
  })
})
