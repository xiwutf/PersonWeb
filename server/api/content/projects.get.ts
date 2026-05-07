import { readMarkdownCollection } from '../../utils/content-files'

export default defineEventHandler((event) => {
  setHeader(event, 'Cache-Control', 'public, max-age=300, s-maxage=300')
  return readMarkdownCollection('projects')
})
