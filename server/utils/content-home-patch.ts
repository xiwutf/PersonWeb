import {
  readLifeHome,
  readWorkHome,
  readYamlFileRawObject,
  writeYamlFile,
} from './content-files'
import {
  apiPathToYamlPath,
  isAllowedLifeHomePath,
  isAllowedWorkHomePath,
  setYamlPath,
} from './content-path-patch'

export function patchLifeHomeField(path: string, value: string) {
  if (!isAllowedLifeHomePath(path)) {
    throw new Error(`Field path not allowed: ${path}`)
  }

  const doc = readYamlFileRawObject('life', 'home.yml')
  const next = setYamlPath(doc, path, value)
  writeYamlFile(['life', 'home.yml'], next)
  return readLifeHome()
}

export function replaceLifeHomeLines(lines: string[]) {
  const cleaned = lines
    .map(line => (typeof line === 'string' ? line.trim() : ''))
    .filter(Boolean)
    .slice(0, 8)

  if (cleaned.length === 0) {
    throw new Error('hero.lines must contain at least one line')
  }

  const doc = readYamlFileRawObject('life', 'home.yml')
  const hero = doc.hero && typeof doc.hero === 'object' && !Array.isArray(doc.hero)
    ? doc.hero as Record<string, unknown>
    : {}
  hero.lines = cleaned
  if (typeof hero.current !== 'string' || !hero.current.trim()) {
    hero.current = cleaned[cleaned.length - 1]
  }
  doc.hero = hero
  writeYamlFile(['life', 'home.yml'], doc)
  return readLifeHome()
}

export function patchWorkHomeField(apiPath: string, value: string) {
  if (!isAllowedWorkHomePath(apiPath)) {
    throw new Error(`Field path not allowed: ${apiPath}`)
  }

  const yamlPath = apiPathToYamlPath('work', apiPath)
  const doc = readYamlFileRawObject('work', 'home.yml')
  const next = setYamlPath(doc, yamlPath, value)
  writeYamlFile(['work', 'home.yml'], next)
  return readWorkHome()
}
