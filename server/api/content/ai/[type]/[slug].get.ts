import { readMarkdownDocument } from '../../../../utils/content-files'

export default defineEventHandler((event) => {
  const { type, slug } = getRouterParams(event)
  const item = readMarkdownDocument(['ai', type], slug)

  if (!item) {
    throw createError({ statusCode: 404, statusMessage: 'AI content not found' })
  }

  return item
})
