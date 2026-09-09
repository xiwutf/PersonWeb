import { checkAuth } from '../../../utils/auth'
import { createLifeNote } from '../../../utils/content-files'

export default defineEventHandler(async (event) => {
  checkAuth(event)

  const body = await readBody(event)
  const title = typeof body?.title === 'string' ? body.title : ''
  const content = typeof body?.content === 'string' ? body.content : ''
  const description = typeof body?.description === 'string' ? body.description : ''
  const date = typeof body?.date === 'string' ? body.date : undefined

  if (!title.trim() || !content.trim()) {
    throw createError({
      statusCode: 400,
      statusMessage: 'title and content required',
    })
  }

  try {
    const note = createLifeNote({ title, content, description, date })
    if (!note) {
      throw createError({
        statusCode: 503,
        statusMessage: 'Unable to write content file',
      })
    }
    setHeader(event, 'Cache-Control', 'no-store')
    return note
  } catch (error: unknown) {
    const message = error instanceof Error ? error.message : String(error)
    if (message.includes('required') || message.includes('slug')) {
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
