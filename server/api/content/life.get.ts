import { readMarkdownCollection } from '../../utils/content-files'

export default defineEventHandler((event) => {
  setHeader(event, 'Cache-Control', 'public, max-age=30, must-revalidate')
  return readMarkdownCollection('life')
})
