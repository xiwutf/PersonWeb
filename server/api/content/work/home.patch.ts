import { checkAuth } from '../../../utils/auth'
import { patchWorkHomeField } from '../../../utils/content-home-patch'

export default defineEventHandler(async (event) => {
  checkAuth(event)

  const body = await readBody(event)
  const path = typeof body?.path === 'string' ? body.path.trim() : ''
  const value = typeof body?.value === 'string' ? body.value : null

  if (!path || value === null) {
    throw createError({
      statusCode: 400,
      statusMessage: 'path and string value required',
    })
  }

  try {
    const data = patchWorkHomeField(path, value)
    setHeader(event, 'Cache-Control', 'no-store')
    return data
  } catch (error: unknown) {
    const message = error instanceof Error ? error.message : String(error)
    if (message.includes('not allowed')) {
      throw createError({
        statusCode: 400,
        statusMessage: 'Field path not allowed',
      })
    }
    throw createError({
      statusCode: 503,
      statusMessage: 'Unable to write content file',
    })
  }
})
