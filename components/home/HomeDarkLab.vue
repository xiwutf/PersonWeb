<template>
  <div class="home-v4">
    <AddToHomeScreen />

    <section class="panel hero-panel" id="hero">
      <div class="shell hero-shell">
        <div class="hero-copy">
          <p class="hero-pill">欢迎来到我的数字花园</p>
          <h1 class="hero-title">
            <span class="hero-title-prefix">你好，我是</span>
            <span class="hero-name">溪午听风</span>
          </h1>
          <p class="hero-role">我是一个全栈开发者 · AI 应用探索者</p>
          <p class="hero-subtitle">
            我把 AI、业务系统与自动化工具整合成可持续演进的产品能力。
            从架构设计、全栈开发到体验落地，目标始终是让复杂问题拥有清晰、稳定、可复用的解法。
          </p>
          <div class="hero-tags">
            <span>AI 应用工程化</span>
            <span>全栈产品交付</span>
            <span>自动化效率提升</span>
            <span>长期主义构建</span>
          </div>
          <div class="hero-actions">
            <NuxtLink to="/projects" class="btn btn-primary">浏览项目</NuxtLink>
            <NuxtLink to="/contact" class="btn btn-ghost">联系合作</NuxtLink>
          </div>
        </div>

        <aside class="hero-card">
          <div class="hero-card-shell">
            <div class="hero-dot-grid" aria-hidden="true"></div>
            <p class="hero-card-kicker">AVAILABLE FOR PRODUCT BUILDING</p>
            <span class="hero-code-badge">&lt;/&gt;</span>

            <div class="hero-avatar-wrap">
              <span class="hero-avatar-glow" aria-hidden="true"></span>
              <div class="hero-avatar-frame">
                <img src="/images/avatar.jpg" alt="溪午听风头像" class="hero-avatar-image" />
              </div>
            </div>

            <h3>溪午听风</h3>
            <p>全栈开发 · AI 应用落地</p>

            <div class="hero-card-meta">
              <span>Vue · Nuxt</span>
              <span>.NET · Node.js</span>
              <span>AI 工具链</span>
            </div>

            <div class="hero-cap-grid">
              <article class="hero-cap-item">
                <p class="hero-cap-kicker">BUILD</p>
                <p class="hero-cap-text">面向产品与业务目标设计可扩展系统方案</p>
              </article>
              <article class="hero-cap-item">
                <p class="hero-cap-kicker">SHIP</p>
                <p class="hero-cap-text">把想法落地为真正上线、可维护、可持续演进的能力</p>
              </article>
            </div>
          </div>
        </aside>
      </div>
    </section>

    <section class="panel matrix-panel" id="matrix">
      <div class="shell">
        <header class="section-head">
          <p class="section-kicker">The Matrix</p>
          <h2>能力与现状矩阵</h2>
        </header>
        <div class="bento-grid">
          <article class="bento-card bento-large">
            <p class="card-kicker">当前研究方向</p>
            <h3>AI Agent + 交付系统化</h3>
            <p class="card-desc">围绕咨询、任务拆解、执行与复盘，构建可复用的 AI 工作流底座。</p>
            <ul class="progress-list">
              <li v-for="item in researchTracks" :key="item.title">
                <div class="progress-meta">
                  <span>{{ item.title }}</span>
                  <span>{{ item.progress }}%</span>
                </div>
                <div class="progress-bar"><span :style="{ width: `${item.progress}%` }"></span></div>
              </li>
            </ul>
          </article>

          <article class="bento-card bento-wide">
            <p class="card-kicker">技术栈雷达</p>
            <h3>Vue · Nuxt · .NET · Node</h3>
            <div class="stack-radar">
              <span v-for="item in stackRadar" :key="item.name" class="stack-chip">
                <span class="stack-icon">{{ item.icon }}</span>{{ item.name }}
              </span>
            </div>
          </article>

          <article class="bento-card">
            <p class="card-kicker">地理位置</p>
            <h3>中国 · 保定</h3>
            <div class="micro-map"><div class="map-pin"></div><div class="map-lines"></div></div>
          </article>

          <article class="bento-card">
            <p class="card-kicker">阅读 / 理财</p>
            <h3>《纳瓦尔宝典》</h3>
            <p class="card-desc">长期主义、杠杆思维、可复利的系统建设。</p>
            <div class="book-cover">READING NOW</div>
          </article>
        </div>
      </div>
    </section>

    <section class="panel delivery-panel" id="delivery">
      <div class="shell">
        <header class="section-head">
          <p class="section-kicker">Delivery Showcase</p>
          <h2>精选交付</h2>
        </header>
        <div class="delivery-list">
          <article v-for="(item, index) in deliveries" :key="item.title" class="delivery-card">
            <div class="demo-box">
              <div class="demo-window">
                <div class="demo-head">
                  <div class="demo-dots">
                    <span></span>
                    <span></span>
                    <span></span>
                  </div>
                  <span class="demo-label">Case 0{{ index + 1 }}</span>
                </div>
                <div class="demo-content">
                  <div class="demo-main">
                    <div class="demo-main-top"></div>
                    <div class="demo-chart">
                      <span class="bar bar-1"></span>
                      <span class="bar bar-2"></span>
                      <span class="bar bar-3"></span>
                      <span class="bar bar-4"></span>
                    </div>
                  </div>
                  <div class="demo-side">
                    <div class="demo-side-card">
                      <p>效率提升</p>
                      <strong>+{{ 24 + index * 9 }}%</strong>
                    </div>
                    <div class="demo-side-card">
                      <p>稳定性</p>
                      <strong>{{ 96 + index }}%</strong>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="delivery-copy">
              <h3>{{ item.title }}</h3>
              <p class="delivery-tag">{{ item.tag }}</p>
              <p>{{ item.problem }}</p>
              <p class="delivery-metric">{{ item.metric }}</p>
            </div>
          </article>
        </div>
      </div>
    </section>

    <section class="panel thought-panel" id="thoughts">
      <div class="shell">
        <header class="section-head">
          <p class="section-kicker">Thought Stream</p>
          <h2>思维流</h2>
        </header>
        <div class="thought-tracks">
          <article v-for="track in thoughtTracks" :key="track.key" class="thought-card">
            <h3>{{ track.label }}</h3>
            <p>{{ track.description }}</p>
            <NuxtLink
              v-for="post in track.posts"
              :key="post.id"
              :to="`/blog/${post.slug || post.id}`"
              class="thought-link"
            >
              <span>{{ post.title }}</span>
              <span>{{ formatDate(post.publishTime || post.createdAt) }}</span>
            </NuxtLink>
            <NuxtLink to="/blog" class="thought-more">查看更多</NuxtLink>
          </article>
        </div>
      </div>
    </section>

    <section class="panel cta-panel" id="contact">
      <div class="shell cta-shell">
        <div>
          <p class="section-kicker">Digital Twin</p>
          <h2>把问题直接交给“我的数字分身”</h2>
          <p>你可以直接问我的技术栈、项目经验或技术判断，我会给出可执行建议。</p>
        </div>
        <form class="quick-form" @submit.prevent="submitQuickConsultation">
          <h3>快速咨询</h3>
          <div class="quick-grid">
            <input v-model.trim="quickForm.customerName" type="text" placeholder="称呼" />
            <select v-model="quickForm.contactType">
              <option value="wechat">微信</option>
              <option value="phone">电话</option>
              <option value="email">邮箱</option>
            </select>
            <input v-model.trim="quickForm.contactValue" type="text" :placeholder="contactPlaceholder" />
            <input v-model.trim="quickForm.budgetRange" type="text" placeholder="预算范围（可选）" />
          </div>
          <textarea v-model.trim="quickForm.requirementDescription" rows="3" placeholder="需求描述" />
          <p v-if="quickError" class="quick-msg quick-error">{{ quickError }}</p>
          <p v-if="quickSuccess" class="quick-msg quick-success">提交成功，我们会尽快联系你。</p>
          <button type="submit" class="btn btn-primary" :disabled="quickSubmitting">
            {{ quickSubmitting ? '提交中...' : '提交咨询' }}
          </button>
        </form>
      </div>
    </section>

  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import AddToHomeScreen from '~/components/tools/AddToHomeScreen.vue'

interface HomePost {
  id: number | string
  slug?: string
  title: string
  publishTime?: string
  createdAt?: string
}

type ContactType = 'wechat' | 'phone' | 'email'

const api = useApi()
const route = useRoute()

const researchTracks = [
  { title: '多 Agent 协作编排', progress: 78 },
  { title: '咨询到交付自动化', progress: 66 },
  { title: '个人知识系统检索', progress: 83 }
]

const stackRadar = [
  { name: 'Vue', icon: '△' },
  { name: 'Nuxt', icon: '◉' },
  { name: '.NET', icon: '◆' },
  { name: 'Node', icon: '⬢' }
]

const deliveries = [
  {
    title: '个人工作室交付中台',
    tag: 'AI 工作流 / 自动化',
    problem: '解决内容、工具、咨询入口分散，难以形成稳定转化与复用的问题。',
    metric: 'Metric: 咨询到执行路径缩短 40%'
  },
  {
    title: 'AI 文档处理系统',
    tag: '文档解析 / 知识抽取',
    problem: '解决信息分散、阅读负担高、决策前置成本高的问题。',
    metric: 'Metric: 文档处理效率提升 3.2x'
  },
  {
    title: '业务自动化执行台',
    tag: '自动化 / 运维提效',
    problem: '解决重复流程人工执行，无法稳定回溯的问题。',
    metric: 'Metric: 人工重复操作减少 55%'
  }
]

const { data: latestPostsData } = await useAsyncData(
  'home-thought-latest-posts-v4',
  async () => {
    try {
      const res = await api.get<{ list?: HomePost[] }>('/Articles', { params: { page: 1, pageSize: 9 } })
      return res.list || []
    } catch {
      return []
    }
  },
  { default: () => [] }
)

const latestPosts = computed(() => latestPostsData.value || [])

const thoughtTracks = computed(() => {
  const posts = latestPosts.value
  return [
    {
      key: 'ai',
      label: '[AI 工程化]',
      description: '从模型能力到业务价值，关注可上线、可维护、可复盘。',
      posts: posts.slice(0, 2)
    },
    {
      key: 'system',
      label: '[个人系统 / 理财]',
      description: '构建长期主义的个人系统，关注复利与节奏管理。',
      posts: posts.slice(2, 4)
    },
    {
      key: 'depth',
      label: '[技术深度]',
      description: '记录架构抉择与工程细节，沉淀可复用的方法论。',
      posts: posts.slice(4, 6)
    }
  ]
})

const quickForm = ref({
  customerName: '',
  contactType: 'wechat' as ContactType,
  contactValue: '',
  budgetRange: '',
  requirementDescription: ''
})

const quickSubmitting = ref(false)
const quickError = ref('')
const quickSuccess = ref(false)

const contactPlaceholder = computed(() => {
  if (quickForm.value.contactType === 'phone') return '请输入手机号'
  if (quickForm.value.contactType === 'email') return '请输入邮箱'
  return '请输入微信号'
})

const getOrCreateVisitorId = () => {
  if (!process.client) return ''
  let visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) {
    visitorId = typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : `visitor_${Date.now()}`
    localStorage.setItem('visitor_id', visitorId)
  }
  return visitorId
}

const trackHomeEvent = async (behaviorType: string, triggeredEvent: string, behaviorData?: Record<string, unknown>) => {
  const visitorId = getOrCreateVisitorId()
  if (!visitorId) return
  try {
    await api.post('/VisitorGamification/behavior', {
      visitorId,
      behaviorType,
      triggeredEvent,
      behaviorData: behaviorData || {},
      path: route.path
    })
  } catch {}
}

const formatDate = (dateStr?: string) => (dateStr ? new Date(dateStr).toLocaleDateString('zh-CN') : '')

const submitQuickConsultation = async () => {
  quickError.value = ''
  quickSuccess.value = false

  if (!quickForm.value.customerName) return (quickError.value = '请填写称呼')
  if (!quickForm.value.contactValue) return (quickError.value = '请填写联系方式')
  if (!quickForm.value.requirementDescription) return (quickError.value = '请填写需求描述')

  const payload: {
    productId: number
    customerName: string
    customerPhone?: string
    customerWeChat?: string
    customerEmail?: string
    budgetRange?: string
    requirementDescription: string
  } = {
    productId: 0,
    customerName: quickForm.value.customerName,
    budgetRange: quickForm.value.budgetRange || undefined,
    requirementDescription: `[来源: 首页快速咨询]\n${quickForm.value.requirementDescription}`
  }

  if (quickForm.value.contactType === 'phone') payload.customerPhone = quickForm.value.contactValue
  if (quickForm.value.contactType === 'wechat') payload.customerWeChat = quickForm.value.contactValue
  if (quickForm.value.contactType === 'email') payload.customerEmail = quickForm.value.contactValue

  quickSubmitting.value = true
  try {
    await api.post('/Consultations', payload)
    await trackHomeEvent('home_quick_consult_submit', 'home_quick_consult_success', {
      contactType: quickForm.value.contactType
    })
    quickSuccess.value = true
    quickForm.value = { customerName: '', contactType: 'wechat', contactValue: '', budgetRange: '', requirementDescription: '' }
  } catch (e: any) {
    quickError.value = e?.response?.data?.message || e?.message || '提交失败，请稍后再试'
  } finally {
    quickSubmitting.value = false
  }
}
</script>

<style scoped>
.home-v4 {
  --bg-main: #07173b;
  --bg-card: #142a56;
  --line: rgb(148 163 184 / 28%);
  --line-strong: rgb(96 165 250 / 56%);
  --text-main: #f8fbff;
  --text-muted: #9db0d3;
  --header-offset: clamp(84px, 9vh, 102px);
  min-height: 100svh;
  background:
    radial-gradient(circle at 8% -6%, rgb(59 130 246 / 20%), transparent 42%),
    radial-gradient(circle at 92% 108%, rgb(96 165 250 / 15%), transparent 46%),
    #07173b;
  color: var(--text-main);
}

.shell {
  width: min(1220px, calc(100% - 3rem));
  margin: 0 auto;
}

.panel {
  min-height: clamp(520px, 78svh, 820px);
  padding: 3.5rem 0 1.5rem;
}

.hero-panel {
  min-height: calc(100svh - var(--header-offset));
  padding: calc(var(--header-offset) + 0.8rem) 0 1.6rem;
  display: flex;
  align-items: center;
}

.hero-shell {
  display: grid;
  grid-template-columns: 1.1fr 0.9fr;
  gap: 2rem;
  align-items: center;
}

.hero-pill {
  margin: 0;
  display: inline-flex;
  align-items: center;
  border-radius: 999px;
  border: 1px solid var(--line);
  padding: 0.35rem 0.8rem;
  font-size: 0.86rem;
  color: #bfe2ff;
}

h1 {
  margin: 1rem 0 0;
  font-size: clamp(3.2rem, 7vw, 6rem);
  line-height: 1.02;
  letter-spacing: -0.02em;
}

.hero-title {
  display: grid;
  gap: 0.1em;
}

.hero-title-prefix {
  display: block;
}

.hero-name {
  position: relative;
  display: inline-block;
  width: fit-content;
  letter-spacing: 0.02em;
  background: linear-gradient(92deg, #7dc4ff 0%, #8ea2ff 45%, #c7b4ff 100%);
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
  text-shadow: 0 0 26px rgb(125 196 255 / 34%);
  animation: namePulse 3.2s ease-in-out infinite;
}

.hero-name::after {
  content: '';
  position: absolute;
  left: -8%;
  top: 54%;
  width: 116%;
  height: 0.12em;
  background: linear-gradient(90deg, transparent, rgb(147 197 253 / 90%), transparent);
  transform: translateX(-120%);
  filter: blur(0.6px);
  opacity: 0.76;
  animation: nameScan 3.8s ease-in-out infinite;
}

.hero-role {
  margin: 1rem 0 0;
  font-size: 2rem;
  font-weight: 700;
  color: #7fb1ff;
}

.hero-subtitle {
  margin: 1rem 0 0;
  max-width: 62ch;
  color: #d2ddf2;
  line-height: 1.75;
}

.hero-tags {
  margin-top: 1rem;
  display: flex;
  flex-wrap: wrap;
  gap: 0.55rem;
}

.hero-tags span {
  border: 1px solid var(--line);
  border-radius: 999px;
  padding: 0.36rem 0.72rem;
  color: #dbeafe;
  font-size: 0.85rem;
}

.hero-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  margin-top: 1.15rem;
}

.btn {
  position: relative;
  overflow: hidden;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 2.7rem;
  padding: 0.65rem 1.05rem;
  border-radius: 0.82rem;
  text-decoration: none;
  font-weight: 700;
  border: 1px solid transparent;
}

.btn::after {
  content: '';
  position: absolute;
  inset: 0;
  transform: translateX(-120%);
  background: linear-gradient(110deg, transparent 25%, rgb(255 255 255 / 18%) 50%, transparent 75%);
  transition: transform 0.5s ease;
}

.btn:hover::after {
  transform: translateX(120%);
}

.btn-primary {
  background: linear-gradient(135deg, #2563eb, #60a5fa);
  color: #f8fbff;
}

.btn-ghost {
  border-color: var(--line);
  color: #e5e7eb;
  background: rgb(30 41 59 / 42%);
}

.hero-card {
  border-radius: 1.9rem;
  padding: 1px;
  background: linear-gradient(145deg, rgb(148 163 184 / 42%), rgb(59 130 246 / 18%));
  box-shadow: 0 18px 56px rgb(2 6 23 / 58%);
}

.hero-card-shell {
  position: relative;
  overflow: hidden;
  border-radius: 1.85rem;
  background: linear-gradient(165deg, rgb(20 33 63 / 96%), rgb(20 33 63 / 72%));
  border: 1px solid rgb(148 163 184 / 22%);
  padding: 1.4rem 1.2rem 1rem;
}

.hero-dot-grid {
  position: absolute;
  right: 1rem;
  bottom: 0.7rem;
  width: 140px;
  height: 108px;
  background-image: radial-gradient(circle, rgb(148 163 184 / 42%) 1.2px, transparent 1.2px);
  background-size: 12px 12px;
  opacity: 0.45;
}

.hero-card-kicker {
  margin: 0 0 0.8rem;
  margin-right: 4.8rem;
  height: 2.3rem;
  display: flex;
  align-items: center;
  padding: 0 0.92rem;
  border-radius: 999px;
  border: 1px solid rgb(45 212 191 / 38%);
  color: #d1fff4;
  letter-spacing: 0.14em;
  font-size: 0.72rem;
  font-weight: 700;
  background: rgb(16 185 129 / 8%);
}

.hero-card-kicker::before {
  content: '';
  width: 0.52rem;
  height: 0.52rem;
  border-radius: 50%;
  margin-right: 0.55rem;
  background: #34d399;
  box-shadow: 0 0 12px rgb(52 211 153 / 76%);
}

.hero-code-badge {
  position: absolute;
  right: 1.1rem;
  top: 4.72rem;
  width: 3.1rem;
  height: 3.1rem;
  border-radius: 0.9rem;
  display: grid;
  place-items: center;
  color: #dbeafe;
  font-weight: 800;
  border: 1px solid rgb(148 163 184 / 42%);
  background: linear-gradient(145deg, rgb(59 130 246 / 38%), rgb(30 41 59 / 62%));
}

.hero-avatar-wrap {
  position: absolute;
  left: 50%;
  top: 0.56rem;
  transform: translateX(-50%);
  z-index: 5;
}

.hero-avatar-glow {
  position: absolute;
  inset: -11px;
  border-radius: 1rem;
  background: linear-gradient(135deg, rgb(129 140 248 / 68%), rgb(34 211 238 / 58%));
  filter: blur(18px);
  opacity: 0.75;
  transition: opacity 0.38s ease, inset 0.38s ease;
}

.hero-avatar-frame {
  position: relative;
  width: 104px;
  height: 142px;
  border-radius: 1rem;
  overflow: hidden;
  border: 2px solid rgb(226 232 240 / 86%);
  box-shadow: 0 14px 32px rgb(15 23 42 / 58%);
  transition: width 0.38s ease, height 0.38s ease, transform 0.38s ease;
}

.hero-avatar-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: top;
  transition: transform 0.38s ease;
}

.hero-card:hover .hero-avatar-frame {
  width: 182px;
  height: 246px;
  transform: translateY(30px);
}

.hero-card:hover .hero-avatar-image {
  transform: scale(1.03);
}

.hero-card:hover .hero-avatar-glow {
  inset: -16px;
  opacity: 0.95;
}

.hero-card h3 {
  margin: 0.82rem 0 0;
  font-size: 2rem;
}

.hero-card p {
  margin: 0.38rem 0 0;
  color: #d2ddf2;
}

.hero-card-meta {
  margin-top: 0.65rem;
  display: flex;
  gap: 0.45rem;
  flex-wrap: wrap;
}

.hero-card-meta span {
  border: 1px solid var(--line);
  border-radius: 0.8rem;
  padding: 0.32rem 0.66rem;
  font-size: 0.84rem;
  background: rgb(59 130 246 / 14%);
}

.hero-cap-grid {
  margin-top: 0.65rem;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.56rem;
}

.hero-cap-item {
  border: 1px solid var(--line);
  border-radius: 0.82rem;
  background: rgb(15 23 42 / 34%);
  padding: 0.66rem;
}

.hero-cap-kicker {
  margin: 0;
  color: #93c5fd;
  font-size: 0.74rem;
  letter-spacing: 0.16em;
  text-transform: uppercase;
  font-weight: 700;
}

.hero-cap-text {
  margin: 0.34rem 0 0;
  color: #dbeafe;
  font-size: 0.84rem;
  line-height: 1.45;
}

.section-head {
  margin-bottom: 0.85rem;
}

.section-kicker {
  margin: 0;
  color: #9bc9ff;
  font-size: 0.76rem;
  letter-spacing: 0.17em;
  text-transform: uppercase;
}

h2 {
  margin: 0.45rem 0 0;
  font-size: clamp(1.5rem, 3vw, 2.2rem);
}

.bento-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  grid-auto-rows: minmax(168px, auto);
  gap: 0.85rem;
}

.bento-card {
  border: 1px solid var(--line);
  border-radius: 1rem;
  padding: 0.9rem;
  background: linear-gradient(165deg, rgb(20 33 63 / 94%), rgb(20 33 63 / 62%));
}

.bento-large {
  grid-column: span 2;
  grid-row: span 2;
}

.bento-wide {
  grid-column: span 2;
}

.card-kicker {
  margin: 0;
  color: #a8d1ff;
  font-size: 0.74rem;
  letter-spacing: 0.13em;
  text-transform: uppercase;
}

.bento-card h3 {
  margin: 0.45rem 0 0;
  font-size: 1.1rem;
}

.card-desc {
  margin-top: 0.45rem;
  color: var(--text-muted);
  line-height: 1.58;
}

.progress-list {
  margin: 0.75rem 0 0;
  padding: 0;
  list-style: none;
  display: grid;
  gap: 0.52rem;
}

.progress-meta {
  display: flex;
  justify-content: space-between;
  font-size: 0.84rem;
}

.progress-bar {
  height: 0.5rem;
  border-radius: 999px;
  background: rgb(255 255 255 / 9%);
  overflow: hidden;
}

.progress-bar span {
  display: block;
  height: 100%;
  border-radius: inherit;
  background: linear-gradient(90deg, #3b82f6, #60a5fa);
}

.stack-radar {
  margin-top: 0.72rem;
  display: flex;
  flex-wrap: wrap;
  gap: 0.52rem;
}

.stack-chip {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  border-radius: 999px;
  border: 1px solid var(--line);
  padding: 0.32rem 0.68rem;
  color: #dbeafe;
}

.stack-icon {
  color: #93c5fd;
}

.micro-map {
  margin-top: 0.68rem;
  height: 84px;
  border: 1px solid var(--line);
  border-radius: 0.75rem;
  position: relative;
  overflow: hidden;
}

.map-lines {
  position: absolute;
  inset: 0;
  background-image: linear-gradient(rgb(255 255 255 / 6%) 1px, transparent 1px);
  background-size: 100% 13px;
}

.map-pin {
  position: absolute;
  left: 52%;
  top: 44%;
  width: 0.7rem;
  height: 0.7rem;
  border-radius: 50%;
  background: #60a5fa;
  box-shadow: 0 0 0 6px rgb(96 165 250 / 22%);
}

.book-cover {
  margin-top: 0.68rem;
  height: 84px;
  border-radius: 0.75rem;
  border: 1px solid var(--line-strong);
  display: grid;
  place-items: center;
  color: #dbeafe;
  letter-spacing: 0.12em;
  font-size: 0.76rem;
  background: linear-gradient(145deg, rgb(96 165 250 / 16%), rgb(255 255 255 / 3%));
}

.delivery-list {
  display: grid;
  gap: 0.76rem;
}

.delivery-card {
  display: grid;
  grid-template-columns: 1.02fr 1.2fr;
  gap: 0.78rem;
  border: 1px solid var(--line);
  border-radius: 1rem;
  padding: 0.86rem;
  background: linear-gradient(165deg, rgb(20 33 63 / 94%), rgb(20 33 63 / 62%));
}

.demo-box {
  border: 1px solid var(--line);
  border-radius: 0.82rem;
  padding: 0.62rem;
  background:
    radial-gradient(circle at 88% 10%, rgb(56 189 248 / 14%), transparent 40%),
    linear-gradient(160deg, rgb(96 165 250 / 12%), rgb(255 255 255 / 2%));
}

.demo-window {
  border-radius: 0.72rem;
  border: 1px solid rgb(148 163 184 / 24%);
  background: linear-gradient(165deg, rgb(24 39 78 / 76%), rgb(15 23 42 / 52%));
  padding: 0.55rem;
}

.demo-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.45rem;
}

.demo-dots {
  display: inline-flex;
  gap: 0.3rem;
}

.demo-dots span {
  width: 0.42rem;
  height: 0.42rem;
  border-radius: 50%;
  background: rgb(148 163 184 / 58%);
}

.demo-dots span:first-child {
  background: #60a5fa;
}

.demo-label {
  border: 1px solid rgb(148 163 184 / 28%);
  border-radius: 999px;
  padding: 0.16rem 0.5rem;
  font-size: 0.7rem;
  color: #bfdbfe;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.demo-content {
  margin-top: 0.55rem;
  display: grid;
  grid-template-columns: 1.2fr 0.82fr;
  gap: 0.45rem;
}

.demo-main {
  border: 1px solid rgb(148 163 184 / 22%);
  border-radius: 0.66rem;
  background: rgb(15 23 42 / 36%);
  padding: 0.45rem;
}

.demo-main-top {
  height: 0.46rem;
  width: 68%;
  border-radius: 999px;
  background: linear-gradient(90deg, rgb(96 165 250 / 72%), rgb(255 255 255 / 20%));
}

.demo-chart {
  margin-top: 0.5rem;
  height: 46px;
  border: 1px solid rgb(148 163 184 / 16%);
  border-radius: 0.5rem;
  padding: 0.36rem;
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  align-items: end;
  gap: 0.3rem;
  background: linear-gradient(180deg, rgb(59 130 246 / 12%), rgb(15 23 42 / 20%));
}

.bar {
  display: block;
  border-radius: 0.25rem 0.25rem 0.1rem 0.1rem;
  background: linear-gradient(180deg, #93c5fd, #3b82f6);
}

.bar-1 {
  height: 48%;
}

.bar-2 {
  height: 72%;
}

.bar-3 {
  height: 62%;
}

.bar-4 {
  height: 88%;
}

.demo-side {
  display: grid;
  gap: 0.42rem;
}

.demo-side-card {
  border: 1px solid rgb(148 163 184 / 22%);
  border-radius: 0.66rem;
  background: rgb(15 23 42 / 30%);
  padding: 0.45rem 0.5rem;
}

.demo-side-card p {
  margin: 0;
  font-size: 0.72rem;
  color: #93c5fd;
}

.demo-side-card strong {
  margin-top: 0.22rem;
  display: block;
  font-size: 0.96rem;
  color: #eaf2ff;
}

.delivery-copy h3 {
  margin: 0;
  font-size: 1.18rem;
}

.delivery-tag {
  margin: 0.34rem 0 0;
  color: #a9cbff;
  font-size: 0.84rem;
}

.delivery-copy p {
  margin: 0.42rem 0 0;
  color: #d4ddf2;
  line-height: 1.58;
}

.delivery-metric {
  color: #93c5fd;
  font-weight: 700;
}

.thought-tracks {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 0.8rem;
}

.thought-card {
  border: 1px solid var(--line);
  border-radius: 1rem;
  padding: 0.9rem;
  background: linear-gradient(165deg, rgb(20 33 63 / 94%), rgb(20 33 63 / 62%));
}

.thought-card h3 {
  margin: 0;
  font-size: 1rem;
}

.thought-card p {
  margin: 0.48rem 0 0;
  color: var(--text-muted);
  line-height: 1.56;
}

.thought-link {
  margin-top: 0.5rem;
  border: 1px solid var(--line);
  border-radius: 0.72rem;
  padding: 0.5rem 0.62rem;
  display: flex;
  justify-content: space-between;
  gap: 0.55rem;
  text-decoration: none;
  color: #e5e7eb;
}

.thought-link:hover {
  border-color: var(--line-strong);
}

.thought-more {
  margin-top: 0.55rem;
  display: inline-flex;
  color: #93c5fd;
  text-decoration: none;
}

.cta-shell {
  display: grid;
  grid-template-columns: 1fr 1.1fr;
  gap: 0.9rem;
  align-items: start;
}

.cta-shell h2 {
  margin-top: 0.4rem;
}

.cta-shell p {
  color: #d1ddf2;
  line-height: 1.65;
}

.quick-form {
  border: 1px solid var(--line);
  border-radius: 1rem;
  padding: 0.9rem;
  background: linear-gradient(165deg, rgb(20 33 63 / 94%), rgb(20 33 63 / 62%));
}

.quick-form h3 {
  margin: 0;
  font-size: 1.02rem;
}

.quick-grid {
  margin-top: 0.62rem;
  display: grid;
  gap: 0.55rem;
  grid-template-columns: repeat(2, minmax(0, 1fr));
}

.quick-form input,
.quick-form select,
.quick-form textarea {
  width: 100%;
  border: 1px solid var(--line);
  border-radius: 0.66rem;
  background: rgb(255 255 255 / 3%);
  color: #eef2ff;
  padding: 0.56rem 0.65rem;
}

.quick-form textarea {
  margin-top: 0.56rem;
}

.quick-msg {
  margin: 0.52rem 0 0;
}

.quick-error {
  color: #fecaca;
}

.quick-success {
  color: #86efac;
}

@keyframes namePulse {
  0%,
  100% {
    text-shadow: 0 0 16px rgb(125 196 255 / 26%);
  }

  50% {
    text-shadow: 0 0 30px rgb(167 139 250 / 46%);
  }
}

@keyframes nameScan {
  0%,
  12% {
    transform: translateX(-120%);
    opacity: 0;
  }

  22%,
  58% {
    transform: translateX(0);
    opacity: 0.82;
  }

  70%,
  100% {
    transform: translateX(120%);
    opacity: 0;
  }
}

@media (max-width: 1100px) {
  .panel {
    min-height: auto;
    padding: 3.8rem 0 1.5rem;
  }

  .hero-shell,
  .cta-shell {
    grid-template-columns: 1fr;
  }

  .bento-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .bento-large,
  .bento-wide {
    grid-column: span 2;
    grid-row: auto;
  }

  .thought-tracks {
    grid-template-columns: 1fr 1fr;
  }

}

@media (max-width: 760px) {
  .shell {
    width: min(1220px, calc(100% - 1.6rem));
  }

  h1 {
    font-size: clamp(2.1rem, 10vw, 3.2rem);
  }

  .hero-role {
    font-size: 1.45rem;
  }

  .bento-grid,
  .thought-tracks,
  .quick-grid {
    grid-template-columns: 1fr;
  }

  .bento-large,
  .bento-wide {
    grid-column: span 1;
  }

  .delivery-card {
    grid-template-columns: 1fr;
  }

  .demo-content {
    grid-template-columns: 1fr;
  }

  .demo-side {
    grid-template-columns: 1fr 1fr;
  }

}
</style>
