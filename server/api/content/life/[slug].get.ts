import { readMarkdownDocument } from '../../../utils/content-files'

export default defineEventHandler((event) => {
  const { slug } = getRouterParams(event)
  const item = readMarkdownDocument(['life'], slug)

  if (!item) {
    throw createError({ statusCode: 404, statusMessage: 'Life article not found' })
  }

  return item
})
