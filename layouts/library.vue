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
async function updateLibraryTheme() {
  await nextTick()
  if (isReader.value) return
  if (!root.value) return
  const style = getComputedStyle(root.value)
  const token = (name: string) => style.getPropertyValue(name).trim()
  libraryTheme.value = {
    common: {
      primaryColor: token('--color-primary'), primaryColorHover: token('--color-primary-hover'),
      primaryColorPressed: token('--color-primary'), primaryColorSuppl: token('--color-primary'),
      textColorBase: token('--color-text'), borderColor: token('--color-border-strong'),
    },
    Input: { color: token('--color-surface-elevated'), colorFocus: token('--color-surface-elevated'), borderRadius: token('--radius-sm'),
      textColor: token('--color-text'), placeholderColor: token('--color-text-muted'),
      border: `1px solid ${token('--color-border')}`, borderHover: `1px solid ${token('--color-primary')}`, borderFocus: `1px solid ${token('--color-primary')}`, boxShadowFocus: 'none' },
    Button: { borderRadiusLarge: token('--radius-sm'),
      colorPrimary: token('--color-primary'), colorHoverPrimary: token('--color-primary-hover'), colorPressedPrimary: token('--color-primary-hover'), colorFocusPrimary: token('--color-primary'),
      textColorPrimary: token('--color-surface-elevated'), textColorHoverPrimary: token('--color-surface-elevated'), textColorPressedPrimary: token('--color-surface-elevated'), textColorFocusPrimary: token('--color-surface-elevated'),
      borderPrimary: `1px solid ${token('--color-primary')}`, borderHoverPrimary: `1px solid ${token('--color-primary-hover')}`, borderPressedPrimary: `1px solid ${token('--color-primary-hover')}` },
  }
}
onMounted(updateLibraryTheme)
watch(isReader, updateLibraryTheme)
</script>
