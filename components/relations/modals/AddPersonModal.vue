<template>
  <n-modal
    v-model:show="visible"
    preset="card"
    :title="isEdit ? '编辑对象' : '新增对象'"
    style="width: 600px"
    @close="handleClose"
  >
    <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="80px">
      <n-form-item label="昵称" path="nickname">
        <n-input v-model:value="form.nickname" placeholder="请输入昵称" />
      </n-form-item>

      <n-form-item label="来源" path="source">
        <n-input v-model:value="form.source" placeholder="如：朋友介绍/社交软件/活动等" />
      </n-form-item>

      <n-form-item label="城市" path="city">
        <n-input v-model:value="form.city" placeholder="请输入城市" />
      </n-form-item>

      <n-form-item label="标签" path="tags">
        <n-dynamic-tags v-model:value="form.tags" />
      </n-form-item>

      <n-form-item label="阶段" path="stage">
        <n-select
          v-model:value="form.stage"
          :options="stageOptions"
          placeholder="请选择阶段"
        />
      </n-form-item>

      <n-form-item label="喜好/雷点" path="preferences">
        <n-input
          v-model:value="form.preferences"
          type="textarea"
          :rows="3"
          placeholder="记录对方的喜好、雷点或关键信息"
        />
      </n-form-item>

      <n-form-item label="备注" path="notes">
        <n-input
          v-model:value="form.notes"
          type="textarea"
          :rows="3"
          placeholder="其他备注信息"
        />
      </n-form-item>
    </n-form>

    <template #footer>
      <div style="display: flex; justify-content: flex-end; gap: 8px;">
        <n-button @click="handleClose">取消</n-button>
        <n-button type="primary" :loading="loading" @click="handleSubmit">
          {{ isEdit ? '更新' : '创建' }}
        </n-button>
      </div>
    </template>
  </n-modal>
</template>

<script setup lang="ts">
import { ref, watch, reactive, computed } from 'vue'
import { NModal, NForm, NFormItem, NInput, NSelect, NButton, NDynamicTags, useMessage, type FormInst } from 'naive-ui'
import type { RelationPerson, CreatePersonDto, UpdatePersonDto } from '~/composables/useRelationsApi'

interface Props {
  show: boolean
  person?: RelationPerson
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:show': [value: boolean]
  success: []
}>()

const message = useMessage()
const formRef = ref<FormInst | null>(null)
const loading = ref(false)

const visible = computed({
  get: () => props.show,
  set: (value) => emit('update:show', value)
})

const isEdit = computed(() => !!props.person)

const stageOptions = [
  { label: '新认识', value: 0 },
  { label: '熟悉中', value: 1 },
  { label: '准备约见', value: 2 },
  { label: '已见面', value: 3 },
  { label: '升温中', value: 4 },
  { label: '观察期', value: 5 },
  { label: '已结束', value: 6 }
]

const form = reactive<CreatePersonDto & { tags?: string[] }>({
  nickname: '',
  source: '',
  city: '',
  tags: [],
  stage: 0,
  preferences: '',
  notes: ''
})

const rules = {
  nickname: {
    required: true,
    message: '请输入昵称',
    trigger: 'blur'
  }
}

watch(() => props.show, (show) => {
  if (show) {
    if (props.person) {
      // 编辑模式，填充数据
      form.nickname = props.person.nickname
      form.source = props.person.source || ''
      form.city = props.person.city || ''
      form.tags = props.person.tags || []
      form.stage = props.person.stage
      form.preferences = props.person.preferences || ''
      form.notes = props.person.notes || ''
    } else {
      // 新增模式，重置表单
      form.nickname = ''
      form.source = ''
      form.city = ''
      form.tags = []
      form.stage = 0
      form.preferences = ''
      form.notes = ''
    }
  }
})

const handleClose = () => {
  visible.value = false
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  await formRef.value.validate(async (errors) => {
    if (errors) return

    loading.value = true
    try {
      const relationsApi = useRelationsApi()
      
      if (isEdit.value && props.person) {
        const updateData: UpdatePersonDto = {
          nickname: form.nickname,
          source: form.source || undefined,
          city: form.city || undefined,
          tags: form.tags,
          stage: form.stage,
          preferences: form.preferences || undefined,
          notes: form.notes || undefined
        }
        await relationsApi.updatePerson(props.person.id, updateData)
        message.success('更新成功')
      } else {
        const createData: CreatePersonDto = {
          nickname: form.nickname,
          source: form.source || undefined,
          city: form.city || undefined,
          tags: form.tags,
          stage: form.stage,
          preferences: form.preferences || undefined,
          notes: form.notes || undefined
        }
        await relationsApi.createPerson(createData)
        message.success('创建成功')
      }
      
      emit('success')
      handleClose()
    } catch (error: any) {
      message.error(error.message || '操作失败')
    } finally {
      loading.value = false
    }
  })
}
</script>

