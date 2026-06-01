<template>
  <div class="projects-detail-page">
    <section v-if="loading" class="projects-detail-state">
      <div class="projects-loading-spinner"></div>
      <p class="projects-state-title">项目详情加载中</p>
      <p class="projects-state-text">正在整理项目资料、技术栈与正文内容，请稍候。</p>
    </section>

    <ProjectShowcasePage v-else-if="project" :project="project" />

    <section v-else class="projects-detail-state projects-detail-state--error">
      <div class="projects-empty-icon">!</div>
      <p class="projects-state-title">没有找到这个项目</p>
      <p class="projects-state-text">项目可能已被删除、隐藏，或者当前访问链接不正确。</p>
      <NuxtLink to="/projects" class="projects-detail-button projects-detail-button--primary">
        返回项目列表
      </NuxtLink>
    </section>
  </div>
</template>

<script setup lang="ts">
import type { Project } from '~/types/api'
import { applyProjectCover } from '~/constants/projects/covers'
import ProjectShowcasePage from '~/components/projects/ProjectShowcasePage.vue'
import '~/assets/css/projects.css'

definePageMeta({
  layout: 'default',
})

const route = useRoute()
const api = useApi()
usePageStyle('projects')

const loading = ref(true)
const project = ref<Project | null>(null)

const fetchProject = async () => {
  loading.value = true
  try {
    const projectId = route.params.id as string
    api.post(`/Projects/${projectId}/view`).catch(() => {})
    const response = await api.get<Project>(`/Projects/${projectId}`)
    project.value = applyProjectCover(response)
  } catch (error) {
    if (process.env.NODE_ENV === 'development') {
      console.error('Failed to fetch project:', error)
    }
    project.value = null
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchProject()
})

useHead({
  title: computed(() => `${project.value?.title || '项目详情'} - 项目展示 - 溪午听风`),
  meta: [
    {
      name: 'description',
      content: computed(() => project.value?.description || '项目详情页面'),
    },
  ],
})
</script>

<style scoped>
.projects-detail-page {
  min-height: 100vh;
}

.projects-detail-state {
  position: relative;
  z-index: 1;
  width: min(var(--space-container), calc(100% - 32px));
  margin: 8rem auto 5rem;
  padding: 3rem 2rem;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  background: var(--color-surface);
  text-align: center;
}

.projects-detail-state--error {
  display: grid;
  gap: 0.75rem;
  justify-items: center;
}
</style>
