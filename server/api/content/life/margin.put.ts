import { checkAuth } from '../../../utils/auth'
import { writeLifeMargin, type LifeMarginItem } from '../../../utils/content-files'

export default defineEventHandler(async (event) => {
  checkAuth(event)

  const body = await readBody(event)
  const items = Array.isArray(body?.items) ? body.items as LifeMarginItem[] : null
  if (!items) {
    throw createError({
      statusCode: 400,
      statusMessage: 'items array required',
    })
  }

  try {
    const data = writeLifeMargin(items)
    setHeader(event, 'Cache-Control', 'no-store')
    return data
  } catch (error) {
    const detail = error instanceof Error ? error.message : 'Unable to write content file'
    console.error('[life/margin.put] write failed:', detail)
    throw createError({
      statusCode: 503,
      statusMessage: detail,
      message: detail,
    })
  }
})
