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

function contentWriteError(error: unknown): Error {
  const status = typeof error === 'object' && error !== null
    ? Number((error as { statusCode?: number, status?: number }).statusCode
      ?? (error as { statusCode?: number, status?: number }).status
      ?? 0)
    : 0

  if (status === 401) {
    return new Error('登录已失效，请重新登录后再试')
  }
  if (status === 404) {
    return new Error('当前环境无法写入摘句文件。请用本地 npm run dev 打开 localhost:3000 再操作（线上 /api 走 .NET，没有这份 Nitro 接口）')
  }
  if (status === 503) {
    const dataMessage = typeof error === 'object' && error !== null
      ? (error as { data?: { message?: string, statusMessage?: string } }).data?.message
        || (error as { data?: { statusMessage?: string } }).data?.statusMessage
        || (error as { statusMessage?: string }).statusMessage
      : undefined
    if (dataMessage && dataMessage !== 'Unable to write content file') {
      return new Error(dataMessage)
    }
    return new Error('无法写入内容文件（磁盘写入失败，请稍后重试）')
  }
  if (error instanceof Error && error.message) {
    return error
  }
  return new Error('无法写入内容文件')
}

async function putLifeContent<T>(url: string, body: unknown): Promise<T> {
  try {
    return await $fetch<T>(url, {
      method: 'PUT',
      credentials: 'include',
      body,
    })
  }
  catch (error) {
    throw contentWriteError(error)
  }
}

export function useLifeListSave() {
  async function saveNowItems(items: LifeNowItem[]) {
    return await putLifeContent('/api/content/life/now', { items })
  }

  async function saveMoments(items: LifeMoment[]) {
    return await putLifeContent('/api/content/life/moments', { items })
  }

  async function saveMargin(items: LifeMarginItem[]) {
    return await putLifeContent('/api/content/life/margin', { items })
  }

  async function createNote(payload: {
    title: string
    content: string
    description?: string
    date?: string
  }) {
    try {
      return await $fetch('/api/content/life/notes', {
        method: 'POST',
        credentials: 'include',
        body: payload,
      })
    }
    catch (error) {
      throw contentWriteError(error)
    }
  }

  async function saveHeroLines(lines: string[]) {
    return await putLifeContent('/api/content/life/home/lines', { lines })
  }

  async function saveCognition(payload: {
    title: string
    summary: string
    chapters: Array<{ title: string, body: string }>
  }) {
    return await putLifeContent('/api/content/life/cognition', payload)
  }

  return { saveNowItems, saveMoments, saveMargin, createNote, saveHeroLines, saveCognition }
}
