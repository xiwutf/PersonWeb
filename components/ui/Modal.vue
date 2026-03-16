<template>
  <teleport to="body">
    <Transition name="modal-fade">
      <div
        v-if="show"
        class="modal-overlay"
        :class="{ 'modal-overlay--closable': closable }"
        @click="handleOverlayClick"
      >
        <div
          class="modal-container"
          :class="[
            `modal-container--${size}`,
            `modal-container--${variant}`,
            { 'modal-container--centered': centered }
          ]"
          :style="{ width: customWidth, maxWidth: maxWidth }"
          @click.stop
        >
          <!-- 头部 -->
          <div v-if="showHeader" class="modal-header">
            <slot name="header">
              <h3 class="modal-title">{{ title }}</h3>
            </slot>
            <button
              v-if="closable"
              class="modal-close"
              type="button"
              :aria-label="closeLabel"
              @click="handleClose"
            >
              <n-icon :component="CloseIcon" />
            </button>
          </div>

          <!-- 内容 -->
          <div class="modal-body" :class="{ 'modal-body--scrollable': scrollable }">
            <slot>
              <p v-if="description" class="modal-description">
                {{ description }}
              </p>
            </slot>
          </div>

          <!-- 底部 -->
          <div v-if="showFooter" class="modal-footer">
            <slot name="footer">
              <n-space justify="end">
                <n-button v-if="showCancel" @click="handleCancel">
                  {{ cancelText }}
                </n-button>
                <n-button
                  v-if="showConfirm"
                  type="primary"
                  :loading="confirming"
                  @click="handleConfirm"
                >
                  {{ confirmText }}
                </n-button>
              </n-space>
            </slot>
          </div>
        </div>
      </div>
    </Transition>
  </teleport>
</template>

<script setup lang="ts">
import { watch } from 'vue'
import { Close as CloseIcon } from '@vicons/ionicons5'
import { NIcon, NSpace, NButton } from 'naive-ui'

export interface ModalProps {
  show?: boolean
  title?: string
  description?: string
  size?: 'small' | 'medium' | 'large' | 'auto'
  variant?: 'default' | 'danger' | 'warning'
  closable?: boolean
  closeLabel?: string
  centered?: boolean
  scrollable?: boolean
  showHeader?: boolean
  showFooter?: boolean
  showConfirm?: boolean
  showCancel?: boolean
  confirmText?: string
  cancelText?: string
  confirming?: boolean
  customWidth?: string
  maxWidth?: string
  closeOnOverlay?: boolean
}

const props = withDefaults(defineProps<ModalProps>(), {
  show: false,
  size: 'medium',
  variant: 'default',
  closable: true,
  closeLabel: '关闭',
  centered: true,
  scrollable: false,
  showHeader: true,
  showFooter: true,
  showCancel: true,
  showConfirm: true,
  confirmText: '确定',
  cancelText: '取消',
  confirming: false,
  closeOnOverlay: true
})

const emit = defineEmits<{
  (event: 'update:show', value: boolean): void
  (event: 'close'): void
  (event: 'cancel'): void
  (event: 'confirm'): void
}>()

const handleOverlayClick = () => {
  if (props.closeOnOverlay) {
    handleClose()
  }
}

const handleClose = () => {
  emit('update:show', false)
  emit('close')
}

const handleCancel = () => {
  emit('cancel')
  handleClose()
}

const handleConfirm = () => {
  emit('confirm')
}

// 键盘事件处理
const handleKeydown = (e: KeyboardEvent) => {
  if (e.key === 'Escape' && props.closable) {
    handleClose()
  }
}

watch(() => props.show, (show) => {
  if (show) {
    document.addEventListener('keydown', handleKeydown)
    document.body.style.overflow = 'hidden'
  } else {
    document.removeEventListener('keydown', handleKeydown)
    document.body.style.overflow = ''
  }
})
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: var(--spacing-lg);
}

.modal-overlay--closable {
  cursor: pointer;
}

.modal-container {
  background: var(--color-bg-card);
  border-radius: var(--radius-xl);
  box-shadow: var(--shadow-2xl);
  display: flex;
  flex-direction: column;
  max-height: calc(100vh - var(--spacing-3xl));
  animation: modalSlideIn 0.3s ease-out;
}

.modal-container--small {
  width: 400px;
}

.modal-container--medium {
  width: 560px;
}

.modal-container--large {
  width: 720px;
}

.modal-container--auto {
  width: auto;
  min-width: 320px;
  max-width: calc(100vw - var(--spacing-2xl));
}

.modal-container--centered {
  margin: auto;
}

.modal-container--danger {
  border-left: 4px solid var(--color-error);
}

.modal-container--warning {
  border-left: 4px solid var(--color-warning);
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--spacing-lg) var(--spacing-xl) var(--spacing-md);
  border-bottom: 1px solid var(--color-border-subtle);
}

.modal-title {
  font-size: var(--text-xl);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

.modal-close {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border: none;
  background: transparent;
  border-radius: var(--radius-full);
  color: var(--color-text-muted);
  cursor: pointer;
  transition: all 0.2s;
}

.modal-close:hover {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}

.modal-close:focus-visible {
  outline: 2px solid var(--color-primary);
}

.modal-body {
  padding: var(--spacing-xl);
  overflow-y: auto;
}

.modal-body--scrollable {
  max-height: 400px;
}

.modal-description {
  font-size: var(--text-base);
  color: var(--color-text-sec);
  line-height: 1.6;
  margin: 0 0 var(--spacing-lg);
}

.modal-footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding: var(--spacing-md) var(--spacing-xl) var(--spacing-lg);
  border-top: 1px solid var(--color-border-subtle);
}

/* 动画 */
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.3s ease;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* 响应式 */
@media (max-width: 768px) {
  .modal-overlay {
    padding: var(--spacing-md);
  }

  .modal-container--small,
  .modal-container--medium,
  .modal-container--large {
    width: 100%;
    max-width: calc(100vw - var(--spacing-lg));
  }

  .modal-header,
  .modal-footer {
    padding: var(--spacing-md);
  }

  .modal-body {
    padding: var(--spacing-md);
  }
}
</style>
<style>
/* 全局样式支持 */
.modal-container :deep(.n-space) {
  width: 100%;
}
</style>
