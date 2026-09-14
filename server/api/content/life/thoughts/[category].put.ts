import { checkAuth } from '../../../../utils/auth'
import {
  writeLifeThoughtItems,
  type LifeThoughtItem,
} from '../../../../utils/content-files'

export default defineEventHandler(async (event) => {
  checkAuth(event)

  const slug = String(getRouterParam(event, 'category') || '').trim()
  const body = await readBody(event)
  const items = Array.isArray(body?.items) ? body.items as LifeThoughtItem[] : null
  if (!items) {
    throw createError({
      statusCode: 400,
      statusMessage: 'items array required',
    })
  }

  try {
    const data = writeLifeThoughtItems(slug, items)
    setHeader(event, 'Cache-Control', 'no-store')
    return { items: data }
  }
  catch (error) {
    const detail = error instanceof Error ? error.message : 'Unable to write content file'
    if (detail === 'Thought category not found' || detail === 'Invalid thought category') {
      throw createError({
        statusCode: 404,
        statusMessage: detail,
      })
    }
    console.error('[life/thoughts.put] write failed:', detail)
    throw createError({
      statusCode: 503,
      statusMessage: detail,
      message: detail,
    })
  }
})
