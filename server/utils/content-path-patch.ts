/**
 * Field-level YAML path helpers for inline copy edit (Life/Work home).
 * Only whitelisted leaf paths may be written.
 */

const LIFE_HOME_PATHS = new Set([
  'hero.kicker',
  'hero.greeting',
  'hero.name',
  'hero.current',
  'closing',
  'empty.moments',
  'empty.notes',
  'about.description',
  'about.linkText',
  'sections.now.title',
  'sections.moments.title',
  'sections.notes.title',
  'sections.about.title',
])

const LIFE_HOME_INDEXED = [/^hero\.lines\.(\d+)$/]

const WORK_SECTION_KEYS = ['featured', 'more', 'capabilities', 'contact'] as const

const WORK_HOME_PATHS = new Set([
  'brand.name',
  'brand.tagline',
  'hero.kicker',
  'hero.title',
  'hero.english_name',
  'hero.role',
  'hero.value',
  'panel.current_suffix',
  'footer.name',
  'footer.description',
  'seo.title',
  'seo.description',
])

for (const key of WORK_SECTION_KEYS) {
  WORK_HOME_PATHS.add(`sections.${key}.number`)
  WORK_HOME_PATHS.add(`sections.${key}.label`)
  WORK_HOME_PATHS.add(`sections.${key}.title`)
  WORK_HOME_PATHS.add(`sections.${key}.description`)
  WORK_HOME_PATHS.add(`sections.${key}.link_text`)
}

const WORK_HOME_INDEXED = [
  /^hero\.actions\.(\d+)\.label$/,
  /^panel\.focus\.(\d+)$/,
  /^panel\.status\.(\d+)$/,
  /^panel\.tools\.(\d+)$/,
]

const hasUnsafePathChars = (path: string) =>
  path.includes('..') || path.includes('/') || path.includes('\\')

export function apiPathToYamlPath(world: 'life' | 'work', apiPath: string): string {
  if (world !== 'work') {
    return apiPath
  }

  if (apiPath === 'hero.englishName') return 'hero.english_name'
  if (apiPath === 'panel.currentSuffix') return 'panel.current_suffix'

  const linkTextMatch = apiPath.match(
    /^sections\.(featured|more|capabilities|contact)\.linkText$/,
  )
  if (linkTextMatch) {
    return `sections.${linkTextMatch[1]}.link_text`
  }

  return apiPath
}

export function isAllowedLifeHomePath(path: string): boolean {
  if (!path || hasUnsafePathChars(path)) return false
  if (LIFE_HOME_PATHS.has(path)) return true
  return LIFE_HOME_INDEXED.some(re => re.test(path))
}

export function isAllowedWorkHomePath(path: string): boolean {
  if (!path || hasUnsafePathChars(path)) return false
  const yamlPath = apiPathToYamlPath('work', path)
  if (WORK_HOME_PATHS.has(yamlPath)) return true
  return WORK_HOME_INDEXED.some(re => re.test(yamlPath))
}

export function setYamlPath(
  doc: Record<string, unknown>,
  path: string,
  value: string,
): Record<string, unknown> {
  const parts = path.split('.').filter(Boolean)
  if (parts.length === 0) {
    throw new Error('Empty path')
  }

  const root = structuredClone(doc) as Record<string, unknown>
  let cursor: Record<string, unknown> | unknown[] = root

  for (let i = 0; i < parts.length - 1; i++) {
    const key = parts[i]
    const nextKey = parts[i + 1]
    const isIndex = /^\d+$/.test(key)
    const nextIsIndex = /^\d+$/.test(nextKey)

    if (isIndex) {
      const index = Number(key)
      if (!Array.isArray(cursor)) {
        throw new Error(`Expected array at ${parts.slice(0, i).join('.')}`)
      }
      if (typeof cursor[index] !== 'object' || cursor[index] === null) {
        cursor[index] = nextIsIndex ? [] : {}
      }
      cursor = cursor[index] as Record<string, unknown> | unknown[]
    } else {
      if (Array.isArray(cursor) || typeof cursor !== 'object' || cursor === null) {
        throw new Error(`Expected object at ${parts.slice(0, i).join('.')}`)
      }
      const obj = cursor as Record<string, unknown>
      if (typeof obj[key] !== 'object' || obj[key] === null) {
        obj[key] = nextIsIndex ? [] : {}
      }
      cursor = obj[key] as Record<string, unknown> | unknown[]
    }
  }

  const last = parts[parts.length - 1]
  if (/^\d+$/.test(last)) {
    if (!Array.isArray(cursor)) {
      throw new Error(`Expected array at parent of ${path}`)
    }
    cursor[Number(last)] = value
  } else {
    if (Array.isArray(cursor) || typeof cursor !== 'object' || cursor === null) {
      throw new Error(`Expected object at parent of ${path}`)
    }
    ;(cursor as Record<string, unknown>)[last] = value
  }

  return root
}
