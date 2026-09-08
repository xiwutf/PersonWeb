import { describe, expect, it } from 'vitest'
import {
  apiPathToYamlPath,
  isAllowedLifeHomePath,
  isAllowedWorkHomePath,
  setYamlPath,
} from '../../server/utils/content-path-patch'

describe('content-path-patch', () => {
  it('allows life home scalar and indexed array paths', () => {
    expect(isAllowedLifeHomePath('hero.name')).toBe(true)
    expect(isAllowedLifeHomePath('hero.lines.0')).toBe(true)
    expect(isAllowedLifeHomePath('sections.now.title')).toBe(true)
    expect(isAllowedLifeHomePath('about.linkText')).toBe(true)
    expect(isAllowedLifeHomePath('closing')).toBe(true)
  })

  it('rejects traversal and unknown paths', () => {
    expect(isAllowedLifeHomePath('../secret')).toBe(false)
    expect(isAllowedLifeHomePath('hero')).toBe(false)
    expect(isAllowedLifeHomePath('sections.now')).toBe(false)
    expect(isAllowedWorkHomePath('hero.links.0.label')).toBe(false)
  })

  it('maps API camelCase path to YAML snake_case where needed', () => {
    expect(apiPathToYamlPath('work', 'hero.englishName')).toBe('hero.english_name')
    expect(apiPathToYamlPath('work', 'panel.currentSuffix')).toBe('panel.current_suffix')
    expect(apiPathToYamlPath('work', 'sections.featured.linkText')).toBe(
      'sections.featured.link_text',
    )
  })

  it('sets nested values without dropping siblings', () => {
    const doc = { hero: { name: 'A', greeting: 'Hi' }, closing: 'bye' }
    const next = setYamlPath(doc, 'hero.name', 'B')
    expect(next.hero).toEqual({ name: 'B', greeting: 'Hi' })
    expect(next.closing).toBe('bye')
  })
})
