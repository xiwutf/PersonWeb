import { readMarkdownDocument } from '../../../utils/content-files'

export default defineEventHandler((event) => {
  const { slug } = getRouterParams(event)
  const item = readMarkdownDocument(['cognition'], slug)

  if (!item) {
    throw createError({ statusCode: 404, statusMessage: 'Cognition document not found' })
  }

  setHeader(event, 'Cache-Control', 'public, max-age=300, s-maxage=300')
  return item
})
