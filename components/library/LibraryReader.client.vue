<template>
  <section ref="root" class="library-reader" :class="{ 'library-reader-with-panel': companionOpen }" :data-reader-theme="theme">
    <div class="library-reader-main">
      <header v-show="tools" class="library-reader-toolbar">
        <NuxtLink to="/library">返回书架</NuxtLink><span class="library-reader-title">{{ title || '正在打开书籍' }}</span>
        <NButton :disabled="loading || !!error" @click="tocOpen = !tocOpen">目录</NButton>
        <NButton :disabled="loading || !!error" @click="openNotesPanel">笔记</NButton>
        <NButton :disabled="loading || turning || !!error" :title="WARM_PAPER_PRESET.name" @click="resetPreset">暖纸·幼圆</NButton>
        <NButton :disabled="loading || turning || fontSize <= 14" aria-label="减小字号" @click="resizeFont(-2)">A−</NButton>
        <span>{{ fontSize }}</span><NButton :disabled="loading || turning || fontSize >= 32" aria-label="增大字号" @click="resizeFont(2)">A+</NButton>
        <NButton :disabled="loading || turning" @click="toggleTheme">{{ theme === 'light' ? '深色' : '浅色' }}</NButton>
        <NButton v-if="canFullscreen" @click="fullscreen">全屏</NButton>
        <NButton @click="tools = false">收起</NButton>
      </header>
      <button v-show="!tools" class="library-reader-tools-toggle" aria-label="显示阅读工具" @click="tools = true">阅读工具</button>
      <div class="library-reader-sync" role="status">
        <span>{{ syncLabel }}</span>
        <NButton v-if="syncStatus === 'failed'" text @click="sync?.flush()">重试同步</NButton>
        <template v-if="syncStatus === 'conflict'">
          <span>另一设备已更新进度，本地位置已保留。</span>
          <NButton :disabled="turning" @click="resolveConflict(false)">使用服务端位置</NButton>
          <NButton :disabled="turning" @click="resolveConflict(true)">保留本设备位置</NButton>
        </template>
      </div>
      <aside v-if="tocOpen" class="library-reader-toc" aria-label="书籍目录">
        <LibraryToc :items="tocItems" @close="tocOpen = false" @jump="jump" />
      </aside>
      <div ref="viewport" class="library-reader-viewport" :aria-busy="loading" />
      <div v-if="selectionActive" class="library-reader-selection" role="status">
        <span>已选中文字</span>
        <NButton type="primary" size="small" :disabled="loading || !!error" @click="askSelection">问 AI</NButton>
        <NButton size="small" @click="clearSelection">取消</NButton>
      </div>
      <div v-if="loading || error" class="library-reader-status" role="status">
        <NSpin v-if="loading" description="正在打开正文…" />
        <template v-else><NAlert type="error">{{ error }}</NAlert><NButton @click="open">重试</NButton><NuxtLink to="/library">返回书架</NuxtLink></template>
      </div>
      <footer v-show="tools" class="library-reader-footer">
        <NButton :disabled="loading || turning || !!error || atStart" @click="turn('prev')">上一页</NButton>
        <span>{{ chapter || '阅读中' }}</span><NButton :disabled="loading || turning || !!error || atEnd" @click="turn('next')">下一页</NButton>
      </footer>
    </div>
    <LibraryCompanionPanel
      v-if="companionOpen"
      ref="companion"
      :book-id="bookId"
      :content-version="contentVersion"
      :quote="selectionQuoteText"
      :cfi-range="selectionCfi"
      :chapter-href="selectionHref"
      :jump-hint="jumpHint"
      :reader-theme="theme"
      @close="companionOpen = false"
      @jump="jumpNote"
      @saved="onNoteSaved"
    />
  </section>
</template>
<script setup lang="ts">
import { NAlert, NButton, NSpin } from 'naive-ui'
import type Book from 'epubjs/types/book'
import type Rendition from 'epubjs/types/rendition'
import type Contents from 'epubjs/types/contents'
import { flattenToc, swipeDirection, protectEpubDocument, libraryError, type LibraryResponse, type LibraryBook, type TocItem } from '~/utils/library-reader'
import { WARM_PAPER_PRESET, readingMargin, warmPaperStyles } from '~/utils/library-reading-preset'
import { ReadingProgressSync, type ReadingProgress, type SyncStatus } from '~/utils/library-progress'
import { jumpFailureMessage, selectionQuote, type ReadingNote } from '~/utils/library-companion'
const props = defineProps<{ bookId: string }>()
const { request } = useLibraryAccess()
const root = ref<HTMLElement>(), viewport = ref<HTMLElement>()
const companion = ref<{ startExplain: () => void; showNotes: () => void; reloadNotes: () => Promise<void> } | null>(null)
const loading = ref(true), error = ref(''), title = ref(''), chapter = ref('')
const tools = ref(true), tocOpen = ref(false), turning = ref(false), atStart = ref(true), atEnd = ref(false)
const fontSize = ref<number>(WARM_PAPER_PRESET.fontSize), theme = ref<'light' | 'dark'>('light'), canFullscreen = ref(false)
const toc = ref<ReturnType<typeof flattenToc>>([])
const tocItems = ref<TocItem[]>([])
const companionOpen = ref(false), selectionActive = ref(false)
const selectionQuoteText = ref(''), selectionCfi = ref(''), selectionHref = ref(''), jumpHint = ref('')
const contentVersion = ref(0)
let book: Book | undefined, rendition: Rendition | undefined, abort: AbortController | undefined
let generation = 0, disposed = false, observer: ResizeObserver | undefined
let pendingResize = false
let sync: ReadingProgressSync | undefined
let navigation: 'initial' | 'style' | 'user' | 'idle' = 'initial'
let locationsReady = false
const syncStatus = ref<SyncStatus>('idle'), progressNotice = ref('')
const syncLabel = computed(() => progressNotice.value || ({ idle: '', pending: '等待同步', saving: '同步中…', synced: '已同步', failed: '同步失败，可重试', conflict: '进度冲突', version: '书籍版本已变化，请重新打开。' })[syncStatus.value])
const cleanups = new Map<number, () => void>()
function locationCacheKey(bookId: string, version: number) {
  return `personweb.library.locations.${bookId}.${version}`
}
function progressionOf(publication: Book, cfi: string, href: string): number {
  if (locationsReady) {
    try {
      const value = publication.locations.percentageFromCfi(cfi)
      if (typeof value === 'number' && Number.isFinite(value)) return Math.max(0, Math.min(1, value))
    } catch { /* fall through to spine estimate */ }
  }
  const spine = publication.spine as unknown as { spineItems?: { href?: string; linear?: boolean }[] }
  const items = (spine.spineItems || []).filter(item => item.linear !== false)
  if (!items.length) return 0
  const index = items.findIndex(item => (item.href || '').split('#')[0] === href.split('#')[0])
  return Math.max(0, Math.min(1, ((index < 0 ? 0 : index) + 0.5) / items.length))
}
async function ensureLocations(publication: Book, bookId: string, version: number, current: number) {
  if (disposed || current !== generation || locationsReady) return
  const key = locationCacheKey(bookId, version)
  try {
    const cached = localStorage.getItem(key)
    if (cached) {
      publication.locations.load(cached)
      locationsReady = true
      return
    }
  } catch { /* private mode / quota */ }
  try {
    await publication.locations.generate(1600)
    if (disposed || current !== generation) return
    locationsReady = true
    try { localStorage.setItem(key, publication.locations.save()) }
    catch { /* ignore cache write failures */ }
  } catch {
    // Progress still saves CFI; percentage stays spine-estimated until reopen.
  }
}
async function bounded<T>(promise: Promise<T>): Promise<T> {
  let timer: ReturnType<typeof setTimeout> | undefined
  try { return await Promise.race([promise, new Promise<never>((_, reject) => { timer = setTimeout(() => reject(new Error('Reading load timed out')), 30000) })]) }
  finally { if (timer) clearTimeout(timer) }
}
function destroyBook() {
  sync?.stop(); sync = undefined
  pendingResize = false
  locationsReady = false
  abort?.abort(); observer?.disconnect(); cleanups.forEach(cleanup => cleanup()); cleanups.clear()
  rendition?.destroy(); rendition = undefined; book?.destroy(); book = undefined
  viewport.value?.replaceChildren()
  selectionActive.value = false
}
function applyTheme() {
  if (!rendition || !root.value) return
  const style = getComputedStyle(root.value)
  for (const contents of rendition.getContents() as unknown as Contents[]) {
    const sheet = (contents.document.getElementById(`epubjs-inserted-css-${WARM_PAPER_PRESET.id}`) as HTMLStyleElement | null)?.sheet
    if (sheet) for (let index = sheet.cssRules.length - 1; index >= 0; index--) sheet.deleteRule(index)
  }
  rendition.themes.register(WARM_PAPER_PRESET.id, warmPaperStyles(fontSize.value, {
    text: style.getPropertyValue('--reader-text').trim(), background: style.getPropertyValue('--reader-bg').trim(), link: style.getPropertyValue('--reader-link').trim(),
  }))
  rendition.themes.select(WARM_PAPER_PRESET.id)
  rendition.themes.fontSize(`${fontSize.value}px`)
  const view = rendition as Rendition & { manager?: { settings: { gap: number } } }
  if (view.manager && viewport.value) view.manager.settings.gap = 2 * readingMargin(viewport.value.clientWidth)
}
async function restyle(change?: () => void) {
  if (!rendition || loading.value || turning.value || disposed) return
  const view = rendition, cfi = view.location?.start?.cfi
  turning.value = true
  navigation = 'style'
  try {
    change?.(); await nextTick(); applyTheme()
    if (viewport.value) view.resize(viewport.value.clientWidth, viewport.value.clientHeight)
    await new Promise<void>(resolve => requestAnimationFrame(() => requestAnimationFrame(() => resolve())))
    if (cfi && !disposed && rendition === view) await navigate(() => view.display(cfi))
  } catch { if (!disposed) error.value = '样式更新失败，请重新打开书籍。' }
  finally { finishOperation() }
}
function finishOperation() {
  navigation = 'idle'
  turning.value = false
  if (pendingResize && !disposed) { pendingResize = false; void restyle() }
}
function toggleTheme() { void restyle(() => { theme.value = theme.value === 'light' ? 'dark' : 'light' }) }
function resizeFont(delta: number) { void restyle(() => { fontSize.value = Math.max(14, Math.min(32, fontSize.value + delta)) }) }
function resetPreset() { void restyle(() => { fontSize.value = WARM_PAPER_PRESET.fontSize; theme.value = 'light' }) }
async function turn(direction: 'next' | 'prev') {
  if (!rendition || loading.value || turning.value || error.value || (direction === 'prev' ? atStart.value : atEnd.value)) return
  turning.value = true
  navigation = 'user'
  try { await navigate(() => rendition![direction]()) } catch { error.value = '翻页失败，请重新打开书籍。' }
  finally { finishOperation() }
}
async function jump(href: string) {
  if (!rendition || turning.value) return
  tocOpen.value = false; turning.value = true
  navigation = 'user'
  try { await navigate(() => rendition!.display(href)) } catch { error.value = '这个章节无法打开，请重新加载书籍。' }
  finally { finishOperation() }
}
async function navigate(action: () => Promise<unknown>) {
  const view = rendition!
  let moved!: () => void
  const relocated = new Promise<void>(resolve => { moved = resolve; view.on('relocated', moved) })
  try { await bounded(Promise.all([action(), relocated])) }
  finally { view.off('relocated', moved) }
}
function flushProgress() { void sync?.flush(true) }
function visibility() { if (document.visibilityState === 'hidden') flushProgress() }
async function resolveConflict(local: boolean) {
  if (!sync || turning.value) return
  turning.value = true; navigation = 'initial'; progressNotice.value = ''
  try {
    if (local) await sync.chooseLocal()
    else {
      const remote = await sync.chooseRemote()
      if (remote && remote.contentVersion !== contentVersion.value) { progressNotice.value = '书籍版本已变化，请重新打开。'; sync.stop(); return }
      await navigate(() => rendition!.display(remote?.cfi))
    }
  } catch { progressNotice.value = '无法获取最新进度，请重试选择。' }
  finally { finishOperation() }
}
function keydown(event: KeyboardEvent) {
  if (event.altKey || event.ctrlKey || event.metaKey) return
  const target = event.target as HTMLElement | null
  // Only block when typing; toolbar button focus used to swallow arrow keys.
  if (target?.closest('input, textarea, select, [contenteditable="true"]')) return
  let direction: 'next' | 'prev' | null = null
  if (event.key === 'ArrowRight' || event.key === 'PageDown') direction = 'next'
  else if (event.key === 'ArrowLeft' || event.key === 'PageUp') direction = 'prev'
  else if (event.key === ' ' || event.key === 'Spacebar') direction = event.shiftKey ? 'prev' : 'next'
  else return
  if (event.shiftKey && event.key !== ' ' && event.key !== 'Spacebar') return
  event.preventDefault()
  void turn(direction)
}
function onSelected(cfiRange: string, contents: Contents) {
  const text = selectionQuote(contents.window.getSelection()?.toString() || '')
  if (!text || !cfiRange) { selectionActive.value = false; return }
  selectionQuoteText.value = text
  selectionCfi.value = cfiRange
  selectionHref.value = rendition?.location?.start?.href ?? ''
  selectionActive.value = true
  tools.value = true
}
function clearSelection() {
  selectionActive.value = false
  for (const contents of (rendition?.getContents() as unknown as Contents[] | undefined) || []) {
    contents.window.getSelection()?.removeAllRanges()
  }
}
async function askSelection() {
  if (!selectionQuoteText.value || !selectionCfi.value) return
  companionOpen.value = true
  selectionActive.value = false
  await nextTick()
  companion.value?.startExplain()
}
function openNotesPanel() {
  companionOpen.value = true
  jumpHint.value = ''
  nextTick(() => companion.value?.showNotes())
}
async function jumpNote(note: ReadingNote) {
  if (!rendition || turning.value) return
  jumpHint.value = ''
  turning.value = true
  navigation = 'user'
  try {
    await navigate(() => rendition!.display(note.cfiRange))
    companionOpen.value = true
  } catch {
    jumpHint.value = jumpFailureMessage()
  } finally {
    finishOperation()
  }
}
function onNoteSaved() {
  void companion.value?.reloadNotes()
}
function attachInteraction(contents: Contents) {
  const doc = contents.document
  const linkClicked = () => { if (!loading.value && !turning.value) navigation = 'user' }
  contents.on('linkClicked', linkClicked)
  let start: { x: number; y: number } | undefined, lastSwipe = 0
  // Do not toggle chrome on body click — that feels like accidental fullscreen. Use toolbar buttons only.
  const touchstart = (event: TouchEvent) => {
    start = event.touches.length === 1 ? { x: event.touches[0].clientX, y: event.touches[0].clientY } : undefined
  }
  const touchend = (event: TouchEvent) => {
    if (!start || !event.changedTouches.length || doc.getSelection()?.toString()) return
    const direction = swipeDirection(event.changedTouches[0].clientX - start.x, event.changedTouches[0].clientY - start.y)
    start = undefined
    if (direction) { lastSwipe = Date.now(); void turn(direction) }
  }
  doc.addEventListener('keydown', keydown)
  doc.addEventListener('touchstart', touchstart, { passive: true }); doc.addEventListener('touchend', touchend, { passive: true })
  const cleanup = () => { contents.off('linkClicked', linkClicked); doc.removeEventListener('keydown', keydown); doc.removeEventListener('touchstart', touchstart); doc.removeEventListener('touchend', touchend) }
  cleanups.get(contents.sectionIndex)?.()
  cleanups.set(contents.sectionIndex, cleanup)
}
async function open() {
  await sync?.flush()
  const current = ++generation
  destroyBook(); loading.value = true; error.value = ''; chapter.value = ''; atStart.value = true; atEnd.value = false
  abort = new AbortController()
  if (!/^[1-9]\d*$/.test(props.bookId)) { loading.value = false; error.value = '书籍编号无效，请返回书架。'; return }
  try {
    const [engine, info, bytes, saved] = await Promise.all([
      import('epubjs'),
      request<LibraryResponse<LibraryBook>>(`/books/${props.bookId}`, 'GET', undefined, { signal: abort.signal }),
      request<ArrayBuffer>(`/books/${props.bookId}/content`, 'GET', undefined, { responseType: 'arrayBuffer', signal: abort.signal }),
      request<LibraryResponse<ReadingProgress | null>>(`/books/${props.bookId}/progress`, 'GET', undefined, { signal: abort.signal }),
    ])
    if (disposed || current !== generation || !viewport.value) return
    title.value = info.data.title
    contentVersion.value = info.data.contentVersion; navigation = 'initial'; progressNotice.value = ''; syncStatus.value = 'idle'
    locationsReady = false
    sync = new ReadingProgressSync(
      async (input, keepalive) => (await request<LibraryResponse<ReadingProgress>>(`/books/${props.bookId}/progress`, 'PUT', input, { keepalive })).data,
      async () => (await request<LibraryResponse<ReadingProgress | null>>(`/books/${props.bookId}/progress`)).data,
      () => { if (sync) syncStatus.value = sync.status }, saved.data)
    const publication = engine.default({ replacements: 'blobUrl' })
    book = publication
    publication.spine.hooks.content.register(protectEpubDocument)
    await bounded(publication.open(bytes))
    await bounded(publication.ready)
    if (disposed || current !== generation) return
    tocItems.value = (await publication.loaded.navigation).toc
    toc.value = flattenToc(tocItems.value)
    // Warm location cache before first paint when available; never block first page on a full rebuild.
    try {
      const cached = localStorage.getItem(locationCacheKey(props.bookId, contentVersion.value))
      if (cached) { publication.locations.load(cached); locationsReady = true }
    } catch { /* private mode */ }
    const settings = { width: '100%', height: '100%', flow: 'paginated', spread: 'none', gap: 2 * readingMargin(viewport.value.clientWidth), allowScriptedContent: false, allowPopups: false }
    const view = publication.renderTo(viewport.value, settings)
    rendition = view
    view.hooks.content.register(attachInteraction)
    view.hooks.unloaded.register((unloaded: { section: { index: number } }) => {
      cleanups.get(unloaded.section.index)?.(); cleanups.delete(unloaded.section.index)
    })
    view.on('selected', onSelected)
    view.on('relocated', (location: { atStart: boolean; atEnd: boolean; start: { cfi: string; href: string; displayed: { page: number; total: number } } }) => {
      atStart.value = location.atStart; atEnd.value = location.atEnd
      const item = toc.value.find(item => item.href.split('#')[0] === location.start.href)
      chapter.value = `${item?.label || '本章'} ${location.start.displayed.page}/${location.start.displayed.total}`
      if (!loading.value && navigation === 'user' && location.start.cfi && !error.value) {
        progressNotice.value = ''
        sync?.record({ contentVersion: contentVersion.value, cfi: location.start.cfi, chapterHref: location.start.href,
          progression: progressionOf(publication, location.start.cfi, location.start.href) })
        navigation = 'idle'
      }
    })
    applyTheme()
    const previous = saved.data
    if (previous && previous.contentVersion !== contentVersion.value) progressNotice.value = '书籍版本已变化，未恢复旧位置。翻页后将保存当前版本。'
    try { await navigate(() => view.display(previous?.contentVersion === contentVersion.value ? previous.cfi : undefined)) }
    catch (cause) {
      if (!previous || previous.contentVersion !== contentVersion.value) throw cause
      progressNotice.value = '旧阅读位置无法恢复，尚未覆盖服务端进度。'
      await navigate(() => view.display())
    }
    if (disposed || current !== generation) return
    observer = new ResizeObserver(() => {
      if (loading.value || turning.value) pendingResize = true
      else void restyle()
    })
    observer.observe(viewport.value)
    // Build/cache character locations after first page so "正在打开正文" is not blocked.
    void ensureLocations(publication, props.bookId, contentVersion.value, current)
  } catch (cause) {
    if (!disposed && current === generation) { error.value = (cause as { response?: unknown }).response ? libraryError(cause) : '正文无法打开。这本 EPUB 可能损坏或包含暂不支持的格式。'; destroyBook() }
  } finally {
    if (!disposed && current === generation) {
      loading.value = false
      navigation = 'idle'
      if (pendingResize) { pendingResize = false; void restyle() }
    }
  }
}
async function fullscreen() {
  try { if (document.fullscreenElement) await document.exitFullscreen(); else await root.value?.requestFullscreen() }
  catch { /* Fullscreen is optional; the immersive reader remains available. */ }
}
onMounted(() => { canFullscreen.value = !!document.fullscreenEnabled; document.addEventListener('keydown', keydown); document.addEventListener('visibilitychange', visibility); window.addEventListener('pagehide', flushProgress); void open() })
onBeforeUnmount(() => { flushProgress(); disposed = true; generation++; destroyBook(); document.removeEventListener('keydown', keydown); document.removeEventListener('visibilitychange', visibility); window.removeEventListener('pagehide', flushProgress) })
</script>
