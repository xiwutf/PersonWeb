<template>
  <aside ref="panelRoot" class="library-companion" :data-panel-theme="readerTheme" aria-label="AI 阅读助手">
    <NConfigProvider :theme="readerTheme === 'dark' ? darkTheme : null" :theme-overrides="panelTheme" class="library-companion-config">
    <header class="library-companion-header">
      <div class="library-companion-heading"><div><h2>阅读伴读</h2><p>理解原文，留下自己的想法</p></div><NButton quaternary circle aria-label="关闭助手" title="关闭助手" @click="emit('close')"><span class="library-companion-close" aria-hidden="true">×</span></NButton></div>
      <div class="library-companion-tabs">
        <NButton quaternary :class="{ 'is-active': tab === 'ai' }" @click="tab = 'ai'">问 AI</NButton>
        <NButton quaternary :class="{ 'is-active': tab === 'notes' }" @click="openNotes">我的笔记</NButton>
      </div>
    </header>

    <section v-if="tab === 'ai'" class="library-companion-body">
      <div class="library-companion-block">
        <h3>原文摘录</h3>
        <p class="library-companion-quote">{{ quote || '请先在正文中选中一段文字。' }}</p>
        <p v-if="quote && context && context !== quote" class="library-companion-context">
          <span>参考上下文</span>{{ context }}
        </p>
      </div>
      <div class="library-companion-block">
        <div class="library-companion-block-head">
          <h3>AI 解释</h3>
          <NButton v-if="latestAssistant" size="tiny" :disabled="!latestAssistant" @click="copyAssistant">复制回答</NButton>
        </div>
        <NSpin v-if="asking" size="small" description="正在解释…" />
        <NAlert v-else-if="askError" type="warning">{{ askError }}</NAlert>
        <ul v-else-if="turns.length" class="library-companion-turns">
          <li v-for="(turn, index) in turns" :key="index" :data-role="turn.role">
            <span>{{ turn.role === 'user' ? '你' : 'AI' }}</span>
            <p>{{ turn.content }}</p>
          </li>
        </ul>
        <p v-else class="library-companion-muted">选中文字后点「问 AI」，解释只保留在本面板，不会写入笔记。</p>
      </div>
      <form class="library-companion-ask" @submit.prevent="followUp">
        <NInput v-model:value="question" type="textarea" :rows="2" :disabled="!quote || asking" placeholder="继续追问…" maxlength="2000" show-count />
        <NButton attr-type="submit" type="primary" class="library-companion-send" :disabled="!quote || !question.trim() || asking" :loading="asking">发送追问</NButton>
      </form>
      <div class="library-companion-block library-companion-writing">
        <h3>写下想法</h3>
        <p class="library-companion-muted">只保存你自己的笔记与原文位置；AI 回答不会一并入库。</p>
        <NInput v-model:value="draftNote" type="textarea" :rows="3" :disabled="!quote" placeholder="写下你的理解或摘录备注…" maxlength="20000" show-count />
        <NAlert v-if="saveError" type="error" class="library-companion-alert">{{ saveError }}</NAlert>
        <NAlert v-if="saveOk" type="success" class="library-companion-alert">笔记已保存。</NAlert>
        <NButton type="primary" :disabled="!quote || !draftNote.trim() || !cfiRange || saving" :loading="saving" @click="saveNote">保存笔记</NButton>
      </div>
    </section>

    <section v-else class="library-companion-body">
      <NSpin v-if="notesLoading" size="small" description="加载笔记…" />
      <NAlert v-else-if="notesError" type="error">{{ notesError }}</NAlert>
      <p v-else-if="!notes.length" class="library-companion-muted">还没有笔记。选中原文、写完自己的想法后即可保存。</p>
      <template v-else>
        <div v-if="focusedNote" class="library-companion-block library-companion-focused-note">
          <div class="library-companion-block-head">
            <h3>这条笔记</h3>
            <NButton size="tiny" @click="emit('jump', focusedNote)">跳转原文</NButton>
          </div>
          <p class="library-companion-quote">{{ focusedNote.quote }}</p>
          <p class="library-companion-focused-body">{{ focusedNote.note }}</p>
          <small class="library-companion-muted">{{ formatTime(focusedNote.createdAt) }}</small>
        </div>
        <ul class="library-companion-notes">
          <li v-for="item in notes" :key="item.id">
            <button type="button" class="library-companion-note" :class="{ 'is-focused': focusedNoteId === item.id }" :data-note-id="item.id" @click="selectNote(item)">
              <strong>{{ notePreview(item.note) }}</strong>
              <span>{{ notePreview(item.quote, 64) }}</span>
              <small>{{ formatTime(item.createdAt) }}</small>
            </button>
          </li>
        </ul>
      </template>
      <NAlert v-if="jumpHint" type="warning" class="library-companion-alert">{{ jumpHint }}</NAlert>
    </section>
    </NConfigProvider>
  </aside>
</template>

<script setup lang="ts">
import { darkTheme, NAlert, NButton, NConfigProvider, NInput, NSpin, type GlobalThemeOverrides } from 'naive-ui'
import {
  buildExplainHistory,
  companionError,
  notePreview,
  type CompanionTurn,
  type ReadingNote,
} from '~/utils/library-companion'
import type { LibraryResponse } from '~/utils/library-reader'

const props = defineProps<{
  bookId: string
  contentVersion: number
  quote: string
  context?: string
  cfiRange: string
  chapterHref: string
  jumpHint: string
  readerTheme?: 'light' | 'dark'
}>()
const panelRoot = ref<HTMLElement>()
const panelTheme = ref<GlobalThemeOverrides>({})
async function updatePanelTheme() {
  await nextTick()
  if (!panelRoot.value) return
  const style = getComputedStyle(panelRoot.value)
  const token = (name: string) => style.getPropertyValue(`--companion-${name}`).trim()
  panelTheme.value = {
    common: { primaryColor: token('accent'), primaryColorHover: token('accent-hover'), primaryColorPressed: token('accent-hover'), primaryColorSuppl: token('accent'),
      textColorBase: token('text'), textColor1: token('text'), textColor2: token('text'), textColor3: token('muted'), borderColor: token('border'), borderRadius: token('radius') },
    Input: { color: token('input'), colorFocus: token('input'), textColor: token('text'), placeholderColor: token('muted'), countTextColor: token('muted'),
      border: `1px solid ${token('border')}`, borderHover: `1px solid ${token('accent')}`, borderFocus: `1px solid ${token('accent')}`, boxShadowFocus: 'none' },
    Button: { textColorPrimary: token('on-accent'), textColorHoverPrimary: token('on-accent'), textColorPressedPrimary: token('on-accent'), textColorFocusPrimary: token('on-accent'),
      textColorDisabledPrimary: token('muted'), colorDisabledPrimary: token('soft'), borderDisabledPrimary: `1px solid ${token('border')}`, opacityDisabled: '1' },
  }
}
onMounted(updatePanelTheme)
watch(() => props.readerTheme, updatePanelTheme)
const emit = defineEmits<{ close: []; jump: [ReadingNote]; saved: [] }>()
const { request } = useLibraryAccess()

const tab = ref<'ai' | 'notes'>('ai')
const turns = ref<CompanionTurn[]>([])
const question = ref('')
const draftNote = ref('')
const asking = ref(false)
const askError = ref('')
const saving = ref(false)
const saveError = ref('')
const saveOk = ref(false)
const notes = ref<ReadingNote[]>([])
const notesLoading = ref(false)
const notesError = ref('')
const focusedNoteId = ref<number | null>(null)
const focusedNote = computed(() => notes.value.find(item => item.id === focusedNoteId.value) || null)
let askGeneration = 0

const latestAssistant = computed(() => {
  for (let index = turns.value.length - 1; index >= 0; index--) {
    if (turns.value[index].role === 'assistant') return turns.value[index].content
  }
  return ''
})

watch(() => [props.quote, props.cfiRange, props.context] as const, () => {
  turns.value = []
  question.value = ''
  draftNote.value = ''
  askError.value = ''
  saveError.value = ''
  saveOk.value = false
  askGeneration++
})

async function ask(initial = false, followUpText = '') {
  if (!props.quote || asking.value) return
  const text = followUpText.trim() || (initial ? '请解释这段原文的含义，并说明关键词语或难句。' : '')
  if (!text) return
  const current = ++askGeneration
  asking.value = true
  askError.value = ''
  const history = initial ? [] : buildExplainHistory(turns.value)
  if (!initial) turns.value = [...turns.value, { role: 'user', content: text }]
  try {
    const response = await request<LibraryResponse<{ reply: string }>>('/ai/explain', 'POST', {
      quote: props.quote,
      context: props.context && props.context !== props.quote ? props.context : '',
      question: text,
      history,
    })
    if (current !== askGeneration) return
    turns.value = [...turns.value, { role: 'assistant', content: response.data.reply }]
  } catch (cause) {
    if (current !== askGeneration) return
    askError.value = companionError(cause)
    if (!initial) turns.value = turns.value.slice(0, -1)
  } finally {
    if (current === askGeneration) asking.value = false
  }
}

function followUp() {
  const text = question.value.trim()
  if (!text) return
  question.value = ''
  void ask(false, text)
}

async function copyAssistant() {
  if (!latestAssistant.value || !navigator.clipboard?.writeText) return
  try { await navigator.clipboard.writeText(latestAssistant.value) }
  catch { askError.value = '复制失败，请手动选中文字复制。' }
}

async function saveNote() {
  if (!props.quote || !props.cfiRange || !draftNote.value.trim() || saving.value) return
  saving.value = true
  saveError.value = ''
  saveOk.value = false
  try {
    await request<LibraryResponse<ReadingNote>>(`/books/${props.bookId}/annotations`, 'POST', {
      contentVersion: props.contentVersion,
      cfiRange: props.cfiRange,
      chapterHref: props.chapterHref,
      quote: props.quote,
      note: draftNote.value.trim(),
      kind: 'note',
      color: 'yellow',
      requestId: crypto.randomUUID(),
    })
    saveOk.value = true
    draftNote.value = ''
    emit('saved')
  } catch (cause) {
    saveError.value = companionError(cause)
  } finally {
    saving.value = false
  }
}

async function loadNotes() {
  notesLoading.value = true
  notesError.value = ''
  try {
    const response = await request<LibraryResponse<ReadingNote[]>>(`/books/${props.bookId}/annotations`)
    notes.value = response.data || []
  } catch (cause) {
    notesError.value = companionError(cause)
  } finally {
    notesLoading.value = false
  }
}

function openNotes() {
  tab.value = 'notes'
  void loadNotes()
}

function selectNote(item: ReadingNote) {
  focusedNoteId.value = item.id
  emit('jump', item)
}

function formatTime(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString()
}

async function focusNote(noteId: number) {
  tab.value = 'notes'
  focusedNoteId.value = noteId
  await loadNotes()
  focusedNoteId.value = noteId
  await nextTick()
  const node = panelRoot.value?.querySelector(`[data-note-id="${noteId}"]`) as HTMLElement | null
  node?.scrollIntoView({ block: 'nearest', behavior: 'smooth' })
}

defineExpose({
  startExplain() {
    tab.value = 'ai'
    void ask(true)
  },
  showNotes() {
    openNotes()
  },
  reloadNotes: loadNotes,
  focusNote,
})
</script>
