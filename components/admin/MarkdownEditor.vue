<template>
  <div class="markdown-editor-wrapper">
    <Editor
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
import { Editor } from '@bytemd/vue-next'
import gfm from '@bytemd/plugin-gfm'
import highlight from '@bytemd/plugin-highlight'
import math from '@bytemd/plugin-math'
import mediumZoom from '@bytemd/plugin-medium-zoom'
import 'bytemd/dist/index.css'
import 'highlight.js/styles/github-dark.css'

const props = withDefaults(defineProps<{
  modelValue: string
  placeholder?: string
  height?: string
}>(), {
  modelValue: '',
  placeholder: '开始编写你的 Markdown 内容...',
  height: '600px'
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const content = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

// 配置插件
const plugins = [
  gfm(), // GitHub Flavored Markdown
  highlight(), // 代码高亮
  math(), // 数学公式
  mediumZoom() // 图片缩放
]

const api = useApi()

// 处理图片上传
const handleImageUpload = async (files: File[]): Promise<string[]> => {
  const uploadedUrls: string[] = []
  
  for (const file of files) {
    try {
      // 验证文件类型
      if (!file.type.startsWith('image/')) {
        throw new Error('请选择图片文件')
      }

      // 验证文件大小（限制 5MB）
      if (file.size > 5 * 1024 * 1024) {
        throw new Error('图片大小不能超过 5MB')
      }

      const formData = new FormData()
      formData.append('file', file)

      // 上传到 OSS
      const res = await api.post<any>('/Media/upload', formData)
      if (res && res.url) {
        uploadedUrls.push(res.url)
      } else {
        throw new Error('上传失败：未返回有效 URL')
      }
    } catch (error: any) {
      console.error('Image upload error:', error)
      alert(error.message || '图片上传失败')
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

.markdown-editor {
  height: v-bind(height);
  min-height: 400px;
}

/* 确保 bytemd 根节点有高度，避免编辑区被压成一条线 */
:deep(.bytemd) {
  min-height: 400px;
}

/* 使用主题变量，增强边框与背景对比，便于分辨可填写区域 */
:deep(.bytemd) {
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

/* 左侧编辑区：与周围明显区分，一眼看出「这里可以输入」 */
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

/* 深色主题：整体编辑框用明显亮边，避免和背景融在一起 */
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

/* 深色下左侧编辑区：明显更亮的「可输入」区域，桌面上一眼能看见 */
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

