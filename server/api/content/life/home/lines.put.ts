import { checkAuth } from '../../../../utils/auth'
import { replaceLifeHomeLines } from '../../../../utils/content-home-patch'

export default defineEventHandler(async (event) => {
  checkAuth(event)

  const body = await readBody(event)
  const lines = Array.isArray(body?.lines) ? body.lines : null
  if (!lines || !lines.every((line: unknown) => typeof line === 'string')) {
    throw createError({
      statusCode: 400,
      statusMessage: 'lines string array required',
    })
  }

  try {
    const data = replaceLifeHomeLines(lines)
    setHeader(event, 'Cache-Control', 'no-store')
    return data
  } catch (error: unknown) {
    const message = error instanceof Error ? error.message : String(error)
    if (message.includes('at least one')) {
      throw createError({
        statusCode: 400,
        statusMessage: message,
      })
    }
    throw createError({
      statusCode: 503,
      statusMessage: 'Unable to write content file',
    })
  }
})
