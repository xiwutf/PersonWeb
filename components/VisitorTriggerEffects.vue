<template>
  <!-- 技能树发光效果 -->
  <div
    v-if="effects.skillTreeGlow"
    class="skill-tree-glow-effect"
    :class="{ 'active': effects.skillTreeGlow }"
  ></div>

  <!-- 头像眨眼效果（通过 CSS 类控制） -->
  <div
    v-if="effects.avatarBlink"
    class="avatar-blink-effect"
  ></div>

  <!-- 小助手问候（通过事件触发 AI 助手） -->
  <div
    v-if="effects.assistantGreet"
    class="assistant-greet-trigger"
  ></div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'

const effects = ref({
  skillTreeGlow: false,
  avatarBlink: false,
  assistantGreet: false
})

// 处理触发事件
const handleTrigger = (event: CustomEvent) => {
  const { type } = event.detail

  switch (type) {
    case 'skill_tree_glow':
      triggerSkillTreeGlow()
      break
    case 'avatar_blink':
      triggerAvatarBlink()
      break
    case 'assistant_greet':
      triggerAssistantGreet()
      break
  }
}

// 技能树发光
const triggerSkillTreeGlow = () => {
  effects.value.skillTreeGlow = true
  
  // 添加发光类到技能树页面
  if (process.client) {
    document.documentElement.classList.add('skill-tree-glowing')
    
    setTimeout(() => {
      effects.value.skillTreeGlow = false
      document.documentElement.classList.remove('skill-tree-glowing')
    }, 3000)
  }
}

// 头像眨眼
const triggerAvatarBlink = () => {
  effects.value.avatarBlink = true
  
  // 添加眨眼类到所有头像
  if (process.client) {
    document.documentElement.classList.add('avatar-blinking')
    
    setTimeout(() => {
      effects.value.avatarBlink = false
      document.documentElement.classList.remove('avatar-blinking')
    }, 2000)
  }
}

// 小助手问候
const triggerAssistantGreet = () => {
  effects.value.assistantGreet = true
  
  // 触发 AI 助手打开并发送问候消息
  if (process.client) {
    window.dispatchEvent(new CustomEvent('open-ai-assistant', {
      detail: {
        message: '👋 你好！欢迎回来！有什么我可以帮你的吗？'
      }
    }))
    
    setTimeout(() => {
      effects.value.assistantGreet = false
    }, 1000)
  }
}

onMounted(() => {
  if (process.client) {
    window.addEventListener('visitor-trigger', handleTrigger as EventListener)
  }
})

onUnmounted(() => {
  if (process.client) {
    window.removeEventListener('visitor-trigger', handleTrigger as EventListener)
  }
})
</script>

<style scoped>
.skill-tree-glow-effect {
  position: fixed;
  inset: 0;
  pointer-events: none;
  z-index: 9999;
  opacity: 0;
  transition: opacity 0.5s ease;
}

.skill-tree-glow-effect.active {
  opacity: 1;
  background: radial-gradient(circle at center, var(--theme-primary) 0%, transparent 70%);
  animation: glow-pulse 3s ease-in-out;
}

.avatar-blink-effect,
.assistant-greet-trigger {
  display: none;
}

@keyframes glow-pulse {
  0%, 100% { opacity: 0.3; }
  50% { opacity: 0.6; }
}

/* 全局样式：技能树发光效果 */
:global(.skill-tree-glowing) {
  animation: skill-tree-glow 3s ease-in-out;
}

:global(.skill-tree-glowing .card) {
  box-shadow: 0 0 20px var(--theme-primary);
  border-color: var(--theme-primary);
}

/* 全局样式：头像眨眼效果 */
:global(.avatar-blinking img) {
  animation: avatar-blink 2s ease-in-out;
}

@keyframes skill-tree-glow {
  0%, 100% {
    box-shadow: 0 0 0 rgba(59, 130, 246, 0);
  }
  50% {
    box-shadow: 0 0 30px rgba(59, 130, 246, 0.8);
  }
}

@keyframes avatar-blink {
  0%, 100% { transform: scale(1); }
  10%, 20% { transform: scaleY(0.1); }
  15% { transform: scaleY(0.1) scaleX(1.1); }
}
</style>

