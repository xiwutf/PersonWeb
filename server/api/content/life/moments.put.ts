import { checkAuth } from '../../../utils/auth'
import { writeLifeMoments, type LifeMoment } from '../../../utils/content-files'

export default defineEventHandler(async (event) => {
  checkAuth(event)

  const body = await readBody(event)
  const items = Array.isArray(body?.items) ? body.items as LifeMoment[] : null
  if (!items) {
    throw createError({
      statusCode: 400,
      statusMessage: 'items array required',
    })
  }

  try {
    const data = writeLifeMoments(items)
    setHeader(event, 'Cache-Control', 'no-store')
    return data
  } catch {
    throw createError({
      statusCode: 503,
      statusMessage: 'Unable to write content file',
    })
  }
})
