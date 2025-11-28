<template>
  <span class="inline-block">
    <span>{{ displayedText }}</span>
    <span 
      v-if="isTyping || !hideCursor"
      class="inline-block w-0.5 h-5 bg-current ml-1 animate-pulse"
      :class="cursorClass"
    ></span>
  </span>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  text: string
  speed?: number
  delay?: number
  loop?: boolean
  hideCursor?: boolean
  cursorClass?: string
}>(), {
  speed: 100,
  delay: 0,
  loop: false,
  hideCursor: false,
  cursorClass: ''
})

const displayedText = ref('')
const isTyping = ref(false)
let timeoutId: NodeJS.Timeout | null = null

const typeText = () => {
  isTyping.value = true
  displayedText.value = ''
  let index = 0

  const type = () => {
    if (index < props.text.length) {
      displayedText.value += props.text[index]
      index++
      timeoutId = setTimeout(type, props.speed)
    } else {
      isTyping.value = false
      if (props.loop) {
        setTimeout(() => {
          displayedText.value = ''
          index = 0
          typeText()
        }, 2000)
      }
    }
  }

  if (props.delay > 0) {
    setTimeout(type, props.delay)
  } else {
    type()
  }
}

onMounted(() => {
  typeText()
})

onUnmounted(() => {
  if (timeoutId) {
    clearTimeout(timeoutId)
  }
})

watch(() => props.text, () => {
  typeText()
})
</script>

