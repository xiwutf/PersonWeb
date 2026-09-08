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
