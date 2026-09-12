<template>
  <div class="life-margin-admin">
    <label>
      <span class="sr-only">笔触</span>
      <select :value="item.tone || 'plain'" @change="emit('tone', index, $event)">
        <option v-for="tone in toneOptions" :key="tone.value" :value="tone.value">{{ tone.label }}</option>
      </select>
    </label>
    <label>
      <span class="sr-only">分组</span>
      <input
        :value="item.group || ''"
        type="text"
        maxlength="24"
        placeholder="分组"
        @change="emit('group', index, $event)"
      >
    </label>
    <label class="life-margin-featured">
      <input
        type="checkbox"
        :checked="!!item.featured"
        @change="emit('featured', index, $event)"
      >
      常看
    </label>
    <label class="life-margin-featured">
      <input
        type="checkbox"
        :checked="!!item.archived"
        @change="emit('archived', index, $event)"
      >
      归档
    </label>
    <button type="button" class="life-admin-add" @click="emit('remove', index)">
      删除
    </button>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  item: {
    tone?: string
    group?: string
    featured?: boolean
    archived?: boolean
  }
  index: number
  toneOptions: Array<{ value: string, label: string }>
}>()

const emit = defineEmits<{
  tone: [index: number, event: Event]
  group: [index: number, event: Event]
  featured: [index: number, event: Event]
  archived: [index: number, event: Event]
  remove: [index: number]
}>()
</script>
