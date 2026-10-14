<template>
  <div ref="element" class="library-cover">
    <img v-if="url" :src="url" :alt="book.title + '封面'" @error="release" />
    <div v-else class="library-cover-placeholder"><span>{{ book.title }}</span><small>{{ book.author || '作者未标注' }}</small></div>
  </div>
</template>
<script setup lang="ts">
import type { LibraryBook } from '~/utils/library-reader'
const props = defineProps<{ book: LibraryBook }>()
const { request } = useLibraryAccess()
const element = ref<HTMLElement>()
const url = ref('')
const abort = new AbortController()
let observer: IntersectionObserver | undefined
function release() { if (url.value) URL.revokeObjectURL(url.value); url.value = '' }
async function load() {
  observer?.disconnect()
  try {
    const blob = await request<Blob>(`/books/${props.book.id}/cover`, 'GET', undefined, { responseType: 'blob', signal: abort.signal })
    if (!abort.signal.aborted) url.value = URL.createObjectURL(blob)
  } catch { /* Missing covers use a local text cover; authorization is handled centrally. */ }
}
onMounted(() => {
  if (!('IntersectionObserver' in window)) { void load(); return }
  observer = new IntersectionObserver(entries => { if (entries.some(e => e.isIntersecting)) void load() }, { rootMargin: '150px' })
  if (element.value) observer.observe(element.value)
})
onBeforeUnmount(() => { abort.abort(); observer?.disconnect(); release() })
</script>
