<template>
  <article class="life-thought-list-item">
    <InlineEditableText
      v-if="canEdit"
      v-model="local.text"
      :field-path="`${fieldPrefix}.text`"
      as="p"
      display-class="life-thought-list-text"
      multiline
      :rows="3"
      :save="save"
    />
    <p v-else class="life-thought-list-text">{{ item.text }}</p>

    <InlineEditableText
      v-if="canEdit"
      v-model="local.note"
      :field-path="`${fieldPrefix}.note`"
      as="p"
      display-class="life-thought-list-note"
      multiline
      :rows="2"
      :save="save"
    />
    <p v-else-if="item.note" class="life-thought-list-note">{{ item.note }}</p>

    <p v-if="item.sourceUrl" class="life-thought-list-source">
      <a :href="item.sourceUrl" target="_blank" rel="noopener noreferrer">
        {{ item.source || item.sourceUrl }}
      </a>
    </p>
    <p v-else-if="item.source" class="life-thought-list-source">{{ item.source }}</p>
    <p v-if="item.createdAt" class="life-thought-list-date">{{ item.createdAt }}</p>

    <div v-if="canEdit" class="life-thought-list-actions">
      <button type="button" class="life-admin-add" @click="$emit('remove')">删除</button>
    </div>
  </article>
</template>

<script setup lang="ts">
type ThoughtItem = {
  id: string
  text: string
  note?: string
  source?: string
  sourceUrl?: string
  createdAt?: string
  featured?: boolean
}

const props = defineProps<{
  item: ThoughtItem
  canEdit?: boolean
  fieldPrefix?: string
  save?: (path: string, value: string) => Promise<unknown>
}>()

defineEmits<{
  remove: []
}>()

const local = computed(() => ({
  text: props.item.text,
  note: props.item.note || '',
}))
</script>
