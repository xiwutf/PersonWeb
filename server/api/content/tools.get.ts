import { readMarkdownCollection } from '../../utils/content-files'

export default defineEventHandler(() => readMarkdownCollection('tools'))
