import { readMarkdownDocument } from '../../../utils/content-files'

export default defineEventHandler((event) => {
  const { slug } = getRouterParams(event)
  const item = readMarkdownDocument(['projects'], slug)

  if (!item) {
    throw createError({ statusCode: 404, statusMessage: 'Project not found' })
  }

  return item
})
