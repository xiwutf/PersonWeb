import { readLifeHome, readLifeNow, readMarkdownCollection, readWorkHome } from '../../utils/content-files'

export default defineEventHandler((event) => {
  setHeader(event, 'Cache-Control', 'public, max-age=60, s-maxage=60')

  const lifeHome = readLifeHome()
  const now = readLifeNow()
  const notes = readMarkdownCollection('life')
  const workHome = readWorkHome()
  const latestNote = notes[0]

  return {
    life: {
      line: lifeHome.hero.lines[0] || lifeHome.hero.current || '',
      now: now.items.slice(0, 3).map(item => ({
        title: item.title || item.category,
        description: item.description,
      })),
      note: latestNote
        ? {
            title: latestNote.title || '',
            description: latestNote.description || latestNote.summary || '',
          }
        : null,
    },
    work: {
      role: workHome.hero.role,
      focus: workHome.panel.focus.slice(0, 4),
    },
  }
})
