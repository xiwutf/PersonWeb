# Inline Copy Edit (P0+P1) Implementation Plan

> **For agentic workers:** Execute task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. Do **not** git commit unless the user explicitly asks.

**Goal:** Let an authenticated admin click-to-edit Life/Work home YAML copy on `/life` and `/work`; guests remain read-only.

**Architecture:** Reuse `admin_token` + `checkAuth`. Add field-level PATCH handlers that whitelist paths and write `content/{life|work}/home.yml`. Frontend: `useAdminSession` + `InlineEditableText` wired only when `isAdmin`.

**Tech Stack:** Nuxt 3 / Nitro, `yaml` package, Vitest, existing auth utilities.

**Spec:** `docs/superpowers/specs/2026-09-04-inline-copy-edit-design.md`

---

## File map

| File | Responsibility |
| --- | --- |
| `server/utils/content-path-patch.ts` | Whitelist matchers, set-by-path, shared patch helper |
| `server/utils/content-files.ts` | Add `writeYamlFile` |
| `server/api/content/life/home.patch.ts` | Auth + Life home patch |
| `server/api/content/work/home.patch.ts` | Auth + Work home patch |
| `composables/useAdminSession.ts` | Client session probe (`isAdmin`) |
| `composables/useInlineCopySave.ts` | PATCH helper + local state update |
| `components/content/InlineEditableText.vue` | Click-to-edit text |
| `components/content/AdminEditBanner.vue` | “已登录 · 点击文案可编辑” |
| `assets/css/inline-edit.css` | Token-based edit chrome |
| `pages/life/index.vue` | Wire editable fields |
| `pages/work.vue` | Wire editable fields |
| `nuxt.config.ts` | Import `inline-edit.css` if not auto |
| `test/server/content-path-patch.test.ts` | Unit tests for path/whitelist |
| `test/server/content-home-patch.test.ts` | Auth + file write behavior |
| `docs/CONTENT_ARCHITECTURE.md` | Admin write path note |
| `docs/PROJECT_STRUCTURE_GUIDE.md` | Edit location note |

---

### Task 1: Path patch utilities + writeYamlFile

**Files:**
- Create: `server/utils/content-path-patch.ts`
- Modify: `server/utils/content-files.ts`
- Test: `test/server/content-path-patch.test.ts`

- [ ] **Step 1: Write failing unit tests**

```ts
import { describe, expect, it } from 'vitest'
import {
  isAllowedLifeHomePath,
  isAllowedWorkHomePath,
  setYamlPath,
  apiPathToYamlPath,
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
    expect(apiPathToYamlPath('work', 'sections.featured.linkText')).toBe('sections.featured.link_text')
  })

  it('sets nested values without dropping siblings', () => {
    const doc = { hero: { name: 'A', greeting: 'Hi' }, closing: 'bye' }
    const next = setYamlPath(doc, 'hero.name', 'B')
    expect(next.hero.name).toBe('B')
    expect(next.hero.greeting).toBe('Hi')
    expect(next.closing).toBe('bye')
  })
})
```

- [ ] **Step 2: Run test — expect FAIL (module missing)**

Run: `npx vitest run test/server/content-path-patch.test.ts`

- [ ] **Step 3: Implement utilities**

`server/utils/content-path-patch.ts`:

```ts
const LIFE_HOME_PATHS = new Set([
  'hero.kicker', 'hero.greeting', 'hero.name', 'hero.current', 'closing',
  'empty.moments', 'empty.notes',
  'about.description', 'about.linkText',
  'sections.now.title', 'sections.moments.title', 'sections.notes.title', 'sections.about.title',
])

const LIFE_HOME_INDEXED = [
  /^hero\.lines\.(\d+)$/,
]

const WORK_HOME_PATHS = new Set([
  'brand.name', 'brand.tagline',
  'hero.kicker', 'hero.title', 'hero.english_name', 'hero.role', 'hero.value',
  'panel.current_suffix',
  'footer.name', 'footer.description',
  'seo.title', 'seo.description',
])

const WORK_SECTION_KEYS = ['featured', 'more', 'capabilities', 'contact'] as const
for (const key of WORK_SECTION_KEYS) {
  WORK_HOME_PATHS.add(`sections.${key}.number`)
  WORK_HOME_PATHS.add(`sections.${key}.label`)
  WORK_HOME_PATHS.add(`sections.${key}.title`)
  WORK_HOME_PATHS.add(`sections.${key}.description`)
  WORK_HOME_PATHS.add(`sections.${key}.link_text`)
  // link_to stays read-only in P1
}

const WORK_HOME_INDEXED = [
  /^hero\.actions\.(\d+)\.label$/,
  /^panel\.focus\.(\d+)$/,
  /^panel\.status\.(\d+)$/,
  /^panel\.tools\.(\d+)$/,
]

const API_TO_YAML: Record<string, string> = {
  'hero.englishName': 'hero.english_name',
  'panel.currentSuffix': 'panel.current_suffix',
  'sections.featured.linkText': 'sections.featured.link_text',
  'sections.more.linkText': 'sections.more.link_text',
  'sections.capabilities.linkText': 'sections.capabilities.link_text',
  'sections.contact.linkText': 'sections.contact.link_text',
  // also map linkTo if ever allowed later
}

export function apiPathToYamlPath(world: 'life' | 'work', apiPath: string): string {
  if (world === 'work' && API_TO_YAML[apiPath]) return API_TO_YAML[apiPath]
  // sections.*.linkText dynamic
  const m = apiPath.match(/^sections\.(featured|more|capabilities|contact)\.linkText$/)
  if (world === 'work' && m) return `sections.${m[1]}.link_text`
  if (apiPath === 'hero.englishName') return 'hero.english_name'
  if (apiPath === 'panel.currentSuffix') return 'panel.current_suffix'
  return apiPath
}

export function isAllowedLifeHomePath(path: string): boolean {
  if (path.includes('..') || path.includes('/') || path.includes('\\')) return false
  if (LIFE_HOME_PATHS.has(path)) return true
  return LIFE_HOME_INDEXED.some(re => re.test(path))
}

export function isAllowedWorkHomePath(path: string): boolean {
  if (path.includes('..') || path.includes('/') || path.includes('\\')) return false
  const yamlPath = apiPathToYamlPath('work', path)
  if (WORK_HOME_PATHS.has(yamlPath)) return true
  return WORK_HOME_INDEXED.some(re => re.test(yamlPath))
}

export function setYamlPath(doc: Record<string, unknown>, path: string, value: string): Record<string, unknown> {
  const parts = path.split('.')
  const root = structuredClone(doc)
  let cursor: any = root
  for (let i = 0; i < parts.length - 1; i++) {
    const key = parts[i]
    const nextKey = parts[i + 1]
    const index = /^\d+$/.test(key) ? Number(key) : key
    if (typeof cursor[index] !== 'object' || cursor[index] === null) {
      cursor[index] = /^\d+$/.test(nextKey) ? [] : {}
    }
    cursor = cursor[index]
  }
  const last = parts[parts.length - 1]
  const lastKey = /^\d+$/.test(last) ? Number(last) : last
  cursor[lastKey] = value
  return root
}
```

In `content-files.ts`, add:

```ts
import { stringify as stringifyYaml } from 'yaml'

export const writeYamlFile = (segments: string[], data: unknown) => {
  const fullPath = path.join(contentRoot, ...segments)
  // only allow known relative segments under contentRoot
  const resolved = path.resolve(fullPath)
  if (!resolved.startsWith(path.resolve(contentRoot) + path.sep) && resolved !== path.resolve(contentRoot)) {
    throw new Error('Refusing to write outside content root')
  }
  const body = stringifyYaml(data, { lineWidth: 0 })
  fs.writeFileSync(resolved, body.endsWith('\n') ? body : `${body}\n`, 'utf-8')
}

export const readYamlFileRawObject = (...segments: string[]) => {
  const parsed = readYamlFile(...segments)
  if (!parsed || typeof parsed !== 'object' || Array.isArray(parsed)) {
    return {} as Record<string, unknown>
  }
  return parsed as Record<string, unknown>
}
```

Also export a shared patch runner in `content-path-patch.ts` or a small `patch-home-yaml.ts` used by both routes.

- [ ] **Step 4: Re-run tests — expect PASS**

---

### Task 2: Life + Work PATCH API

**Files:**
- Create: `server/api/content/life/home.patch.ts`
- Create: `server/api/content/work/home.patch.ts`
- Test: `test/server/content-home-patch.test.ts`

- [ ] **Step 1: Write tests for whitelist + auth gate (unit-level helpers)**

Test the patch helper function directly (prefer extracting `patchLifeHomeField` / `patchWorkHomeField` so tests don't need full Nitro):

```ts
import { describe, expect, it, beforeEach, afterEach } from 'vitest'
import { readFileSync, writeFileSync, copyFileSync } from 'node:fs'
import { resolve } from 'node:path'
import { patchLifeHomeField, patchWorkHomeField } from '../../server/utils/content-home-patch'
import { readLifeHome, readWorkHome } from '../../server/utils/content-files'

const lifeHome = resolve(__dirname, '../../content/life/home.yml')
const workHome = resolve(__dirname, '../../content/work/home.yml')
let lifeBackup = ''
let workBackup = ''

beforeEach(() => {
  lifeBackup = readFileSync(lifeHome, 'utf8')
  workBackup = readFileSync(workHome, 'utf8')
})
afterEach(() => {
  writeFileSync(lifeHome, lifeBackup, 'utf8')
  writeFileSync(workHome, workBackup, 'utf8')
})

it('patches life hero.name and returns reader shape', () => {
  const result = patchLifeHomeField('hero.name', '测试名')
  expect(result.hero.name).toBe('测试名')
  expect(readLifeHome().hero.name).toBe('测试名')
})

it('rejects illegal life path', () => {
  expect(() => patchLifeHomeField('hero', 'x')).toThrow(/not allowed/i)
})
```

- [ ] **Step 2: Implement `server/utils/content-home-patch.ts` + route handlers**

Handlers:

```ts
import { checkAuth } from '../../../utils/auth'
import { patchLifeHomeField } from '../../../utils/content-home-patch'

export default defineEventHandler(async (event) => {
  checkAuth(event)
  const body = await readBody(event)
  const path = typeof body?.path === 'string' ? body.path.trim() : ''
  const value = typeof body?.value === 'string' ? body.value : null
  if (!path || value === null) {
    throw createError({ statusCode: 400, statusMessage: 'path and string value required' })
  }
  try {
    const data = patchLifeHomeField(path, value)
    setHeader(event, 'Cache-Control', 'no-store')
    return data
  } catch (error: any) {
    if (String(error?.message || '').includes('not allowed')) {
      throw createError({ statusCode: 400, statusMessage: 'Field path not allowed' })
    }
    throw createError({ statusCode: 503, statusMessage: 'Unable to write content file' })
  }
})
```

Same for work with `patchWorkHomeField` (apply `apiPathToYamlPath` inside).

- [ ] **Step 3: Run tests — expect PASS**

Also add a small test that route source files contain `checkAuth(event)`.

---

### Task 3: useAdminSession + save helper + CSS + components

**Files:**
- Create: `composables/useAdminSession.ts`
- Create: `composables/useInlineCopySave.ts`
- Create: `components/content/InlineEditableText.vue`
- Create: `components/content/AdminEditBanner.vue`
- Create: `assets/css/inline-edit.css`
- Modify: `nuxt.config.ts` (add CSS if needed)

- [ ] **Step 1: `useAdminSession`**

```ts
export function useAdminSession() {
  const isAdmin = useState('admin-session-is-admin', () => false)
  const pending = useState('admin-session-pending', () => true)
  const loaded = useState('admin-session-loaded', () => false)

  async function refresh() {
    if (!import.meta.client) {
      pending.value = false
      return
    }
    pending.value = true
    try {
      const session = await $fetch<{ authenticated: boolean }>('/api/auth/session', {
        credentials: 'include',
      })
      isAdmin.value = Boolean(session?.authenticated)
    } catch {
      isAdmin.value = false
    } finally {
      pending.value = false
      loaded.value = true
    }
  }

  if (import.meta.client && !loaded.value) {
    refresh()
  }

  return { isAdmin, pending, refresh }
}
```

- [ ] **Step 2: `useInlineCopySave`**

```ts
export function useInlineCopySave(endpoint: '/api/content/life/home' | '/api/content/work/home') {
  async function saveField(path: string, value: string) {
    return await $fetch(endpoint, {
      method: 'PATCH',
      credentials: 'include',
      body: { path, value },
    })
  }
  return { saveField }
}
```

- [ ] **Step 3: `InlineEditableText.vue`**

Props: `modelValue: string`, `fieldPath: string`, `as?: string` default `span`, `multiline?: boolean`, `save: (path, value) => Promise<unknown>`.

Use `useAdminSession().isAdmin`. When false, render `<component :is="as">{{ modelValue }}</component>` only. When true, click → input/textarea, blur/Enter save, Esc cancel. Emit `update:modelValue` on success.

Styles in `assets/css/inline-edit.css` using `--color-border-default`, `--color-primary-soft`, `--radius-sm`.

- [ ] **Step 4: `AdminEditBanner.vue`**

Show only if `isAdmin`: text「已登录 · 点击文案可编辑」.

- [ ] **Step 5: Register CSS in `nuxt.config.ts` css array**

---

### Task 4: Wire `/life`

**Files:**
- Modify: `pages/life/index.vue`

- [ ] Replace static hero/section/empty/about/closing text nodes with `InlineEditableText` bound to reactive `home` (convert `home` to `ref`/`reactive` copy from `homeData` so local updates stick).

- [ ] Mount `AdminEditBanner` near top of `.life-home`.

- [ ] `save` calls Life endpoint; on success merge returned payload into local home state.

Example pattern:

```vue
<InlineEditableText
  v-model="home.hero.name"
  field-path="hero.name"
  as="h1"
  :save="saveLifeField"
/>
```

For `hero.lines`, `v-for="(line, index) in home.hero.lines"` with `field-path="\`hero.lines.${index}\`"`.

---

### Task 5: Wire `/work`

**Files:**
- Modify: `pages/work.vue`

- [ ] Same pattern for brand/hero/panel/sections/footer strings listed in spec P1.

- [ ] Mount `AdminEditBanner`.

- [ ] Do **not** wrap `hero.links` or DB project titles.

---

### Task 6: Docs + smoke tests

**Files:**
- Modify: `docs/CONTENT_ARCHITECTURE.md`
- Modify: `docs/PROJECT_STRUCTURE_GUIDE.md`
- Modify: `docs/superpowers/specs/2026-09-04-inline-copy-edit-design.md` status → 已确认

- [ ] Update Work Display copy Admin row: `前台 InlineEditableText → PATCH /api/content/work/home → content/work/home.yml`（需登录）。

- [ ] Update 内容编辑位置 table with 前台登录编辑.

- [ ] Run: `npx vitest run test/server/content-path-patch.test.ts test/server/content-home-patch.test.ts test/content/work-content-sot.test.ts`

- [ ] Manual: login at `/admin/login`, open `/life`, edit `hero.name`, refresh — value persists; logout — no edit chrome.

---

## Spec coverage checklist

| Spec item | Task |
| --- | --- |
| checkAuth on write | Task 2 |
| Whitelist paths | Task 1–2 |
| Life + Work home only (P0+P1) | Task 4–5 |
| Inline click-to-edit | Task 3–5 |
| Guest identical | Task 3 (`isAdmin` false branch) |
| Banner | Task 3–5 |
| Docs | Task 6 |
| P2 files | Out of scope |

## Notes for executor

- Prefer restoring YAML fixtures in tests (`afterEach`).
- Do not commit unless user asks.
- Do not run full production build (user preference).
- Style tokens only — no hardcoded colors.
