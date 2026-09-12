import { checkAuth } from '../../../utils/auth'
import {
  writeLifeCognition,
  type LifeCognitionChapter,
} from '../../../utils/content-files'

export default defineEventHandler(async (event) => {
  checkAuth(event)

  const body = await readBody(event)
  if (!body || typeof body !== 'object') {
    throw createError({
      statusCode: 400,
      statusMessage: 'body required',
    })
  }

  const payload = body as {
    title?: string
    summary?: string
    chapters?: LifeCognitionChapter[]
  }

  if (payload.chapters !== undefined && !Array.isArray(payload.chapters)) {
    throw createError({
      statusCode: 400,
      statusMessage: 'chapters array required',
    })
  }

  try {
    const data = writeLifeCognition({
      title: typeof payload.title === 'string' ? payload.title : undefined,
      summary: typeof payload.summary === 'string' ? payload.summary : undefined,
      chapters: payload.chapters,
    })
    setHeader(event, 'Cache-Control', 'no-store')
    return data
  }
  catch (error) {
    const detail = error instanceof Error ? error.message : 'Unable to write content file'
    console.error('[life/cognition.put] write failed:', detail)
    throw createError({
      statusCode: 503,
      statusMessage: detail,
      message: detail,
    })
  }
})
