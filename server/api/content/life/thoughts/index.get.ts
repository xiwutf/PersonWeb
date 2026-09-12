import { readLifeThoughtIndex } from '../../../../utils/content-files'

export default defineEventHandler((event) => {
  setHeader(event, 'Cache-Control', 'public, max-age=60, s-maxage=60')
  return {
    title: '生活里的想法',
    description: '把一些反复想起的话，按主题慢慢收起来。',
    sections: readLifeThoughtIndex(),
  }
})
