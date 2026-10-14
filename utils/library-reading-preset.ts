// Transcribed from the user's Calibre profiles.json: viewer: / 暖纸·幼圆·舒适阅读.
export const WARM_PAPER_PRESET = {
  id: 'warm-paper-youyuan', name: '暖纸·幼圆·舒适阅读',
  fontFamily: '"YouYuan", "幼圆", sans-serif', fontSize: 22,
  lineHeight: 1.7, paragraphSpacing: 0.55, indent: 2,
  maxTextWidth: 760, marginX: 40, marginY: 28, headingLineHeight: 1.4,
  linkColor: '#866344',
} as const

export function readingMargin(width: number) { return width <= 600 ? 20 : WARM_PAPER_PRESET.marginX }

export function warmPaperStyles(fontSize: number, colors: { background: string; text: string; link: string }) {
  // Only typography and colors are overridden. Heading sizes and image/table geometry remain intact.
  return {
    'body, body *': { 'font-family': `${WARM_PAPER_PRESET.fontFamily} !important` },
    body: { 'font-size': `${fontSize}px !important`, 'line-height': '1.7 !important',
      color: `${colors.text} !important`, 'background-color': `${colors.background} !important`,
      'padding-top': '28px !important', 'padding-bottom': '28px !important' },
    'body *': { color: 'inherit !important', 'background-color': 'transparent !important' },
    'p, li, blockquote': { 'font-size': `${fontSize}px !important`, 'line-height': '1.7 !important' },
    p: { 'margin-top': '0 !important', 'margin-bottom': '0.55em !important', 'text-indent': '2em !important' },
    'p span, li span': { 'font-size': 'inherit !important', 'line-height': 'inherit !important' },
    'h1, h2, h3': { 'line-height': '1.4 !important' },
    'a, a *': { color: `${colors.link} !important` },
    'p:has(> img:only-child), p:has(> svg:only-child)': { 'text-indent': '0 !important' },
  }
}
