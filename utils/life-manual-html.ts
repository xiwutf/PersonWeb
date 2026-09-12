/**
 * Wrap markdown HTML so each h2 becomes a manual chapter card.
 * Leading content before the first h2 stays as intro prose.
 */
export function enhanceManualHtml(html: string): string {
  const source = html.trim()
  if (!source) return ''

  const parts = source.split(/(?=<h2\b[^>]*>)/i)
  if (parts.length < 2) {
    return `<div class="life-manual-prose">${source}</div>`
  }

  const intro = parts[0]?.trim()
    ? `<div class="life-manual-intro life-manual-prose">${parts[0]}</div>`
    : ''

  const chapters = parts
    .slice(1)
    .map((chunk, index) => {
      const body = chunk.trim()
      if (!body) return ''
      return `<section class="life-manual-chapter" style="--chapter-i:${index}">${body}</section>`
    })
    .filter(Boolean)
    .join('')

  return `${intro}${chapters}`
}
