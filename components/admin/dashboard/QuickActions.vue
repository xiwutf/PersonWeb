<template>
  <div class="quick-actions-card">
    <div class="card-header">
      <h3 class="card-title">快捷操作</h3>
    </div>
    <div class="actions-grid">
      <NuxtLink
        v-for="action in actions"
        :key="action.path"
        :to="action.path"
        class="action-button"
      >
        <div class="action-icon">{{ action.icon }}</div>
        <div class="action-label">{{ action.label }}</div>
      </NuxtLink>
    </div>
  </div>
</template>

<script setup lang="ts">
interface ActionItem {
  path: string
  icon: string
  label: string
}

interface Props {
  actions: ActionItem[]
}

withDefaults(defineProps<Props>(), {
  actions: () => []
})
</script>

<style scoped>
.quick-actions-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  padding: 1.5rem;
  margin-bottom: 2rem;
}

.card-header {
  margin-bottom: 1.5rem;
}

.card-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--color-text-main, #f3f4f6);
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

@media (min-width: 640px) {
  .actions-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

.action-button {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  padding: 1.5rem 1rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.75rem;
  text-decoration: none;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.action-button::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, var(--color-primary-10, rgba(59, 130, 246, 0.1)), var(--color-purple-500-10, rgba(168, 85, 247, 0.1)));
  opacity: 0;
  transition: opacity 0.3s ease;
}

.action-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.3), 0 4px 6px -2px rgba(0, 0, 0, 0.2);
  background: var(--color-border);
  border-color: rgba(255, 255, 255, 0.2);
}

.action-button:hover::before {
  opacity: 1;
}

.action-icon {
  font-size: 2rem;
  position: relative;
  z-index: 1;
}

.action-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--color-border);
  position: relative;
  z-index: 1;
}
</style>

