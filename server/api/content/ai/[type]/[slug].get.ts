import { readMarkdownDocument } from '../../../../utils/content-files'

export default defineEventHandler((event) => {
  const { type, slug } = getRouterParams(event)
  const item = readMarkdownDocument(['ai', type], slug)

  if (!item) {
    throw createError({ statusCode: 404, statusMessage: 'AI content not found' })
  }

  setHeader(event, 'Cache-Control', 'public, max-age=300, s-maxage=300')
  return item
})
