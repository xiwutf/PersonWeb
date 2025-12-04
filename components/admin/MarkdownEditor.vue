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
}

.markdown-editor {
  height: v-bind(height);
}

/* 自定义样式 */
:deep(.bytemd) {
  border: 1px solid rgb(209 213 219);
  border-radius: 0.5rem;
}

:deep(.bytemd-toolbar) {
  border-bottom: 1px solid rgb(229 231 235);
  background: rgb(249 250 251);
}

:deep(.bytemd-editor) {
  font-size: 14px;
  line-height: 1.6;
}

/* 暗色模式支持 */
.dark :deep(.bytemd) {
  border-color: rgb(75 85 99);
  background: rgb(17 24 39);
  color: rgb(229 231 235);
}

.dark :deep(.bytemd-toolbar) {
  background: rgb(31 41 55);
  border-bottom-color: rgb(75 85 99);
}

.dark :deep(.bytemd-editor) {
  background: rgb(17 24 39);
  color: rgb(229 231 235);
}

.dark :deep(.bytemd-preview) {
  background: rgb(17 24 39);
  color: rgb(229 231 235);
}
</style>

