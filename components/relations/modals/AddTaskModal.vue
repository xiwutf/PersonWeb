<template>
  <n-modal
    v-model:show="visible"
    preset="card"
    :title="isEdit ? '编辑任务' : '新增任务'"
    style="width: 500px"
    @close="handleClose"
  >
    <n-form ref="formRef" :model="form" :rules="rules" label-placement="left" label-width="80px">
      <n-form-item label="任务标题" path="title">
        <n-input
          v-model:value="form.title"
          placeholder="请输入任务标题"
          show-count
          :maxlength="500"
        />
      </n-form-item>

      <n-form-item label="截止时间" path="dueAt">
        <n-date-picker
          v-model:value="form.dueAt"
          type="datetime"
          format="yyyy-MM-dd HH:mm"
          placeholder="请选择截止时间"
          style="width: 100%"
          clearable
        />
      </n-form-item>

      <n-form-item label="优先级" path="priority">
        <n-select
          v-model:value="form.priority"
          :options="priorityOptions"
          placeholder="请选择优先级"
        />
      </n-form-item>

      <n-form-item v-if="isEdit" label="状态" path="status">
        <n-select
          v-model:value="form.status"
          :options="statusOptions"
          placeholder="请选择状态"
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
import {
  NModal,
  NForm,
  NFormItem,
  NInput,
  NSelect,
  NDatePicker,
  NButton,
  type FormInst
} from 'naive-ui'
import type { RelationTask, CreateTaskDto, UpdateTaskDto } from '~/composables/useRelationsApi'
import { useSafeMessage } from '~/composables/useNaiveUI'

interface Props {
  show: boolean
  task?: RelationTask
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:show': [value: boolean]
  success: []
}>()

// 使用安全的 message composable，避免 Provider 未挂载时的错误
const message = useSafeMessage()
const formRef = ref<FormInst | null>(null)
const loading = ref(false)

const visible = computed({
  get: () => props.show,
  set: (value) => emit('update:show', value)
})

const isEdit = computed(() => !!props.task)

const priorityOptions = [
  { label: '低', value: 0 },
  { label: '中', value: 1 },
  { label: '高', value: 2 },
  { label: '紧急', value: 3 }
]

const statusOptions = [
  { label: '待办', value: 0 },
  { label: '完成', value: 1 },
  { label: '延期', value: 2 }
]

const form = reactive<CreateTaskDto & { status?: number; dueAt?: number }>({
  title: '',
  dueAt: undefined,
  priority: 1
})

const rules = {
  title: {
    required: true,
    message: '请输入任务标题',
    trigger: 'blur'
  },
  priority: {
    required: true,
    message: '请选择优先级',
    trigger: 'change'
  }
}

watch(() => props.show, (show) => {
  if (show) {
    if (props.task) {
      // 编辑模式
      form.title = props.task.title
      form.dueAt = props.task.dueAt ? new Date(props.task.dueAt).getTime() : undefined
      form.priority = props.task.priority
      form.status = props.task.status
    } else {
      // 新增模式
      form.title = ''
      form.dueAt = undefined
      form.priority = 1
      form.status = undefined
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

      if (isEdit.value && props.task) {
        const updateData: UpdateTaskDto = {
          title: form.title,
          dueAt: form.dueAt ? new Date(form.dueAt).toISOString() : undefined,
          priority: form.priority,
          status: form.status
        }
        await relationsApi.updateTask(props.task.id, updateData)
        showMessage.success('更新成功')
      } else {
        // 新增任务需要 personId，通过 emit 传递
        showMessage.error('新增任务需要在详情页操作')
        return
      }

      emit('success')
      handleClose()
    } catch (error: any) {
      showMessage.error(error.message || '操作失败')
    } finally {
      loading.value = false
    }
  })
}

// 暴露方法供外部调用（用于新增任务）
defineExpose({
  setPersonId: (personId: string) => {
    // 这里可以保存 personId，但新增任务通常通过详情页调用
  }
})
</script>

