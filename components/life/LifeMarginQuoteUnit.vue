<template>
  <article
    class="life-margin-unit"
    :class="[`is-${level}`, toneClass, { 'is-admin': canEdit }]"
  >
    <span v-if="showMark" class="life-margin-unit-mark" aria-hidden="true">“</span>
    <span v-if="showNum" class="life-margin-unit-num" aria-hidden="true">01</span>

    <div class="life-margin-unit-body">
      <InlineEditableText
        v-if="canEdit"
        v-model="text"
        :field-path="`items.${index}.text`"
        as="p"
        :display-class="textClass"
        multiline
        :save="save"
      />
      <p v-else :class="textClass">{{ text }}</p>

      <aside v-if="canEdit || note" class="life-margin-unit-note">
        <InlineEditableText
          v-if="canEdit"
          v-model="note"
          :field-path="`items.${index}.note`"
          as="p"
          display-class="life-margin-note"
          multiline
          :save="save"
        />
        <p v-else class="life-margin-note">{{ note }}</p>
      </aside>

      <slot />
    </div>
  </article>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  index: number
  level: 'lead' | 'thought' | 'note'
  canEdit?: boolean
  showMark?: boolean
  showNum?: boolean
  toneClass?: string
  save: (path: string, value: string) => Promise<unknown>
}>(), {
  canEdit: false,
  showMark: false,
  showNum: false,
  toneClass: '',
})

const text = defineModel<string>('text', { required: true })
const note = defineModel<string>('note', { required: true })

const textClass = computed(() => {
  if (props.level === 'lead') return 'life-margin-text life-margin-text--lead'
  if (props.level === 'note') return 'life-margin-text life-margin-text--note'
  return 'life-margin-text life-margin-text--thought'
})
</script>
