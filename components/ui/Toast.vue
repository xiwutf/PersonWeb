<template>
  <Teleport to="body">
    <TransitionGroup name="toast" tag="div" class="toast-container">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        class="toast-item"
        :class="[
          `toast-item--${toast.type}`,
          `toast-item--${toast.position}`,
          { 'toast-item--closable': toast.closable }
        ]"
        role="alert"
        :aria-live="toast.type === 'error' ? 'assertive' : 'polite'"
      >
        <!-- 图标 -->
        <div class="toast-icon" v-if="toast.showIcon">
          <n-icon v-if="toast.type === 'success'" :component="CheckCircleIcon" />
          <n-icon v-else-if="toast.type === 'error'" :component="XCircleIcon" />
          <n-icon v-else-if="toast.type === 'warning'" :component="WarningIcon" />
          <n-icon v-else-if="toast.type === 'info'" :component="InfoIcon" />
        </div>

        <!-- 内容 -->
        <div class="toast-content">
          <h4 v-if="toast.title" class="toast-title">
            {{ toast.title }}
          </h4>
          <p class="toast-message">
            {{ toast.message }}
          </p>
        </div>

        <!-- 关闭按钮 -->
        <button
          v-if="toast.closable"
          class="toast-close"
          type="button"
          :aria-label="'关闭通知'"
          @click="handleClose(toast.id)"
        >
          <n-icon :component="CloseIcon" />
        </button>

        <!-- 进度条 -->
        <div
          v-if="toast.duration && toast.duration > 0"
          class="toast-progress"
          :style="{ animationDuration: `${toast.duration}ms` }"
        ></div>
      </div>
    </TransitionGroup>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  CheckCircle as CheckCircleIcon,
  XCircle as XCircleIcon,
  Warning as WarningIcon,
  Information as InfoIcon,
  Close as CloseIcon
} from '@vicons/ionicons5'
import { NIcon } from 'naive-ui'

export interface ToastItem {
  id: string
  type: 'success' | 'error' | 'warning' | 'info'
  title?: string
  message: string
  duration?: number
  closable?: boolean
  showIcon?: boolean
  position?: 'top-right' | 'top-left' | 'top-center' | 'bottom-right' | 'bottom-left' | 'bottom-center'
}

const toasts = ref<ToastItem[]>([])
let toastIdCounter = 0

const addToast = (toast: Omit<ToastItem, 'id'>) => {
  const id = `toast-${++toastIdCounter}`
  const newToast: ToastItem = {
    id,
    type: toast.type || 'info',
    title: toast.title,
    message: toast.message,
    duration: toast.duration ?? 3000,
    closable: toast.closable ?? true,
    showIcon: toast.showIcon ?? true,
    position: toast.position ?? 'top-right',
    ...toast
  }

  toasts.value.push(newToast)

  // 自动关闭
  if (newToast.duration && newToast.duration > 0) {
    setTimeout(() => {
      removeToast(id)
    }, newToast.duration)
  }

  return id
}

const removeToast = (id: string) => {
  const index = toasts.value.findIndex(t => t.id === id)
  if (index > -1) {
    toasts.value.splice(index, 1)
  }
}

const handleClose = (id: string) => {
  removeToast(id)
}

// 暴露方法
defineExpose({
  success: (message: string, options?: Partial<ToastItem>) =>
    addToast({ type: 'success', message, ...options }),
  error: (message: string, options?: Partial<ToastItem>) =>
    addToast({ type: 'error', message, ...options }),
  warning: (message: string, options?: Partial<ToastItem>) =>
    addToast({ type: 'warning', message, ...options }),
  info: (message: string, options?: Partial<ToastItem>) =>
    addToast({ type: 'info', message, ...options }),
  show: (options: Omit<ToastItem, 'id'>) =>
    addToast(options)
})
</script>

<style scoped>
.toast-container {
  position: fixed;
  z-index: 2000;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  pointer-events: none;
}

/* 位置变体 */
.toast-item--top-right {
  position: fixed;
  top: var(--spacing-lg);
  right: var(--spacing-lg);
}

.toast-item--top-left {
  position: fixed;
  top: var(--spacing-lg);
  left: var(--spacing-lg);
}

.toast-item--top-center {
  position: fixed;
  top: var(--spacing-lg);
  left: 50%;
  transform: translateX(-50%);
}

.toast-item--bottom-right {
  position: fixed;
  bottom: var(--spacing-lg);
  right: var(--spacing-lg);
}

.toast-item--bottom-left {
  position: fixed;
  bottom: var(--spacing-lg);
  left: var(--spacing-lg);
}

.toast-item--bottom-center {
  position: fixed;
  bottom: var(--spacing-lg);
  left: 50%;
  transform: translateX(-50%);
}

/* Toast 项目 */
.toast-item {
  display: flex;
  align-items: flex-start;
  gap: var(--spacing-md);
  min-width: 320px;
  max-width: 480px;
  padding: var(--spacing-md);
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-xl);
  position: relative;
  pointer-events: auto;
  overflow: hidden;
}

.toast-item--success {
  border-left: 4px solid var(--color-success);
}

.toast-item--error {
  border-left: 4px solid var(--color-error);
}

.toast-item--warning {
  border-left: 4px solid var(--color-warning);
}

.toast-item--info {
  border-left: 4px solid var(--color-info);
}

.toast-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  flex-shrink: 0;
  font-size: var(--text-xl);
}

.toast-item--success .toast-icon {
  color: var(--color-success);
}

.toast-item--error .toast-icon {
  color: var(--color-error);
}

.toast-item--warning .toast-icon {
  color: var(--color-warning);
}

.toast-item--info .toast-icon {
  color: var(--color-info);
}

.toast-content {
  flex: 1;
}

.toast-title {
  font-size: var(--text-base);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-xs);
}

.toast-message {
  font-size: var(--text-sm);
  color: var(--color-text-sec);
  line-height: 1.5;
  margin: 0;
}

.toast-close {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  border: none;
  background: transparent;
  color: var(--color-text-muted);
  cursor: pointer;
  border-radius: var(--radius-full);
  transition: all 0.2s;
  flex-shrink: 0;
}

.toast-close:hover {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}

.toast-close:focus-visible {
  outline: 2px solid var(--color-primary);
}

.toast-progress {
  position: absolute;
  left: 0;
  bottom: 0;
  height: 3px;
  background: currentColor;
  width: 100%;
}

/* 动画 */
.toast-enter-active {
  transition: all 0.3s ease;
}

.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateY(-20px);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(100%);
}

/* 响应式 */
@media (max-width: 768px) {
  .toast-item {
    min-width: calc(100vw - var(--spacing-2xl));
    max-width: calc(100vw - var(--spacing-2xl));
  }
}
</style>
