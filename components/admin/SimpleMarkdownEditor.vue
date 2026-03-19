<template>
  <div class="md-wrap">
    <div class="pane editor">
      <NInput
        v-model:value="localValue"
        type="textarea"
        :autosize="{ minRows: minRows, maxRows: maxRows }"
        :placeholder="placeholder"
        class="editor-textarea"
      />
    </div>
    <div class="pane preview">
      <div class="preview-inner" v-html="html"></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { NInput } from 'naive-ui'
import MarkdownIt from 'markdown-it'
import { computed, ref, watch } from 'vue'

const props = withDefaults(
  defineProps<{
    modelValue: string
    placeholder?: string
    minRows?: number
    maxRows?: number
  }>(),
  {
    modelValue: '',
    placeholder: '用 Markdown 记录（支持标题、列表、代码块等）',
    minRows: 16,
    maxRows: 40
  }
)

const emit = defineEmits<{
  (e: 'update:modelValue', v: string): void
}>()

const localValue = ref(props.modelValue ?? '')
watch(
  () => props.modelValue,
  (v) => {
    if (v !== localValue.value) localValue.value = v ?? ''
  }
)
watch(localValue, (v) => emit('update:modelValue', v))

const md = new MarkdownIt({
  html: false,
  linkify: true,
  breaks: true
})

const html = computed(() => md.render(localValue.value || ''))
</script>

<style scoped>
/* 单层外框，避免与父级 section 重复嵌套 */
.md-wrap {
  display: grid;
  grid-template-columns: 1fr 1fr;
  width: 100%;
  min-height: 400px;
  border: 1px solid var(--color-border-default, var(--color-border));
  border-radius: var(--radius-lg);
  overflow: hidden;
  background: var(--color-bg-elevated, var(--color-bg-card, #fff));
}

.pane {
  overflow: hidden;
  background: transparent;
}

.editor {
  padding: 10px 12px;
  border-right: 1px solid var(--color-border-default, var(--color-border));
}

.editor-textarea :deep(textarea) {
  min-height: 360px !important;
  font-family: ui-monospace, SFMono-Regular, 'SF Mono', Menlo, Consolas, monospace;
  font-size: 14px;
  line-height: 1.6;
  border: none !important;
  background: transparent !important;
  box-shadow: none !important;
}

.preview {
  padding: 10px 12px;
  overflow: auto;
}

.preview-inner {
  min-height: 360px;
  font-size: 14px;
  line-height: 1.6;
  color: var(--color-text-main);
}

.preview-inner :deep(h1) {
  font-size: 22px;
  margin: 12px 0;
}
.preview-inner :deep(h2) {
  font-size: 18px;
  margin: 10px 0;
}
.preview-inner :deep(p) {
  line-height: 1.7;
  margin: 8px 0;
}
.preview-inner :deep(code) {
  padding: 2px 6px;
  border-radius: var(--radius-sm);
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}
.preview-inner :deep(pre) {
  padding: var(--spacing-3);
  border-radius: var(--radius-lg);
  background: var(--color-gray-900);
  color: var(--color-gray-100);
  overflow: auto;
}
.preview-inner :deep(pre code) {
  background: transparent;
  padding: 0;
}
.preview-inner :deep(ul) {
  padding-left: 18px;
}
.preview-inner :deep(blockquote) {
  border-left: 4px solid var(--color-border-default, var(--color-border));
  padding-left: 10px;
  color: var(--color-text-muted);
  margin: 8px 0;
}

/* 深色主题 */
[data-theme='dark'] .md-wrap,
[data-theme='tech-blue'] .md-wrap,
[data-theme='forest'] .md-wrap,
[data-theme='hybrid-super-dark'] .md-wrap {
  border-color: rgba(255, 255, 255, 0.25);
  background: rgba(255, 255, 255, 0.04);
}

[data-theme='dark'] .editor,
[data-theme='tech-blue'] .editor,
[data-theme='forest'] .editor,
[data-theme='hybrid-super-dark'] .editor {
  border-right-color: rgba(255, 255, 255, 0.2);
}
</style>
