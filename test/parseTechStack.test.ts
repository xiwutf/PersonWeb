import { describe, it, expect } from 'vitest'
import { parseTechStack } from '../server/utils/parseTechStack'

describe('parseTechStack', () => {
  it('parses JSON array string', () => {
    expect(parseTechStack('["Vue 3", "TypeScript"]')).toEqual(['Vue 3', 'TypeScript'])
  })

  it('parses comma-separated string', () => {
    expect(parseTechStack('Vue 3, TypeScript, Nuxt')).toEqual(['Vue 3', 'TypeScript', 'Nuxt'])
  })

  it('returns empty array for null', () => {
    expect(parseTechStack(null)).toEqual([])
  })

  it('returns empty array for empty string', () => {
    expect(parseTechStack('')).toEqual([])
  })

  it('returns empty array for non-string input', () => {
    expect(parseTechStack(123)).toEqual([])
  })

  it('falls back to comma-split when JSON is malformed', () => {
    expect(parseTechStack('[malformed')).toEqual(['[malformed'])
  })

  it('filters out empty entries from comma split', () => {
    expect(parseTechStack('Vue 3,,TypeScript, ')).toEqual(['Vue 3', 'TypeScript'])
  })
})
