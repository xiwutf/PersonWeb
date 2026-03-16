<template>
  <section class="project-showcase-section">
    <!-- 区域头部 -->
    <div class="section-header">
      <h2 v-if="title" class="section-title">
        {{ title }}
      </h2>
      <p v-if="subtitle" class="section-subtitle">
        {{ subtitle }}
      </p>
      <div v-if="viewAllAction" class="view-all-action">
        <button @click="viewAllAction.onClick" class="view-all-link">
          {{ viewAllAction.text }}
          <i class="fas fa-arrow-right"></i>
        </button>
      </div>
    </div>

    <!-- 项目网格 -->
    <div class="project-grid" :class="`project-grid--${columns}`">
      <div
        v-for="project in projects"
        :key="project.id"
        class="project-card"
        @click="handleProjectClick(project)"
      >
        <!-- 封面图 -->
        <div v-if="project.coverUrl" class="project-cover">
          <img :src="project.coverUrl" :alt="project.title" class="cover-image" />
        </div>

        <!-- 内容 -->
        <div class="project-content">
          <!-- 标签 -->
          <div v-if="project.tags && project.tags.length" class="project-tags">
            <span
              v-for="tag in project.tags.slice(0, 3)"
              :key="tag"
              class="project-tag"
            >
              {{ tag }}
            </span>
            <span v-if="project.tags.length > 3" class="project-tag-more">
              +{{ project.tags.length - 3 }}
            </span>
          </div>

          <!-- 标题 -->
          <h3 class="project-title">
            {{ project.title }}
          </h3>

          <!-- 描述 -->
          <p v-if="project.description" class="project-description">
            {{ project.description }}
          </p>

          <!-- 信息 -->
          <div class="project-meta">
            <span v-if="project.createdAt" class="project-date">
              <i class="fas fa-calendar"></i>
              {{ formatDate(project.createdAt) }}
            </span>
            <span v-if="project.viewCount !== undefined" class="project-views">
              <i class="fas fa-eye"></i>
              {{ formatNumber(project.viewCount) }}
            </span>
          </div>
        </div>

        <!-- 操作按钮 -->
        <div class="project-actions">
          <button
            v-if="project.demoUrl"
            @click.stop="handleDemoClick(project)"
            class="project-action project-action-demo"
            title="在线演示"
          >
            <i class="fas fa-external-link-alt"></i>
          </button>
          <button
            v-if="project.githubUrl"
            @click.stop="handleGithubClick(project)"
            class="project-action project-action-github"
            title="GitHub 仓库"
          >
            <i class="fab fa-github"></i>
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
export interface Project {
  id: string
  title: string
  description?: string
  coverUrl?: string
  tags?: string[]
  createdAt?: string
  viewCount?: number
  demoUrl?: string
  githubUrl?: string
}

export interface ViewAllAction {
  text: string
  onClick: () => void
}

interface Props {
  projects: Project[]
  title?: string
  subtitle?: string
  columns?: 2 | 3 | 4
  viewAllAction?: ViewAllAction
  class?: string | string[] | Record<string, boolean>
}

const props = withDefaults(defineProps<Props>(), {
  columns: 3
})

const emit = defineEmits<{
  (event: 'project-click', project: Project): void
}>()

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString()
}

const formatNumber = (num: number) => {
  if (!num) return '0'
  if (num >= 10000) return (num / 10000).toFixed(1) + 'w'
  if (num >= 1000) return (num / 1000).toFixed(1) + 'k'
  return num.toString()
}

const handleProjectClick = (project: Project) => {
  emit('project-click', project)
}

const handleDemoClick = (project: Project) => {
  if (project.demoUrl) {
    window.open(project.demoUrl, '_blank')
  }
}

const handleGithubClick = (project: Project) => {
  if (project.githubUrl) {
    window.open(project.githubUrl, '_blank')
  }
}
</script>

<style scoped>
.project-showcase-section {
  padding: var(--section-padding, var(--spacing-20) 0);
}

.section-header {
  text-align: center;
  margin-bottom: var(--spacing-3xl);
}

.section-title {
  font-size: var(--text-3xl);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm);
}

.section-subtitle {
  font-size: var(--text-lg);
  color: var(--color-text-muted);
  margin: 0;
}

.view-all-action {
  margin-top: var(--spacing-lg);
}

.view-all-link {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: var(--spacing-sm) var(--spacing-md);
  background: var(--color-bg-card);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-default);
  border-radius: var(--radius-md);
  font-size: var(--text-base);
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.view-all-link:hover {
  background: var(--color-bg-elevated);
  border-color: var(--color-primary);
  color: var(--color-primary);
}

.project-grid {
  display: grid;
  gap: var(--spacing-xl);
}

.project-grid--2 {
  grid-template-columns: repeat(2, 1fr);
}

.project-grid--3 {
  grid-template-columns: repeat(3, 1fr);
}

.project-grid--4 {
  grid-template-columns: repeat(4, 1fr);
}

.project-card {
  background: var(--project-card-bg, var(--color-bg-card));
  border: 1px solid var(--project-card-border, var(--color-border-subtle));
  border-radius: var(--radius-xl);
  overflow: hidden;
  transition: all 0.3s;
  display: flex;
  flex-direction: column;
}

.project-card:hover {
  background: var(--project-card-hover-bg, var(--color-bg-elevated));
  border-color: var(--color-primary);
  transform: translateY(-4px);
  box-shadow: var(--shadow-lg);
}

.project-cover {
  position: relative;
  overflow: hidden;
}

.cover-image {
  width: 100%;
  height: 200px;
  object-fit: cover;
}

.project-content {
  padding: var(--spacing-xl);
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.project-tags {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-xs);
  margin-bottom: var(--spacing-sm);
}

.project-tag {
  display: inline-flex;
  align-items: center;
  padding: var(--spacing-xs) var(--spacing-sm);
  background: var(--project-tag-bg, var(--color-bg-elevated));
  color: var(--project-tag-color, var(--color-text-secondary));
  border-radius: var(--radius-full);
  font-size: var(--text-xs);
  font-weight: 500;
}

.project-tag-more {
  padding: var(--spacing-xs) var(--spacing-sm);
  color: var(--color-text-muted);
}

.project-title {
  font-size: var(--text-xl);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0;
  line-height: 1.4;
}

.project-description {
  font-size: var(--text-base);
  color: var(--color-text-muted);
  line-height: 1.6;
  margin: 0;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.project-meta {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  margin-top: auto;
  font-size: var(--text-sm);
  color: var(--color-text-muted);
}

.project-meta i {
  margin-right: var(--spacing-xs);
}

.project-actions {
  display: flex;
  gap: var(--spacing-xs);
  padding: var(--spacing-md) var(--spacing-xl);
  border-top: 1px solid var(--color-border-subtle);
}

.project-action {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: var(--radius-md);
  background: var(--color-bg-elevated);
  color: var(--color-text-main);
  border: 1px solid var(--color-border-subtle);
  cursor: pointer;
  transition: all 0.2s;
}

.project-action:hover {
  background: var(--color-primary);
  color: var(--color-text-on-primary);
  border-color: var(--color-primary);
}

/* Responsive */
@media (max-width: 1024px) {
  .project-grid--3,
  .project-grid--4 {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .project-grid--2,
  .project-grid--3,
  .project-grid--4 {
    grid-template-columns: 1fr;
  }

  .project-content {
    padding: var(--spacing-lg);
  }

  .project-description {
    -webkit-line-clamp: 2;
  }
}
</style>
