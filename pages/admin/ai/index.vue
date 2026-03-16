<template>
  <ClientOnly>
    <div class="admin-ai-center-page">
      <!-- Hero 区域 -->
      <n-card class="hero-card mb-6">
        <div class="hero-content">
          <h1 class="hero-title">AI 智能体中�?/h1>
          <p class="hero-subtitle">
            统一管理内容创作、项目展示和客户线索处理的自动化智能工具，让个人网站像一家公司一样自动运转�?          </p>
          <p class="hero-description">
            这些智能体协助完成高质量文案生成、项目介绍自动化、线索评分与标签化，提升效率与专业度�?          </p>
        </div>
      </n-card>

      <!-- 智能体卡片（响应式网格，支持多行�?-->
      <div class="agents-grid">
        <!-- 内容生成智能�?-->
        <n-card class="agent-card" hoverable>
          <template #header>
            <div class="agent-header">
              <div class="agent-icon-wrapper agent-icon-blue">
                <span class="agent-icon-emoji">📄</span>
              </div>
              <div class="agent-header-text">
                <h3 class="agent-title">内容生成智能�?/h3>
                <p class="agent-subtitle">Content Agent</p>
              </div>
            </div>
          </template>
          <div class="agent-body">
            <p class="agent-description">
              自动生成文章草稿、项目介绍、工具说明等高质量内容�?            </p>
            <div class="agent-features">
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>文章草稿生成</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>项目/工具介绍生成</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>SEO 描述生成</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>多种语气可�?/span>
              </div>
            </div>
          </div>
          <template #action>
            <n-button type="primary" block @click="navigateTo('/admin/ai/content')">
              进入内容生成
            </n-button>
          </template>
        </n-card>

        <!-- Demo 上架智能�?-->
        <n-card class="agent-card" hoverable>
          <template #header>
            <div class="agent-header">
              <div class="agent-icon-wrapper agent-icon-purple">
                <span class="agent-icon-emoji">🧩</span>
              </div>
              <div class="agent-header-text">
                <h3 class="agent-title">Demo 上架智能�?/h3>
                <p class="agent-subtitle">Demo Agent</p>
              </div>
            </div>
          </template>
          <div class="agent-body">
            <p class="agent-description">
              自动为工�?项目生成展示级文案，包括卖点、场景、目标用户等�?            </p>
            <div class="agent-features">
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>项目卖点生成</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>自动生成展示文案</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>写回项目数据库字�?/span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>快速上�?Demo</span>
              </div>
            </div>
          </div>
          <template #action>
            <n-button type="primary" block @click="navigateTo('/admin/projects')">
              管理项目文案
            </n-button>
          </template>
        </n-card>

        <!-- 表单线索智能�?-->
        <n-card class="agent-card" hoverable>
          <template #header>
            <div class="agent-header">
              <div class="agent-icon-wrapper agent-icon-green">
                <span class="agent-icon-emoji">🎯</span>
              </div>
              <div class="agent-header-text">
                <h3 class="agent-title">表单线索智能�?/h3>
                <p class="agent-subtitle">Lead Agent</p>
              </div>
            </div>
          </template>
          <div class="agent-body">
            <p class="agent-description">
              自动分析客户需求，生成摘要、标签和评分�?            </p>
            <div class="agent-features">
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>自动摘要</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>标签提取</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>优先级评�?/span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>接洽建议</span>
              </div>
            </div>
          </div>
          <template #action>
            <n-button type="primary" block @click="navigateTo('/admin/consultations')">
              查看线索管理
            </n-button>
          </template>
        </n-card>

        <!-- 客服问答智能�?-->
        <n-card class="agent-card" hoverable>
          <template #header>
            <div class="agent-header">
              <div class="agent-icon-wrapper agent-icon-cyan">
                <span class="agent-icon-emoji">💬</span>
              </div>
              <div class="agent-header-text">
                <h3 class="agent-title">客服问答智能�?/h3>
                <p class="agent-subtitle">Support Agent</p>
              </div>
            </div>
          </template>
          <div class="agent-body">
            <p class="agent-description">
              为网站访客提供智能客服问答，自动回答服务内容、项目开发、工具使用等问题�?            </p>
            <div class="agent-features">
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>前台智能客服</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>FAQ 知识库支�?/span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>相关链接推荐</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>人工跟进建议</span>
              </div>
            </div>
          </div>
          <template #action>
            <n-button type="primary" block @click="navigateTo('/admin/ai/support-config')">
              配置客服
            </n-button>
          </template>
        </n-card>

        <!-- 个人助理智能�?-->
        <n-card class="agent-card" hoverable>
          <template #header>
            <div class="agent-header">
              <div class="agent-icon-wrapper agent-icon-orange">
                <span class="agent-icon-emoji">🤖</span>
              </div>
              <div class="agent-header-text">
                <h3 class="agent-title">个人助理智能�?/h3>
                <p class="agent-subtitle">Assistant Agent</p>
              </div>
            </div>
          </template>
          <div class="agent-body">
            <p class="agent-description">
              为管理员提供专属 AI 助理，帮助分析线索、规划学习、安排项目优先级等�?            </p>
            <div class="agent-features">
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>会话历史记录</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>上下文感�?/span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>数据智能分析</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>快捷操作建议</span>
              </div>
            </div>
          </div>
          <template #action>
            <n-button type="primary" block @click="navigateTo('/admin/ai/assistant')">
              打开个人助理
            </n-button>
          </template>
        </n-card>

        <!-- 自动报价智能�?-->
        <n-card class="agent-card" hoverable>
          <template #header>
            <div class="agent-header">
              <div class="agent-icon-wrapper agent-icon-pink">
                <span class="agent-icon-emoji">💰</span>
              </div>
              <div class="agent-header-text">
                <h3 class="agent-title">自动报价智能�?/h3>
                <p class="agent-subtitle">Quotation Agent</p>
              </div>
            </div>
          </template>
          <div class="agent-body">
            <p class="agent-description">
              根据客户需求和项目信息，自动生成专业的报价方案，包括明细、付款方式、交付时间等�?            </p>
            <div class="agent-features">
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>智能报价生成</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>报价明细列表</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>付款方式建议</span>
              </div>
              <div class="feature-item">
                <i class="fas fa-check-circle feature-icon"></i>
                <span>交付时间评估</span>
              </div>
            </div>
          </div>
          <template #action>
            <n-button type="primary" block @click="navigateTo('/admin/consultations')">
              在线索管理中查看
            </n-button>
          </template>
        </n-card>
      </div>

      <!-- 底部区域：工作流提示 + 模型状态面�?-->
      <div class="bottom-section">
        <!-- 工作流提示卡�?-->
        <n-card class="workflow-tip-card">
          <div class="workflow-tip-content">
            <i class="fas fa-lightbulb tip-icon"></i>
            <p class="tip-text">
              你可以随时扩展新的智能体，所有智能体共享统一�?AI 调用框架�?            </p>
          </div>
        </n-card>

        <!-- 模型状态面�?-->
        <n-card class="model-status-card">
          <template #header>
            <div class="status-header">
              <h3 class="status-title">模型状�?/h3>
              <n-button
                text
                size="small"
                @click="checkAiServiceStatus"
                :loading="statusLoading"
              >
                <template #icon>
                  <i class="fas fa-sync-alt"></i>
                </template>
                刷新
              </n-button>
            </div>
          </template>
          <div class="status-content">
            <div class="status-item">
              <span class="status-label">服务状�?/span>
              <n-tag :type="serviceStatus.type" size="small">
                <template #icon>
                  <i :class="serviceStatus.icon"></i>
                </template>
                {{ serviceStatus.text }}
              </n-tag>
            </div>
            <div class="status-item" v-if="modelInfo.model">
              <span class="status-label">当前模型</span>
              <span class="status-value">{{ modelInfo.model }}</span>
            </div>
            <div class="status-item" v-if="modelInfo.temperature !== null">
              <span class="status-label">温度参数</span>
              <span class="status-value">{{ modelInfo.temperature }}</span>
            </div>
            <div class="status-item" v-if="modelInfo.maxTokens">
              <span class="status-label">Token 限制</span>
              <span class="status-value">{{ modelInfo.maxTokens }}</span>
            </div>
            <div class="status-item">
              <span class="status-label">日志状�?/span>
              <n-tag type="success" size="small">已启�?/n-tag>
            </div>
            <div class="status-actions">
              <n-button
                secondary
                size="small"
                @click="navigateTo('/admin/ai/logs')"
              >
                <template #icon>
                  <i class="fas fa-list"></i>
                </template>
                查看日志
              </n-button>
            </div>
          </div>
        </n-card>
      </div>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import { NCard, NButton, NTag } from 'naive-ui'
import { useSafeMessage } from '~/composables/useNaiveUI'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth',
  ssr: false
})

const api = useApi()
const message = useSafeMessage()

// 模型状态信�?const statusLoading = ref(false)
const serviceStatus = ref({
  type: 'default' as 'success' | 'warning' | 'error' | 'info' | 'default',
  text: '检查中...',
  icon: 'fas fa-circle-notch fa-spin'
})

const modelInfo = ref({
  model: '',
  temperature: null as number | null,
  maxTokens: null as number | null
})

// 检�?AI 服务状�?const checkAiServiceStatus = async () => {
  statusLoading.value = true
  try {
    // 调用后端健康检查接�?    const res = await api.get('/ai/health').catch(() => null)
    
    if (res && res.success) {
      serviceStatus.value = {
        type: 'success',
        text: '运行�?,
        icon: 'fas fa-check-circle'
      }
      
      // 设置默认模型信息（可以从配置或响应中获取�?      modelInfo.value = {
        model: 'DeepSeek',
        temperature: 0.7,
        maxTokens: 2000
      }
    } else {
      // 如果后端接口不可用，尝试直接调用 Python AI Service
      try {
        const healthRes = await $fetch('http://localhost:8001/api/ai/health', {
          method: 'GET',
          timeout: 3000
        }).catch(() => null)
        
        if (healthRes && healthRes.success) {
          serviceStatus.value = {
            type: 'success',
            text: '运行�?,
            icon: 'fas fa-check-circle'
          }
          modelInfo.value = {
            model: 'DeepSeek',
            temperature: 0.7,
            maxTokens: 2000
          }
        } else {
          throw new Error('服务不可�?)
        }
      } catch {
        serviceStatus.value = {
          type: 'warning',
          text: '未连�?,
          icon: 'fas fa-exclamation-triangle'
        }
        modelInfo.value = {
          model: '',
          temperature: null,
          maxTokens: null
        }
      }
    }
  } catch (e: any) {
    serviceStatus.value = {
      type: 'error',
      text: '连接失败',
      icon: 'fas fa-times-circle'
    }
    modelInfo.value = {
      model: '',
      temperature: null,
      maxTokens: null
    }
    console.error('检�?AI 服务状态失�?', e)
  } finally {
    statusLoading.value = false
  }
}

// 页面加载时检查状�?onMounted(() => {
  checkAiServiceStatus()
})

useHead({
  title: 'AI 智能体中�?- 后台管理',
  meta: [
    { name: 'description', content: 'AI 智能体中心，提供内容生成、Demo 上架、线索处理等 AI 能力' }
  ]
})
</script>

<style scoped>
.admin-ai-center-page {
  max-width: 1400px;
  margin: 0 auto;
  padding: var(--spacing-6);
}

/* Hero 区域样式 - 使用 CSS 变量 */
.hero-card {
  background: linear-gradient(135deg, var(--color-primary, var(--color-indigo-500)) 0%, var(--color-purple, var(--color-purple-600)) 100%);
  border: none;
  box-shadow: var(--shadow-md);
}

.hero-content {
  color: var(--color-text-main, var(--color-bg-light, white));
  text-align: center;
  padding: var(--spacing-20) 0;
}

.hero-title {
  font-size: var(--text-3xl);
  font-weight: 700;
  margin-bottom: var(--spacing-4);
  text-shadow: var(--shadow-text, 0 var(--spacing-0_5) var(--spacing-3) rgba(0, 0, 0, 0.2));
}

.hero-subtitle {
  font-size: var(--text-xl);
  font-weight: 500;
  margin-bottom: var(--spacing-3);
  opacity: 0.95;
  line-height: 1.6;
}

.hero-description {
  font-size: var(--text-base);
  opacity: 0.85;
  line-height: 1.6;
  max-width: 800px;
  margin: 0 auto;
}

/* 智能体卡片网�?*/
.agents-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
  gap: var(--spacing-6);
  margin-bottom: var(--spacing-6);
}

/* 智能体卡片样�?*/
.agent-card {
  transition: all 0.3s ease;
  border: 1px solid var(--n-border-color);
}

.agent-card:hover {
  transform: translateY(calc(var(--spacing-0_5) * -1));
  box-shadow: var(--shadow-lg);
}

.agent-header {
  display: flex;
  align-items: center;
  gap: var(--spacing-4);
}

.agent-icon-wrapper {
  width: var(--spacing-14);
  height: var(--spacing-14);
  border-radius: var(--radius-lg);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

/* 智能体图标样�?- 使用 CSS 变量 */
.agent-icon-blue {
  background: linear-gradient(135deg, var(--color-primary, var(--color-primary)) 0%, var(--color-primary-hover, var(--color-primary-hover)) 100%);
}

.agent-icon-purple {
  background: linear-gradient(135deg, var(--color-purple, var(--color-purple-500)) 0%, var(--color-purple-hover, var(--color-purple-600)) 100%);
}

.agent-icon-green {
  background: linear-gradient(135deg, var(--color-success, var(--color-success)) 0%, var(--color-success-hover, var(--color-green-700)) 100%);
}

.agent-icon-cyan {
  background: linear-gradient(135deg, var(--color-info, var(--color-cyan-500)) 0%, var(--color-info-hover, var(--color-cyan-600)) 100%);
}

.agent-icon-orange {
  background: linear-gradient(135deg, var(--color-warning, var(--color-orange-500)) 0%, var(--color-warning-hover, var(--color-orange-600)) 100%);
}

.agent-icon-pink {
  background: linear-gradient(135deg, var(--color-error, var(--color-pink-500)) 0%, var(--color-error-hover, var(--color-pink-600)) 100%);
}

.agent-icon-emoji {
  font-size: var(--text-2xl);
  line-height: 1;
}

.agent-header-text {
  flex: 1;
}

.agent-title {
  font-size: var(--text-xl);
  font-weight: 600;
  margin: 0 0 var(--spacing-1) 0;
  color: var(--n-text-color);
}

.agent-subtitle {
  font-size: var(--text-sm);
  color: var(--n-text-color-2);
  margin: 0;
}

.agent-body {
  min-height: var(--spacing-50);
  padding: var(--spacing-2) 0;
}

.agent-description {
  font-size: var(--text-sm);
  color: var(--n-text-color-2);
  line-height: 1.6;
  margin-bottom: var(--spacing-20);
}

.agent-features {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-3);
}

.feature-item {
  display: flex;
  align-items: center;
  gap: var(--spacing-2_5);
  font-size: var(--text-sm);
  color: var(--n-text-color-1);
}

.feature-icon {
  color: var(--color-success, var(--color-success));
  font-size: var(--text-sm);
  flex-shrink: 0;
}

/* 底部区域 */
.bottom-section {
  display: grid;
  grid-template-columns: 1fr 400px;
  gap: var(--spacing-6);
  margin-top: var(--spacing-6);
}

@media (max-width: 1024px) {
  .bottom-section {
    grid-template-columns: 1fr;
  }
}

/* 工作流提示卡�?- 使用 CSS 变量 */
.workflow-tip-card {
  background: linear-gradient(135deg, var(--color-warning-soft, var(--color-amber-100)) 0%, var(--color-warning-soft, var(--color-amber-200)) 100%);
  border: 1px solid var(--color-warning, var(--color-amber-400));
}

.workflow-tip-content {
  display: flex;
  align-items: center;
  gap: var(--spacing-3);
  padding: var(--spacing-2) 0;
}

.tip-icon {
  font-size: var(--text-xl);
  color: var(--color-warning-hover, var(--color-amber-600));
  flex-shrink: 0;
}

.tip-text {
  font-size: var(--text-sm);
  color: var(--color-text-main, var(--color-amber-800));
  margin: 0;
  line-height: 1.6;
}

/* 模型状态面�?*/
.model-status-card {
  border: 1px solid var(--n-border-color);
}

.status-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.status-title {
  font-size: var(--text-lg);
  font-weight: 600;
  margin: 0;
  color: var(--n-text-color);
}

.status-content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-4);
}

.status-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-2) 0;
  border-bottom: 1px solid var(--n-border-color);
}

.status-item:last-of-type {
  border-bottom: none;
}

.status-label {
  font-size: var(--text-sm);
  color: var(--n-text-color-2);
  font-weight: 500;
}

.status-value {
  font-size: var(--text-sm);
  color: var(--n-text-color);
  font-weight: 600;
}

.status-actions {
  margin-top: var(--spacing-2);
  padding-top: var(--spacing-4);
  border-top: 1px solid var(--n-border-color);
}

/* 响应式设�?*/
@media (max-width: 768px) {
  .admin-ai-center-page {
    padding: var(--spacing-4);
  }

  .hero-title {
    font-size: var(--text-2xl);
  }

  .hero-subtitle {
    font-size: var(--text-lg);
  }

  .agents-grid {
    grid-template-columns: 1fr;
    gap: var(--spacing-4);
  }

  .bottom-section {
    grid-template-columns: 1fr;
    gap: var(--spacing-4);
  }
}
</style>
