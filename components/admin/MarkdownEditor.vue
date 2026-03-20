<template>
  <div class="markdown-editor-wrapper">
    <div v-if="loadError" class="markdown-editor-status markdown-editor-status--error">
      Markdown editor failed to load. Please refresh and try again.
    </div>
    <div v-else-if="!editorComponent" class="markdown-editor-status">
      Loading markdown editor...
    </div>
    <component
      :is="editorComponent"
      v-else
      v-model="content"
      :plugins="plugins"
      :upload-images="handleImageUpload"
      :placeholder="placeholder"
      class="markdown-editor"
      @change="handleChange"
    />
  </div>
</template>

<script setup lang="ts">
import type { Component } from 'vue'

interface ByteMdPlugin {
  [key: string]: unknown
}

const props = withDefaults(defineProps<{
  modelValue: string
  placeholder?: string
  height?: string
}>(), {
  modelValue: '',
  placeholder: 'Start writing your Markdown content...',
  height: '600px'
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const content = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const editorComponent = shallowRef<Component | null>(null)
const plugins = shallowRef<ByteMdPlugin[]>([])
const loadError = ref('')
const api = useApi()

// Load ByteMD and its plugins only when the editor itself is rendered.
onMounted(async () => {
  try {
    const [
      editorModule,
      gfmModule,
      highlightModule,
      mathModule,
      mediumZoomModule
    ] = await Promise.all([
      import('@bytemd/vue-next'),
      import('@bytemd/plugin-gfm'),
      import('@bytemd/plugin-highlight'),
      import('@bytemd/plugin-math'),
      import('@bytemd/plugin-medium-zoom'),
      import('bytemd/dist/index.css'),
      import('highlight.js/styles/github-dark.css')
    ])

    editorComponent.value = editorModule.Editor
    plugins.value = [
      gfmModule.default(),
      highlightModule.default(),
      mathModule.default(),
      mediumZoomModule.default()
    ]
  } catch (error) {
    console.error('Failed to load markdown editor:', error)
    loadError.value = 'Failed to load markdown editor.'
  }
})

const handleImageUpload = async (files: File[]): Promise<string[]> => {
  const uploadedUrls: string[] = []

  for (const file of files) {
    try {
      if (!file.type.startsWith('image/')) {
        throw new Error('Please select an image file.')
      }

      if (file.size > 5 * 1024 * 1024) {
        throw new Error('Image size cannot exceed 5MB.')
      }

      const formData = new FormData()
      formData.append('file', file)

      const res = await api.post<{ url?: string }>('/Media/upload', formData)
      if (res?.url) {
        uploadedUrls.push(res.url)
      } else {
        throw new Error('Upload failed: invalid response URL.')
      }
    } catch (error: unknown) {
      console.error('Image upload error:', error)
      alert(error instanceof Error ? error.message : 'Image upload failed.')
    }
  }

  return uploadedUrls
}

const handleChange = (value: string) => {
  emit('update:modelValue', value)
}
</script>

<style scoped>
.markdown-editor-wrapper {
  width: 100%;
  min-height: 400px;
}

.markdown-editor-status {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 400px;
  padding: var(--spacing-6, 1.5rem);
  border: 1px solid var(--color-border-default, rgb(209 213 219));
  border-radius: var(--radius-md, 0.5rem);
  background: var(--color-bg-elevated, var(--color-bg-card));
  color: var(--color-text-muted, rgb(107 114 128));
}

.markdown-editor-status--error {
  color: var(--color-error, #dc2626);
}

.markdown-editor {
  height: v-bind(height);
  min-height: 400px;
}

:deep(.bytemd) {
  min-height: 400px;
  border: 2px solid var(--color-border-default, rgb(209 213 219));
  border-radius: var(--radius-md, 0.5rem);
  background: var(--color-bg-elevated, var(--color-bg-card));
  box-shadow: 0 0 0 1px rgba(0, 0, 0, 0.03);
}

:deep(.bytemd:focus-within) {
  border-color: var(--color-primary, var(--color-primary-hover));
  box-shadow: 0 0 0 3px var(--color-primary-soft, rgba(37, 99, 235, 0.2));
}

:deep(.bytemd-toolbar) {
  border-bottom: 1px solid var(--color-border-subtle, rgb(229 231 235));
  background: var(--color-bg-elevated, rgb(249 250 251));
}

:deep(.bytemd-editor) {
  font-size: 14px;
  line-height: 1.6;
  min-height: 280px;
  background: var(--color-bg-editor, rgba(255, 255, 255, 0.95)) !important;
  border-right: 1px solid var(--color-border-subtle, var(--color-border));
}

:deep(.bytemd-editor .CodeMirror) {
  background: transparent !important;
  cursor: text !important;
}

:deep(.bytemd-editor .CodeMirror-scroll) {
  cursor: text !important;
}

:deep(.bytemd-editor .CodeMirror-placeholder) {
  color: var(--color-text-muted, var(--color-text-sec)) !important;
}

:deep(.bytemd-preview) {
  background: var(--color-bg-elevated, var(--color-bg-card));
}

[data-theme='dark'] :deep(.bytemd),
[data-theme='tech-blue'] :deep(.bytemd),
[data-theme='forest'] :deep(.bytemd),
[data-theme='hybrid-super-dark'] :deep(.bytemd) {
  border: 2px solid rgba(255, 255, 255, 0.35) !important;
  background: var(--color-bg-elevated);
  box-shadow: 0 0 0 1px rgba(255, 255, 255, 0.1);
}

[data-theme='dark'] :deep(.bytemd-toolbar),
[data-theme='tech-blue'] :deep(.bytemd-toolbar),
[data-theme='forest'] :deep(.bytemd-toolbar),
[data-theme='hybrid-super-dark'] :deep(.bytemd-toolbar) {
  background: var(--color-border);
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
}

[data-theme='dark'] :deep(.bytemd-editor),
[data-theme='tech-blue'] :deep(.bytemd-editor),
[data-theme='forest'] :deep(.bytemd-editor),
[data-theme='hybrid-super-dark'] :deep(.bytemd-editor) {
  background: rgba(255, 255, 255, 0.14) !important;
  border-right: 1px solid rgba(255, 255, 255, 0.25) !important;
  box-shadow: inset 0 0 0 1px var(--color-border);
}

[data-theme='dark'] :deep(.bytemd-editor .CodeMirror),
[data-theme='tech-blue'] :deep(.bytemd-editor .CodeMirror),
[data-theme='forest'] :deep(.bytemd-editor .CodeMirror),
[data-theme='hybrid-super-dark'] :deep(.bytemd-editor .CodeMirror) {
  color: var(--color-text-main) !important;
}

[data-theme='dark'] :deep(.bytemd-editor .CodeMirror-placeholder),
[data-theme='tech-blue'] :deep(.bytemd-editor .CodeMirror-placeholder),
[data-theme='forest'] :deep(.bytemd-editor .CodeMirror-placeholder),
[data-theme='hybrid-super-dark'] :deep(.bytemd-editor .CodeMirror-placeholder) {
  color: rgba(255, 255, 255, 0.55) !important;
}

[data-theme='dark'] :deep(.bytemd-preview),
[data-theme='tech-blue'] :deep(.bytemd-preview),
[data-theme='forest'] :deep(.bytemd-preview),
[data-theme='hybrid-super-dark'] :deep(.bytemd-preview) {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}
</style>
