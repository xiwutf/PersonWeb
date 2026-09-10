type LifeNowItem = {
  title: string
  description: string
  href?: string
  icon?: string
}

type LifeMoment = {
  date: string
  content: string
  image?: string
  note?: string
}

type LifeMarginItem = {
  text: string
  note?: string
  group?: string
  featured?: boolean
  archived?: boolean
  tone?: 'plain' | 'strong' | 'quiet' | 'curious' | 'passing'
  sourceImage?: string
  scanBatch?: string
}

export function useLifeListSave() {
  async function saveNowItems(items: LifeNowItem[]) {
    return await $fetch('/api/content/life/now', {
      method: 'PUT',
      credentials: 'include',
      body: { items },
    })
  }

  async function saveMoments(items: LifeMoment[]) {
    return await $fetch('/api/content/life/moments', {
      method: 'PUT',
      credentials: 'include',
      body: { items },
    })
  }

  async function saveMargin(items: LifeMarginItem[]) {
    return await $fetch('/api/content/life/margin', {
      method: 'PUT',
      credentials: 'include',
      body: { items },
    })
  }

  async function createNote(payload: {
    title: string
    content: string
    description?: string
    date?: string
  }) {
    return await $fetch('/api/content/life/notes', {
      method: 'POST',
      credentials: 'include',
      body: payload,
    })
  }

  async function saveHeroLines(lines: string[]) {
    return await $fetch('/api/content/life/home/lines', {
      method: 'PUT',
      credentials: 'include',
      body: { lines },
    })
  }

  return { saveNowItems, saveMoments, saveMargin, createNote, saveHeroLines }
}
