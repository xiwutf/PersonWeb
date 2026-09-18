<template>
  <div class="ci-reaction">
    <button
      type="button"
      class="ci-reaction-btn"
      :class="{ 'is-liked': liked }"
      :disabled="busy"
      :aria-pressed="liked"
      @click="toggle"
    >
      <span class="ci-reaction-heart" aria-hidden="true">{{ liked ? '♥' : '♡' }}</span>
      <span>{{ likeCount }} 人觉得有启发</span>
    </button>
  </div>
</template>

<script setup lang="ts">
const props = defineProps<{
  targetType: string
  targetId: string
}>()

const api = useApi()
const { getVisitorId } = useVisitorId()

const likeCount = ref(0)
const liked = ref(false)
const busy = ref(false)
const loaded = ref(false)

const load = async () => {
  if (!props.targetType || !props.targetId) return
  try {
    const visitorId = getVisitorId()
    const data = await api.get<{
      likeCount?: number
      liked?: boolean
      LikeCount?: number
      Liked?: boolean
    }>(`/interactions/${encodeURIComponent(props.targetType)}/${encodeURIComponent(props.targetId)}`, {
      query: { visitorId },
      silent: true,
    })
    likeCount.value = Number(data?.likeCount ?? data?.LikeCount ?? 0)
    liked.value = Boolean(data?.liked ?? data?.Liked)
    loaded.value = true
  } catch {
    loaded.value = true
  }
}

const toggle = async () => {
  if (busy.value || !props.targetId) return
  busy.value = true
  const visitorId = getVisitorId()
  const prevLiked = liked.value
  const prevCount = likeCount.value

  // 乐观更新
  liked.value = !prevLiked
  likeCount.value = Math.max(0, prevCount + (prevLiked ? -1 : 1))

  try {
    if (prevLiked) {
      const data = await api.del<{ likeCount?: number, LikeCount?: number }>(
        `/interactions/${encodeURIComponent(props.targetType)}/${encodeURIComponent(props.targetId)}/like`,
        { query: { visitorId }, silent: true },
      )
      likeCount.value = Number(data?.likeCount ?? data?.LikeCount ?? likeCount.value)
      liked.value = false
    } else {
      const data = await api.post<{ likeCount?: number, LikeCount?: number }>(
        `/interactions/${encodeURIComponent(props.targetType)}/${encodeURIComponent(props.targetId)}/like`,
        { visitorId },
        { silent: true },
      )
      likeCount.value = Number(data?.likeCount ?? data?.LikeCount ?? likeCount.value)
      liked.value = true
    }
  } catch {
    liked.value = prevLiked
    likeCount.value = prevCount
  } finally {
    busy.value = false
  }
}

watch(
  () => [props.targetType, props.targetId],
  () => { void load() },
  { immediate: true },
)

onMounted(() => {
  if (!loaded.value) void load()
})
</script>
