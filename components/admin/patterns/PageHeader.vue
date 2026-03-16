<template>
  <div class="admin-page-header" :class="props.class" :style="props.style">
    <slot>
      <!-- 默认内容：标题和描述 -->
      <div class="header-content">
        <component
          :is="headingTag"
          class="header-title"
        >
          <slot name="title">{{ props.title }}</slot>
        </component>
        <p v-if="$slots.description || props.description" class="header-description">
          <slot name="description">{{ props.description }}</slot>
        </p>
      </div>
    </slot>

    <!-- 右侧操作栏 -->
    <div v-if="props.showActions || $slots.actions || $slots['actions-prepend'] || $slots['actions-append']" class="header-actions">
      <slot name="actions-prepend">
        <!-- 预留扩展点 -->
      </slot>

      <template v-if="props.showActions && props.actions">
        <slot name="actions">
          <n-space :size="12">
            <n-button
              v-for="action in props.actions"
              :key="action.label"
              :type="action.type"
              :disabled="action.disabled"
              :loading="action.loading"
              @click="handleActionClick(action)"
            >
              <template v-if="action.icon" #icon>
                <i :class="action.icon"></i>
              </template>
              {{ action.label }}
            </n-button>
          </n-space>
        </slot>
      </template>

      <slot name="actions-append">
        <!-- 预留扩展点 -->
      </slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { NSpace, NButton } from 'naive-ui'

export interface ActionConfig {
  type?: 'primary' | 'default' | 'info' | 'success' | 'warning' | 'error'
  label: string
  icon?: string
  disabled?: boolean
  loading?: boolean
  onClick?: () => void
}

interface Props {
  /** 页面标题 */
  title: string
  /** 页面描述（可选） */
  description?: string
  /** 页面标题样式级别 */
  level?: 'h1' | 'h2'
  /** 是否显示面包屑（预留） */
  showBreadcrumb?: boolean
  /** 面包屑配置（预留） */
  breadcrumbs?: Array<{ label: string; to?: string }>
  /** 是否显示右侧操作栏 */
  showActions?: boolean
  /** 操作按钮配置 */
  actions?: ActionConfig[]
  /** 额外类名 */
  class?: string | string[] | Record<string, boolean>
  /** 自定义样式 */
  style?: Record<string, string>
}

const props = withDefaults(defineProps<Props>(), {
  level: 'h1',
  showBreadcrumb: false,
  showActions: false,
  breadcrumbs: () => [],
  actions: () => []
})

const emit = defineEmits<{
  (event: 'action-click', action: ActionConfig): void
}>()

// 根据级别计算标题标签
const headingTag = computed(() => props.level)

// 处理操作按钮点击
const handleActionClick = (action: ActionConfig) => {
  if (action.onClick) {
    action.onClick()
  }
  emit('action-click', action)
}
</script>

<style scoped>
.admin-page-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--spacing-xl);
  padding: var(--spacing-xl) 0;
}

.header-content {
  flex: 1;
  min-width: 0;
}

.header-title {
  font-size: var(--text-2xl);
  font-weight: 700;
  line-height: 1.3;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm) 0;
}

.admin-page-header[level="h2"] .header-title {
  font-size: var(--text-xl);
}

.header-description {
  font-size: var(--text-sm);
  color: var(--color-text-muted);
  margin: 0;
  line-height: 1.5;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  flex-shrink: 0;
}

/* 响应式：移动端改为垂直布局 */
@media (max-width: 768px) {
  .admin-page-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .header-actions {
    width: 100%;
    justify-content: flex-start;
  }
}
</style>
