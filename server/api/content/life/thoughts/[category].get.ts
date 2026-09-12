import { readLifeThoughtCategoryPage } from '../../../../utils/content-files'

export default defineEventHandler((event) => {
  const slug = String(getRouterParam(event, 'category') || '').trim()
  const page = readLifeThoughtCategoryPage(slug)
  if (!page) {
    throw createError({
      statusCode: 404,
      statusMessage: 'Category not found',
    })
  }

  setHeader(event, 'Cache-Control', 'public, max-age=60, s-maxage=60')
  return page
})
