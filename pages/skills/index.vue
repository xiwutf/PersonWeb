<template>
  <div class="skills-page">
    <div class="skills-background-noise"></div>
    <div class="skills-background">
      <div class="skills-background-blob skills-background-blob--violet"></div>
      <div class="skills-background-blob skills-background-blob--blue"></div>
      <div class="skills-background-blob skills-background-blob--emerald"></div>
      <div class="skills-background-grid"></div>
    </div>

    <div class="skills-shell">
      <section class="skills-hero skills-hero--map">
        <div class="skills-hero-copy">
          <div class="skills-hero-badge">
            <span class="skills-hero-badge-dot"></span>
            能力地图
          </div>
          <h1 class="skills-title">把能力整理成可理解、可验证、可协作的地图</h1>
          <p class="skills-subtitle">
            这页不再强调“我给自己打几分”，而是把我能处理的问题类型、核心技术模块和代表能力点整理成一张更适合对外沟通的能力地图。
          </p>
        </div>

        <div class="skills-overview-panel">
          <div class="skills-overview-stat">
            <span class="skills-overview-label">能力方向</span>
            <strong class="skills-overview-value">{{ filteredCategories.length }}</strong>
          </div>
          <div class="skills-overview-stat">
            <span class="skills-overview-label">覆盖技能点</span>
            <strong class="skills-overview-value">{{ totalSkills }}</strong>
          </div>
          <div class="skills-overview-stat">
            <span class="skills-overview-label">当前最强模块</span>
            <strong class="skills-overview-value">{{ strongestCategoryName }}</strong>
          </div>
          <div class="skills-overview-note">
            更适合回答这些问题：
            “你擅长做什么？”
            “你能解决哪类问题？”
            “你的能力更偏工程、产品，还是 AI 应用？”
          </div>
        </div>
      </section>

      <div v-if="loading" class="skills-loading">
        <div class="skills-loading-spinner"></div>
        <p class="skills-loading-text">正在整理能力地图...</p>
      </div>

      <template v-else>
        <section class="skills-toolbar-card">
          <div class="skills-toolbar-copy">
            <h2 class="skills-toolbar-title">按方向查看能力模块</h2>
            <p class="skills-toolbar-text">
              你可以按分类筛选，快速看到我在哪些方向上更成熟，以及每个方向下最值得提的技能点。
            </p>
          </div>

          <div class="skills-toolbar-actions">
            <select v-model="selectedCategoryId" class="skills-toolbar-select">
              <option :value="null">全部方向</option>
              <option v-for="category in categories" :key="category.id" :value="category.id">
                {{ category.icon }} {{ category.name }}
              </option>
            </select>
          </div>
        </section>

        <section class="skills-highlight-grid">
          <article class="skills-highlight-card">
            <span class="skills-highlight-kicker">Top Capability</span>
            <h3 class="skills-highlight-title">{{ topCapability.name }}</h3>
            <p class="skills-highlight-text">
              {{ topCapability.description || '这是当前能力地图中表现最稳定、最容易拿来解决实际问题的核心能力点。' }}
            </p>
          </article>

          <article class="skills-highlight-card">
            <span class="skills-highlight-kicker">Best Fit</span>
            <h3 class="skills-highlight-title">{{ strongestCategoryName }}</h3>
            <p class="skills-highlight-text">
              这一块更适合承接复杂实现、系统整合或从想法到落地的完整交付。
            </p>
          </article>

          <article class="skills-highlight-card">
            <span class="skills-highlight-kicker">Collaboration Mode</span>
            <h3 class="skills-highlight-title">工程实现 + 结构化表达</h3>
            <p class="skills-highlight-text">
              不只关心技术点本身，也会关注可维护性、信息组织和真实协作效率。
            </p>
          </article>
        </section>

        <section class="skills-map-grid">
          <article
            v-for="category in filteredCategories"
            :key="category.id"
            class="skills-map-card"
            :style="{ '--skills-category-accent': category.color || '#8b5cf6' }"
          >
            <div class="skills-map-card-head">
              <div class="skills-map-card-symbol">
                <span class="skills-map-card-icon">{{ category.icon || '✦' }}</span>
              </div>
              <div class="skills-map-card-copy">
                <div class="skills-map-card-meta">Capability Domain</div>
                <h2 class="skills-map-card-title">{{ category.name }}</h2>
                <p class="skills-map-card-summary">
                  {{ buildCategorySummary(category) }}
                </p>
              </div>
            </div>

            <div class="skills-map-card-stats">
              <div class="skills-map-card-stat">
                <span class="skills-map-card-stat-label">技能数</span>
                <strong class="skills-map-card-stat-value">{{ category.skills.length }}</strong>
              </div>
              <div class="skills-map-card-stat">
                <span class="skills-map-card-stat-label">代表技能</span>
                <strong class="skills-map-card-stat-value">{{ getTopSkillName(category) }}</strong>
              </div>
            </div>

            <div class="skills-capability-list">
              <article
                v-for="skill in getFeaturedSkills(category)"
                :key="skill.id"
                class="skills-capability-item"
              >
                <div class="skills-capability-top">
                  <div class="skills-capability-name-wrap">
                    <span v-if="skill.icon" class="skills-capability-icon">{{ skill.icon }}</span>
                    <h3 class="skills-capability-name">{{ skill.name }}</h3>
                  </div>
                  <span class="skills-capability-level" :class="getCapabilityLevelClass(skill.currentRating)">
                    {{ getCapabilityLevelText(skill.currentRating) }}
                  </span>
                </div>

                <p v-if="skill.description" class="skills-capability-description">
                  {{ skill.description }}
                </p>

                <div class="skills-capability-progress">
                  <div class="skills-capability-progress-track">
                    <div
                      class="skills-capability-progress-fill"
                      :style="{ width: `${Math.max(8, (skill.currentRating || 0) * 10)}%` }"
                    ></div>
                  </div>
                </div>
              </article>
            </div>
          </article>
        </section>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
type SkillItem = {
  id: number | string
  name: string
  description?: string
  icon?: string
  currentRating: number
}

type SkillCategory = {
  id: number | string
  name: string
  icon?: string
  color?: string
  skills: SkillItem[]
}

const api = useApi()
const loading = ref(true)
const skillTree = ref<SkillCategory[]>([])
const categories = ref<Array<{ id: number | string; name: string; icon?: string }>>([])
const selectedCategoryId = ref<number | string | null>(null)

const normalizeSkillTree = (items: any[]): SkillCategory[] => {
  return (items || []).map((category: any) => ({
    id: category.id,
    name: category.name || '未命名方向',
    icon: category.icon || '✦',
    color: category.color || '#8b5cf6',
    skills: (category.skills || []).map((skill: any) => ({
      id: skill.id,
      name: skill.name || '未命名技能',
      description: skill.description || '',
      icon: skill.icon || '',
      currentRating: Number(skill.currentRating || 0)
    }))
  }))
}

const filteredCategories = computed(() => {
  if (!selectedCategoryId.value) return skillTree.value
  return skillTree.value.filter(category => category.id === selectedCategoryId.value)
})

const totalSkills = computed(() =>
  filteredCategories.value.reduce((sum, category) => sum + category.skills.length, 0)
)

const allSkills = computed(() =>
  filteredCategories.value
    .flatMap(category => category.skills.map(skill => ({ ...skill, categoryName: category.name })))
    .sort((a, b) => (b.currentRating || 0) - (a.currentRating || 0))
)

const topCapability = computed(() =>
  allSkills.value[0] || {
    id: 'empty',
    name: '暂无能力数据',
    description: '后续补充技能数据后，这里会展示当前最值得强调的能力点。',
    currentRating: 0
  }
)

const strongestCategoryName = computed(() => {
  if (!filteredCategories.value.length) return '暂无'

  const sorted = [...filteredCategories.value].sort((a, b) => {
    const avgA = getCategoryAverage(a)
    const avgB = getCategoryAverage(b)
    return avgB - avgA
  })

  return sorted[0]?.name || '暂无'
})

const getCategoryAverage = (category: SkillCategory) => {
  if (!category.skills.length) return 0
  return category.skills.reduce((sum, skill) => sum + (skill.currentRating || 0), 0) / category.skills.length
}

const getTopSkillName = (category: SkillCategory) => {
  const topSkill = [...category.skills].sort((a, b) => (b.currentRating || 0) - (a.currentRating || 0))[0]
  return topSkill?.name || '暂无'
}

const getFeaturedSkills = (category: SkillCategory) => {
  return [...category.skills]
    .sort((a, b) => (b.currentRating || 0) - (a.currentRating || 0))
    .slice(0, 4)
}

const buildCategorySummary = (category: SkillCategory) => {
  const topSkill = getTopSkillName(category)
  if (!category.skills.length) {
    return '这一方向下暂时还没有整理出明确的技能点。'
  }
  return `这一模块更适合承接与 ${topSkill} 相关的实际问题，也代表了该方向下最成熟的一组能力。`
}

const getCapabilityLevelText = (rating: number) => {
  if (rating >= 8) return '核心能力'
  if (rating >= 6) return '稳定可用'
  if (rating >= 4) return '持续强化'
  return '探索中'
}

const getCapabilityLevelClass = (rating: number) => {
  if (rating >= 8) return 'skills-capability-level--strong'
  if (rating >= 6) return 'skills-capability-level--solid'
  if (rating >= 4) return 'skills-capability-level--growing'
  return 'skills-capability-level--early'
}

const fetchSkillTree = async () => {
  loading.value = true
  try {
    const res = await api.get<any[]>('/SkillTree')
    skillTree.value = normalizeSkillTree(res || [])
  } catch (e) {
    console.error('Failed to fetch skill tree:', e)
    skillTree.value = []
  } finally {
    loading.value = false
  }
}

const fetchCategories = async () => {
  try {
    const res = await api.get<any[]>('/SkillTree/categories')
    categories.value = (res || []).map((category: any) => ({
      id: category.id,
      name: category.name || '未命名方向',
      icon: category.icon || '✦'
    }))
  } catch (e) {
    console.error('Failed to fetch categories:', e)
    categories.value = []
  }
}

onMounted(async () => {
  await fetchCategories()
  await fetchSkillTree()
})

useHead({
  title: '能力地图 - 溪午听风',
  meta: [
    { name: 'description', content: '把技术能力整理成可理解、可验证、可协作的能力地图。' }
  ]
})
</script>
