<template>
  <div class="library-toc-heading"><strong>目录</strong><NButton size="small" @click="emit('close')">关闭</NButton></div>
  <div class="library-toc-actions"><NButton text size="small" @click="expanded = branchKeys">全部展开</NButton><NButton text size="small" @click="expanded = []">全部收起</NButton></div>
  <p v-if="!items.length">这本书没有提供目录。</p>
  <NTree v-else v-model:expanded-keys="expanded" :data="nodes" :selectable="false" :expand-on-click="false" :render-label="renderLabel" :theme-overrides="treeTheme" block-line />
</template>

<script setup lang="ts">
import { h } from 'vue'
import { NButton, NTree, type TreeOption } from 'naive-ui'
import type { TocItem } from '~/utils/library-reader'
const props = defineProps<{ items: TocItem[] }>()
const emit = defineEmits<{ close: []; jump: [href: string] }>()
const expanded = ref<string[]>([])
function mapNodes(items: TocItem[], prefix = ''): TreeOption[] {
  return items.map((item, index) => ({ key: `${prefix}${index}`, label: item.label.trim() || '未命名章节', href: item.href,
    children: item.subitems?.length ? mapNodes(item.subitems, `${prefix}${index}/`) : undefined }))
}
const nodes = computed(() => mapNodes(props.items))
const branchKeys = computed(() => {
  const keys: string[] = []
  function collect(items: TreeOption[]) { for (const item of items) if (item.children?.length) { keys.push(String(item.key)); collect(item.children) } }
  collect(nodes.value); return keys
})
const treeTheme = { nodeTextColor: 'var(--reader-text)', arrowColor: 'var(--reader-link)', nodeColorHover: 'transparent', nodeColorActive: 'transparent' }
function renderLabel({ option }: { option: TreeOption }) {
  return h('button', { type: 'button', class: 'library-toc-link', title: String(option.label),
    onClick: (event: MouseEvent) => { event.stopPropagation(); emit('jump', String(option.href)) } }, String(option.label))
}
</script>
