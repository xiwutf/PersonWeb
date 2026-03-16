<template>
  <div class="ai-project-list">
    <!-- Tag 筛选器 -->
    <div class="ai-project-filters">
      <button
        v-for="tag in filterTags"
        :key="tag"
        @click="selectedTag = tag"
        class="ai-project-filter-tag"
        :class="{ 'ai-project-filter-tag--active': selectedTag === tag }"
      >
        {{ tag }}
      </button>
    </div>

    <!-- 项目列表 -->
    <div class="ai-project-grid">
      <div
        v-for="project in filteredProjects"
        :key="project.id"
        class="ai-project-card"
        @click="handleProjectClick(project)"
      >
        <div class="ai-project-card-bg"></div>
        <div class="ai-project-card-content">
          <!-- 状态标签 -->
          <div class="ai-project-status">
            <span 
              class="ai-project-status-badge"
              :class="getStatusClass(project.status)"
            >
              {{ project.status }}
            </span>
          </div>

          <!-- 标题和描述 -->
          <h3 class="ai-project-title">{{ project.title }}</h3>
          <p class="ai-project-summary">{{ project.summary }}</p>

          <!-- 标签 -->
          <div class="ai-project-tags">
            <span
              v-for="tag in project.tags"
              :key="tag"
              class="ai-project-tag"
            >
              {{ tag }}
            </span>
          </div>

          <!-- 技术栈 -->
          <div class="ai-project-stack">
            <span class="ai-project-stack-label">技术栈：</span>
            <span
              v-for="(tech, index) in project.stack"
              :key="tech"
              class="ai-project-stack-item"
            >
              {{ tech }}<span v-if="index < project.stack.length - 1">, </span>
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- 空状态 -->
    <div v-if="filteredProjects.length === 0" class="ai-project-empty">
      <div class="ai-project-empty-icon">🤖</div>
      <p class="ai-project-empty-text">暂无 {{ selectedTag === '全部' ? '' : selectedTag }} 相关项目</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

// 定义项目类型
interface AiProject {
  id: string
  title: string
  summary: string
  tags: string[]
  stack: string[]
  status: string
  path?: string  // 可选：项目路径，用于跳转
}

// Props
const props = defineProps<{
  projects?: AiProject[]
}>()

// 默认项目数据（占位数据）
const defaultProjects: AiProject[] = [
  {
    id: 'name-tool',
    title: '智能取名助手',
    summary: '游戏名、网名、英文名一键生成，支持多维度评分、风格筛选、收藏管理。',
    tags: ['AI工具', '文本生成'],
    stack: ['Python', 'FastAPI', 'OpenAI API', 'Vue3'],
    status: '已上线',
    path: '/tools/name'
  },
  {
    id: '1',
    title: '个人知识库智能问答助手',
    summary: '接入个人笔记与文档，通过 RAG 技术实现"像问人一样问资料"，支持中文语义检索。',
    tags: ['RAG', '知识库', 'Python'],
    stack: ['Python', 'OpenAI API', '向量数据库'],
    status: '已上线'
  },
  {
    id: '2',
    title: '多智能体任务分解助手',
    summary: '设计多个角色 AI 协作的工作流，一个负责拆任务，一个负责查资料，一个负责写方案。',
    tags: ['多智能体', '工作流'],
    stack: ['Python', 'LangChain', 'OpenAI API'],
    status: '实验中'
  },
  {
    id: '3',
    title: '业务日报自动生成系统',
    summary: '结合 AI + 脚本，自动收集工作数据，生成结构化日报，支持多平台推送。',
    tags: ['自动化', '脚本代理'],
    stack: ['Python', 'FastAPI', 'OpenAI API'],
    status: '内部自用'
  },
  {
    id: '4',
    title: '智能代码审查助手',
    summary: '基于大模型的代码审查工具，自动分析代码质量、潜在问题，并提供改进建议。',
    tags: ['开发者工具', '代码分析'],
    stack: ['TypeScript', 'VSCode Extension API', 'OpenAI API'],
    status: '已上线'
  },
  {
    id: '5',
    title: '文档智能提取与问答',
    summary: '从 PDF、Word 等文档中提取关键信息，构建知识图谱，支持多轮对话问答。',
    tags: ['RAG', '文档处理', '知识图谱'],
    stack: ['Python', 'LangChain', '向量数据库', 'PDF解析'],
    status: '实验中'
  },
  {
    id: '6',
    title: 'AI 客服机器人',
    summary: '面向业务的智能客服系统，支持多轮对话、意图识别、知识库检索。',
    tags: ['智能体', '业务自动化'],
    stack: ['Python', 'FastAPI', 'OpenAI API', 'WebSocket'],
    status: '已上线'
  }
]

// 使用传入的项目或默认项目
const projects = computed(() => props.projects || defaultProjects)

// 筛选标签
const filterTags = ref(['全部', '智能体', '知识库', '自动化', '插件工具'])
const selectedTag = ref('全部')

// 筛选后的项目
const filteredProjects = computed(() => {
  if (selectedTag.value === '全部') {
    return projects.value
  }
  return projects.value.filter(project => 
    project.tags.some(tag => 
      tag.includes(selectedTag.value) || 
      selectedTag.value.includes(tag)
    )
  )
})

// 获取状态样式类
const getStatusClass = (status: string) => {
  if (status === '已上线') return 'ai-project-status-badge--success'
  if (status === '实验中') return 'ai-project-status-badge--warning'
  if (status === '内部自用') return 'ai-project-status-badge--info'
  return 'ai-project-status-badge--default'
}

// 处理项目点击
const router = useRouter()
const handleProjectClick = (project: AiProject) => {
  // 如果有 path，跳转到对应页面；否则显示详情
  if (project.path) {
    router.push(project.path)
  } else {
    // 可以跳转到项目详情页或打开弹窗
    console.log('点击项目:', project)
  }
}
</script>

<style scoped>
.ai-project-list {
  width: 100%;
}

/* 筛选器 */
.ai-project-filters {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-xl);
  padding-bottom: var(--spacing-lg);
  border-bottom: 1px solid var(--divider-color);
}

.ai-project-filter-tag {
  padding: var(--spacing-sm) 40px;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
  color: var(--text-secondary);
  cursor: pointer;
  transition: all 0.3s ease;
}

.ai-project-filter-tag:hover {
  background: var(--bg-elevated);
  border-color: var(--primary);
  color: var(--text-main);
}

.ai-project-filter-tag--active {
  background: var(--primary-soft-bg);
  border-color: var(--primary);
  color: var(--primary);
}

/* 项目网格 */
.ai-project-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: var(--spacing-lg);
}

@media (max-width: 640px) {
  .ai-project-grid {
    grid-template-columns: 1fr;
  }
}

/* 项目卡片 */
.ai-project-card {
  position: relative;
  padding: var(--spacing-lg);
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  cursor: pointer;
  transition: all 0.3s ease;
  overflow: hidden;
}

.ai-project-card:hover {
  background: var(--bg-elevated);
  border-color: var(--primary);
  transform: translateY(-8px);
  box-shadow: var(--shadow-soft);
}

.ai-project-card-bg {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, var(--primary-soft-bg) 0%, rgba(37, 99, 235, 0.05) 100%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.ai-project-card:hover .ai-project-card-bg {
  opacity: 0.3;
}

.ai-project-card-content {
  position: relative;
  z-index: 1;
}

.ai-project-status {
  margin-bottom: var(--spacing-md);
}

.ai-project-status-badge {
  display: inline-block;
  padding: var(--spacing-xs) var(--spacing-md);
  border-radius: var(--radius-sm);
  font-size: var(--text-xs);
  font-weight: 500;
}

/* 浅色主题下的状态标签 */
:global([data-theme="light"]) .ai-project-status-badge--success {
  background: var(--color-success-bg);
  border: 1px solid var(--color-success-border);
  color: var(--color-success-text);
}

:global([data-theme="light"]) .ai-project-status-badge--warning {
  background: var(--color-warning-bg);
  border: 1px solid var(--color-warning-border);
  color: var(--color-warning-text);
}

html[data-theme="light"] .ai-project-status-badge--info {
  background: var(--primary-soft-bg);
  border: 1px solid rgba(37, 99, 235, 0.3);
  color: var(--primary);
}

html[data-theme="light"] .ai-project-status-badge--default {
  background: var(--primary-soft-bg);
  border: 1px solid var(--border-color);
  color: var(--text-secondary);
}

/* 深色主题下的状态标签 */
:global(.dark) .ai-project-status-badge--success,
:global([data-theme="dark"]) .ai-project-status-badge--success {
  background: var(--color-success-15, rgba(34, 197, 94, 0.15));
  border: 1px solid var(--color-success-30, rgba(34, 197, 94, 0.3));
  color: var(--color-success);
}

:global(.dark) .ai-project-status-badge--warning,
:global([data-theme="dark"]) .ai-project-status-badge--warning {
  background: var(--color-warning-bg);
  border: 1px solid var(--color-warning-border);
  color: var(--color-warning-text);
}

:global(.dark) .ai-project-status-badge--info,
:global([data-theme="dark"]) .ai-project-status-badge--info {
  background: var(--color-primary-15, rgba(59, 130, 246, 0.15));
  border: 1px solid var(--color-primary, var(--theme-primary));
  color: var(--color-primary);
}

:global(.dark) .ai-project-status-badge--default,
:global([data-theme="dark"]) .ai-project-status-badge--default {
  background: var(--color-gray-500-15, rgba(148, 163, 184, 0.15));
  border: 1px solid var(--color-gray-500-30, rgba(148, 163, 184, 0.3));
  color: var(--color-text-muted);
}

.ai-project-title {
  font-size: var(--text-3xl);
  font-weight: 600;
  color: var(--text-main);
  margin: 0 0 var(--spacing-md) 0;
  line-height: 1.4;
}

.ai-project-summary {
  font-size: var(--text-sm);
  line-height: 1.6;
  color: var(--text-secondary);
  margin: 0 0 var(--spacing-lg) 0;
}

.ai-project-tags {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-sm);
  margin-bottom: var(--spacing-md);
}

.ai-project-tag {
  padding: var(--spacing-xs) var(--spacing-md);
  border-radius: var(--radius-sm);
  font-size: var(--text-xs);
  /* 浅色主题优化（2025-01）：标签使用主色柔和背景，提高可读性 */
  background: var(--primary-soft-bg, rgba(6, 182, 212, 0.1));
  border: 1px solid rgba(37, 99, 235, 0.2);
  color: var(--primary, #06b6d4);
}

/* 深色主题保持原有样式 */
:global(.dark) .ai-project-tag,
:global([data-theme="dark"]) .ai-project-tag {
  background: var(--color-cyan-500-10, rgba(6, 182, 212, 0.1));
  border: 1px solid var(--color-cyan-500-30, rgba(6, 182, 212, 0.3));
  color: var(--color-cyan-500, #06b6d4);
}

.ai-project-stack {
  font-size: var(--text-xs);
  color: var(--text-secondary);
  line-height: 1.6;
}

.ai-project-stack-label {
  color: var(--text-muted);
}

.ai-project-stack-item {
  color: var(--text-secondary);
}

/* 空状态 */
.ai-project-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: calc(var(--spacing-20) * 2) var(--spacing-lg);
  text-align: center;
}

.ai-project-empty-icon {
  font-size: var(--text-8xl);
  margin-bottom: var(--spacing-md);
  opacity: 0.5;
}

.ai-project-empty-text {
  font-size: var(--text-base);
  color: var(--text-secondary);
  margin: 0;
}
</style>
