export const parseTechStack = (raw: unknown): string[] => {
  if (!raw || typeof raw !== 'string') return []
  const s = raw.trim()
  if (s.startsWith('[')) {
    try {
      const parsed = JSON.parse(s) as unknown[]
      return parsed.filter((x): x is string => typeof x === 'string' && x.trim() !== '')
    } catch {
      // fall through to comma split
    }
  }
  return s.split(',').map(x => x.trim()).filter(Boolean)
}
