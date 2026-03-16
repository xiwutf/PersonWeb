<template>
  <div
    class="avatar"
    :class="[
      `avatar--${size}`,
      `avatar--${variant}`,
      { 'avatar--square': square },
      { 'avatar--clickable': clickable }
    ]"
    :style="{ width: customSize, height: customSize }"
    @click="handleClick"
    role="img"
    :aria-label="alt"
  >
    <!-- 图片头像 -->
    <img
      v-if="src"
      :src="src"
      :alt="alt"
      class="avatar-image"
      @error="handleImageError"
    />

    <!-- 文字/图标头像 -->
    <span v-else class="avatar-content">
      <slot>
        <span v-if="text" class="avatar-text">{{ text }}</span>
        <n-icon v-else-if="icon" :size="iconSize" :component="icon" />
      </slot>
    </span>

    <!-- 徽章 -->
    <Badge
      v-if="badge"
      :text="badge"
      variant="badgeVariant"
      :dot="badgeDot"
      class="avatar-badge"
    />
  </div>
</template>

<script setup lang="ts">
import { NIcon } from 'naive-ui'

export interface AvatarProps {
  src?: string
  alt?: string
  text?: string
  icon?: any
  size?: 'xsmall' | 'small' | 'medium' | 'large' | 'xlarge' | 'auto'
  customSize?: string
  variant?: 'default' | 'circle' | 'square'
  square?: boolean
  clickable?: boolean
  badge?: string | number
  badgeVariant?: 'default' | 'primary' | 'success' | 'warning' | 'error'
  badgeDot?: boolean
}

const props = withDefaults(defineProps<AvatarProps>(), {
  alt: 'Avatar',
  size: 'medium',
  variant: 'circle',
  square: false,
  clickable: false,
  badgeVariant: 'default',
  badgeDot: false
})

const emit = defineEmits<{
  (event: 'click'): void
}>()

const handleClick = () => {
  if (props.clickable) {
    emit('click')
  }
}

const handleImageError = () => {
  // 图片加载失败时，可以触发回退逻辑
  emit('click') // 触发事件让父组件处理
}

// 根据尺寸计算图标大小
const iconSize = computed(() => {
  const sizeMap = {
    xsmall: 16,
    small: 20,
    medium: 24,
    large: 32,
    xlarge: 40,
    auto: 24
  }
  return sizeMap[props.size]
})
</script>

<style scoped>
.avatar {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: var(--color-bg-elevated);
  border-radius: var(--radius-full);
  overflow: hidden;
  flex-shrink: 0;
  transition: all 0.2s;
}

/* 变体 */
.avatar--circle {
  border-radius: var(--radius-full);
}

.avatar--square {
  border-radius: var(--radius-md);
}

.avatar--clickable {
  cursor: pointer;
}

.avatar--clickable:hover {
  transform: scale(1.05);
  box-shadow: var(--shadow-lg);
}

.avatar--clickable:active {
  transform: scale(0.95);
}

/* 尺寸 */
.avatar--xsmall {
  width: 24px;
  height: 24px;
}

.avatar--small {
  width: 32px;
  height: 32px;
}

.avatar--medium {
  width: 40px;
  height: 40px;
}

.avatar--large {
  width: 48px;
  height: 48px;
}

.avatar--xlarge {
  width: 64px;
  height: 64px;
}

.avatar--auto {
  width: var(--spacing-10);
  height: var(--spacing-10);
}

/* 图片 */
.avatar-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

/* 内容（文字/图标） */
.avatar-content {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.avatar-text {
  font-size: var(--text-base);
  font-weight: 600;
  color: var(--color-text-main);
}

/* 不同尺寸的文字大小 */
.avatar--xsmall .avatar-text {
  font-size: var(--text-xs);
}

.avatar--small .avatar-text {
  font-size: var(--text-sm);
}

.avatar--large .avatar-text {
  font-size: var(--text-lg);
}

.avatar--xlarge .avatar-text {
  font-size: var(--text-xl);
}

/* 徽章 */
.avatar-badge {
  position: absolute;
  top: -4px;
  right: -4px;
  z-index: 1;
}

/* 默认背景色变体 */
.avatar--default {
  background: var(--color-bg-elevated);
}

.avatar--circle.avatar--default {
  background: linear-gradient(135deg, var(--color-primary) 0%, var(--color-purple-500) 100%);
  color: white;
}

/* 响应式 */
@media (max-width: 768px) {
  .avatar--xlarge,
  .avatar--large {
    width: 40px;
    height: 40px;
  }
}
</style>
