<template>
  <teleport to="body">
    <Transition name="drawer-fade">
      <div
        v-if="show"
        class="drawer-overlay"
        @click="handleOverlayClick"
      >
        <Transition name="drawer-slide">
          <div
            v-if="show"
            class="drawer-container"
            :class="[
              `drawer-container--${placement}`,
              `drawer-container--${size}`
            ]"
            :style="{ width: customWidth }"
            @click.stop
          >
            <!-- 头部 -->
            <div v-if="showHeader" class="drawer-header">
              <slot name="header">
                <h3 class="drawer-title">{{ title }}</h3>
              </slot>
              <button
                v-if="closable"
                class="drawer-close"
                type="button"
                :aria-label="closeLabel"
                @click="handleClose"
              >
                <n-icon :component="CloseIcon" />
              </button>
            </div>
          </div>

            <!-- 内容 -->
            <div class="drawer-body" :class="{ 'drawer-body--no-header': !showHeader }">
              <slot>
                <p v-if="description" class="drawer-description">
                  {{ description }}
                </p>
              </slot>
            </div>

            <!-- 底部 -->
            <div v-if="showFooter" class="drawer-footer">
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
        </Transition>
      </div>
    </Transition>
  </teleport>
</template>

<script setup lang="ts">
import { watch } from 'vue'
import { Close as CloseIcon } from '@vicons/ionicons5'
import { NIcon, NSpace, NButton } from 'naive-ui'

export interface DrawerProps {
  show?: boolean
  title?: string
  description?: string
  placement?: 'left' | 'right' | 'top' | 'bottom'
  size?: 'small' | 'medium' | 'large' | 'auto'
  closable?: boolean
  closeLabel?: string
  showHeader?: boolean
  showFooter?: boolean
  showConfirm?: boolean
  showCancel?: boolean
  confirmText?: string
  cancelText?: string
  confirming?: boolean
  customWidth?: string
  closeOnOverlay?: boolean
}

const props = withDefaults(defineProps<DrawerProps>(), {
  show: false,
  placement: 'right',
  size: 'medium',
  closable: true,
  closeLabel: '关闭',
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
.drawer-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 1000;
}

.drawer-container {
  background: var(--color-bg-card);
  box-shadow: var(--shadow-2xl);
  display: flex;
  flex-direction: column;
  max-height: 100vh;
}

/* 位置变体 */
.drawer-container--left {
  position: fixed;
  left: 0;
  top: 0;
  bottom: 0;
  width: 360px;
  border-right: 1px solid var(--color-border-default);
}

.drawer-container--right {
  position: fixed;
  right: 0;
  top: 0;
  bottom: 0;
  width: 360px;
  border-left: 1px solid var(--color-border-default);
}

.drawer-container--top {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: auto;
  min-height: 200px;
  max-height: 70vh;
  border-bottom: 1px solid var(--color-border-default);
}

.drawer-container--bottom {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  height: auto;
  min-height: 200px;
  max-height: 70vh;
  border-top: 1px solid var(--color-border-default);
}

/* 尺寸变体 */
.drawer-container--small {
  width: 300px;
}

.drawer-container--medium {
  width: 400px;
}

.drawer-container--large {
  width: 520px;
}

.drawer-container--auto {
  width: auto;
  min-width: 280px;
  max-width: 85vw;
}

/* 头部 */
.drawer-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--spacing-lg);
  border-bottom: 1px solid var(--color-border-subtle);
  flex-shrink: 0;
}

.drawer-title {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
}

.drawer-close {
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
  flex-shrink: 0;
}

.drawer-close:hover {
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
}

.drawer-close:focus-visible {
  outline: 2px solid var(--color-primary);
}

/* 内容 */
.drawer-body {
  flex: 1;
  overflow-y: auto;
  padding: var(--spacing-xl);
}

.drawer-body--no-header {
  padding-top: var(--spacing-xl);
}

.drawer-description {
  font-size: var(--text-base);
  color: var(--color-text-sec);
  line-height: 1.6;
  margin: 0 0 var(--spacing-lg);
}

/* 底部 */
.drawer-footer {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding: var(--spacing-md) var(--spacing-xl) var(--spacing-lg);
  border-top: 1px solid var(--color-border-subtle);
  flex-shrink: 0;
}

/* 动画 */
.drawer-fade-enter-active,
.drawer-fade-leave-active {
  transition: opacity 0.3s ease;
}

.drawer-fade-enter-from,
.drawer-fade-leave-to {
  opacity: 0;
}

.drawer-slide-enter-active,
.drawer-slide-leave-active {
  transition: transform 0.3s ease;
}

.drawer-slide-left-enter-from,
.drawer-slide-left-leave-to {
  transform: translateX(-100%);
}

.drawer-slide-left-enter-to,
.drawer-slide-left-leave-from {
  transform: translateX(0);
}

.drawer-slide-right-enter-from,
.drawer-slide-right-leave-to {
  transform: translateX(100%);
}

.drawer-slide-right-enter-to,
.drawer-slide-right-leave-from {
  transform: translateX(0);
}

.drawer-slide-top-enter-from,
.drawer-slide-top-leave-to {
  transform: translateY(-100%);
}

.drawer-slide-top-enter-to,
.drawer-slide-top-leave-from {
  transform: translateY(0);
}

.drawer-slide-bottom-enter-from,
.drawer-slide-bottom-leave-to {
  transform: translateY(100%);
}

.drawer-slide-bottom-enter-to,
.drawer-slide-bottom-leave-from {
  transform: translateY(0);
}

/* 响应式 */
@media (max-width: 768px) {
  .drawer-container--left,
  .drawer-container--right {
    width: 100%;
  }

  .drawer-container--top,
  .drawer-container--bottom {
    width: 100%;
  }

  .drawer-header,
  .drawer-footer {
    padding: var(--spacing-md);
  }

  .drawer-body {
    padding: var(--spacing-md);
  }
}
</style>
<style>
.drawer-container :deep(.n-space) {
  width: 100%;
}
</style>
