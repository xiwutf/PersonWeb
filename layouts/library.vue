<template>
  <main ref="root" class="library-layout" :class="{ 'library-layout-reader': isReader }">
    <NConfigProvider :theme="null" :theme-overrides="isReader ? undefined : libraryTheme">
      <LibraryAccessGate><slot /></LibraryAccessGate>
    </NConfigProvider>
  </main>
</template>

<script setup lang="ts">
import '~/assets/css/library-access.css'
import '~/assets/css/library.css'
import { NConfigProvider, type GlobalThemeOverrides } from 'naive-ui'
const route = useRoute()
const isReader = computed(() => route.path.startsWith('/library/read/'))
const root = ref<HTMLElement>()
const libraryTheme = ref<GlobalThemeOverrides>({})
onMounted(() => {
  if (!root.value) return
  const style = getComputedStyle(root.value)
  const token = (name: string) => style.getPropertyValue(name).trim()
  libraryTheme.value = {
    common: {
      primaryColor: token('--color-primary'), primaryColorHover: token('--color-primary-hover'),
      primaryColorPressed: token('--color-primary'), primaryColorSuppl: token('--color-primary'),
      textColorBase: token('--color-text'), borderColor: token('--color-border-strong'),
    },
    Input: { color: token('--color-bg-soft'), borderRadius: token('--radius-sm'),
      textColor: token('--color-text'), placeholderColor: token('--color-text-muted') },
    Button: { borderRadiusLarge: token('--radius-sm') },
  }
})
</script>
