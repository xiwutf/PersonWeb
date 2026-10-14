import { describe, expect, it } from 'vitest'
import { WARM_PAPER_PRESET, readingMargin, warmPaperStyles } from '../../utils/library-reading-preset'

describe('Calibre warm paper reading preset', () => {
  it('keeps the exact exported layout parameters', () => {
    expect(WARM_PAPER_PRESET).toMatchObject({ name: '暖纸·幼圆·舒适阅读', fontSize: 22, lineHeight: 1.7,
      paragraphSpacing: 0.55, indent: 2, maxTextWidth: 760, marginX: 40, marginY: 28, headingLineHeight: 1.4, linkColor: '#866344' })
    expect(readingMargin(840)).toBe(40); expect(readingMargin(390)).toBe(20)
  })
  it('applies manual font size to paragraph text without flattening heading or image sizes', () => {
    const css = warmPaperStyles(24, { background: '#f4ecd8', text: '#453b31', link: '#866344' })
    expect(css['p, li, blockquote']['font-size']).toBe('24px !important')
    expect(css['body, body *']['font-family']).toBe('"YouYuan", "幼圆", sans-serif !important')
    expect(css.p['margin-bottom']).toBe('0.55em !important')
    expect(css.p['text-indent']).toBe('2em !important')
    for (const rule of Object.values(css)) { expect(rule).not.toHaveProperty('width'); expect(rule).not.toHaveProperty('height') }
    expect(JSON.stringify(css)).not.toContain('@font-face'); expect(JSON.stringify(css)).not.toContain('url(')
  })
})
