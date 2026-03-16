<template>
  <div class="tools-page">
    <!-- ?????? -->
    <div class="tools-background-noise"></div>

    <!-- ???????-->
    <div class="tools-background-container">
      <div class="tools-background-blob tools-background-blob--orange"></div>
      <div class="tools-background-blob tools-background-blob--red"></div>
      <div class="tools-background-blob tools-background-blob--amber"></div>
    </div>

    <div class="tools-content">
      <!-- ???? -->
      <header class="tools-header">
        <div class="tools-header-icon">
          <span>??</span>
        </div>
        <h1 class="tools-title">????</h1>
        <p class="tools-subtitle">
          ???Revit?????????????????????????????        </p>
      </header>

      <!-- ???? -->
      <div class="tools-stats-grid">
        <div class="tools-stat-card">
          <div class="tools-stat-value tools-stat-value--orange">
            {{ tools.length || 0 }}+
          </div>
          <div class="tools-stat-label">????</div>
        </div>
        <div class="tools-stat-card">
          <div class="tools-stat-value tools-stat-value--red">
            1000+
          </div>
          <div class="tools-stat-label">????</div>
        </div>
        <div class="tools-stat-card">
          <div class="tools-stat-value tools-stat-value--green">
            99%
          </div>
          <div class="tools-stat-label">???</div>
        </div>
      </div>

      <!-- ?????-->
      <div class="tools-navigation">
        <NuxtLink
          to="/tools/marketplace"
          class="tools-nav-button tools-nav-button--primary"
        >
          ?? ????
        </NuxtLink>
        <NuxtLink
          to="/tools/collections"
          class="tools-nav-button tools-nav-button--secondary"
        >
          ?? ????
        </NuxtLink>
        <NuxtLink
          to="/tools/my-tools"
          class="tools-nav-button tools-nav-button--secondary"
        >
          ?? ????
        </NuxtLink>
        <NuxtLink
          to="/order/query"
          class="tools-nav-button tools-nav-button--secondary"
        >
          ?? ????
        </NuxtLink>
      </div>

      <!-- ???? -->
      <div v-if="pending" class="tools-loading">
        <div class="tools-loading-spinner"></div>
        <p class="tools-loading-text">??????...</p>
      </div>

      <div v-else-if="error" class="tools-error">
        <div class="tools-error-icon">??</div>
        <h3 class="tools-error-title">????</h3>
        <p class="tools-error-message">{{ error }}</p>
      </div>

      <div v-else class="tools-grid">
        <TransitionGroup name="tools-list">
          <div
            v-for="(tool, index) in tools"
            :key="tool._path"
            class="tools-card"
            :style="{ transitionDelay: `${index * 50}ms` }"
          >
            <!-- ???? -->
            <div class="tools-card-header">
              <div class="tools-card-top">
                <div class="tools-card-icon">
                  ??
                </div>
                <div class="tools-card-price">
                  <div class="tools-card-price-current">?{{ tool.price }}</div>
                  <div class="tools-card-price-original">?{{ Math.round(tool.price * 1.5) }}</div>
                </div>
              </div>

              <h3 class="tools-card-title">
                {{ tool.title }}
              </h3>
              
              <p class="tools-card-description">
                {{ tool.description }}
              </p>
              
              <!-- ?? -->
              <div class="tools-card-tags">
                <span
                  v-for="tag in tool.tags"
                  :key="tag"
                  class="tools-card-tag"
                >
                  {{ tag }}
                </span>
              </div>
            </div>

            <!-- ???? -->
            <div class="tools-card-footer">
              <div class="tools-card-actions">
                <NuxtLink
                  :to="`/tools/detail-${tool.slug || tool._path?.split('/').pop() || ''}`"
                  class="tools-card-button tools-card-button--secondary"
                >
                  <svg class="tools-card-button-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path>
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"></path>
                  </svg>
                  <span>????</span>
                </NuxtLink>
                <button
                  v-if="tool.id"
                  @click="handleOrder(tool)"
                  class="tools-card-button tools-card-button--primary tools-card-button--order"
                >
                  <svg class="tools-card-button-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path>
                  </svg>
                  <span>????</span>
                </button>
                <button
                  v-else
                  @click="handleConsultation(tool)"
                  class="tools-card-button tools-card-button--primary tools-card-button--consult"
                >
                  <svg class="tools-card-button-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"></path>
                  </svg>
                  <span>??</span>
                </button>
              </div>
            </div>
          </div>
        </TransitionGroup>
      </div>

      <!-- ??CTA -->
      <div class="tools-cta">
        <div class="tools-cta-container">
          <div class="tools-cta-overlay"></div>
          
          <div class="tools-cta-content">
            <h3 class="tools-cta-title">????????</h3>
            <p class="tools-cta-description">???????????????????????Revit?AutoCAD?Office???????????????????????????????????????????????</p>
            <button
              @click="showWeChatQR = true"
              class="tools-cta-button"
            >
              <span>??</span>
              ????
            </button>
          </div>
          
          <!-- ????????-->
          <div v-if="showWeChatQR" class="wechat-qr-modal" @click="showWeChatQR = false">
            <div class="wechat-qr-content" @click.stop>
              <button class="wechat-qr-close" @click="showWeChatQR = false">
                <i class="fas fa-times"></i>
              </button>
              <div class="wechat-qr-header">
                <h3 class="wechat-qr-title">??????</h3>
                <p class="wechat-qr-subtitle">?????????????</p>
              </div>
              <div class="wechat-qr-image-wrapper">
                <img src="/images/wechat-qr.png" alt="?????" class="wechat-qr-image" />
              </div>
              <div class="wechat-qr-info">
                <p class="wechat-qr-text">????LinXi-5152</p>
                <p class="wechat-qr-hint">???????????</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ???? -->
    <ConsultationDialog
      v-if="selectedTool"
      v-model:visible="showConsultationDialog"
      :product-id="selectedTool.id || 0"
      :product-name="selectedTool.title || selectedTool.name || ''"
      @success="handleConsultationSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSafeMessage } from '~/composables/useNaiveUI'

// ?? default ????? Header?
definePageMeta({
  layout: 'default'
})

const api = useApi()
const router = useRouter()
const message = useSafeMessage()
const tools = ref<any[]>([])
const pending = ref(true)
const error = ref<string | null>(null)
const showWeChatQR = ref(false)
const showConsultationDialog = ref(false)
const selectedTool = ref<any>(null)

// ?API??????
const fetchTools = async () => {
  try {
    pending.value = true
    error.value = null
    
    // ????Toolbox API ??
    try {
      const res = await api.get<any>('/Toolbox/marketplace', {
        params: { page: 1, pageSize: 100 }
      })
      
      // useApi ?????????????? data ??
      // ?????????
      let toolsList: any[] = []
      if (res && res.tools && Array.isArray(res.tools)) {
        toolsList = res.tools
      } else if (res && res.data && res.data.tools && Array.isArray(res.data.tools)) {
        toolsList = res.data.tools
      } else if (Array.isArray(res)) {
        toolsList = res
      }
      
      if (toolsList.length > 0) {
        // ??????????
        tools.value = toolsList.map((t: any) => ({
          id: t.id,
          title: t.name || t.title,
          name: t.name || t.title,
          slug: t.slug,
          description: t.description || '',
          price: t.price || 0,
          tags: t.tags || t.tagsJson || [],
          _path: `/tools/${t.slug}`,
          enableOnlineOrder: t.enableOnlineOrder || false
        }))
        return
      }
    } catch (e) {
      console.warn('??Toolbox API ??????????????', e)
    }
    
    // ? API ?????? @nuxt/content ? Content v3 queryCollection
      const { data: contentTools } = await useAsyncData('tools', () =>
      queryCollection('content').where('path', 'LIKE', '/tools%').order('date', 'DESC').all()
    )
    
    if (contentTools.value && Array.isArray(contentTools.value) && contentTools.value.length > 0) {
      tools.value = contentTools.value
    } else {
      tools.value = []
    }
  } catch (e: any) {
    console.error('Failed to fetch tools:', e)
    error.value = e.message || '????'
    tools.value = []
  } finally {
    pending.value = false
  }
}

// ????
const handleOrder = (tool: any) => {
  if (tool.id) {
    router.push(`/order/create?productId=${tool.id}`)
  } else {
    message.error('?????????')
  }
}

// ????
const handleConsultation = (tool: any) => {
  // ????ID????????????????
  if (tool.id) {
    selectedTool.value = tool
    showConsultationDialog.value = true
  } else {
    const slug = tool.slug || tool.path?.split('/').pop()
    if (slug) {
      router.push(`/tools/detail-${slug}`)
    } else {
      message.error('?????????')
    }
  }
}

// ??????
const handleConsultationSuccess = () => {
  console.log('??????')
  showConsultationDialog.value = false
  selectedTool.value = null
  message.success('????????????????')
}

onMounted(() => {
  fetchTools()
})

useHead({
  title: '???? - ????',
  meta: [
    { name: 'description', content: '???? - Revit?AutoCAD?Office ?????' }
  ]
})
</script>

<style scoped>
/* ??????????assets/css/tools.css */
/* ??????????????????*/

/* ????????*/
.wechat-qr-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: var(--overlay-color, rgba(0, 0, 0, 0.7));
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  animation: fadeIn 0.2s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.wechat-qr-content {
  position: relative;
  background: var(--color-bg-card, var(--color-border-default));
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: 0 20px 60px var(--overlay-color, rgba(0, 0, 0, 0.5));
  animation: slideUp 0.3s ease-out;
  max-width: 90vw;
  width: 100%;
  max-width: 400px;
}

@keyframes slideUp {
  from {
    transform: translateY(20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.wechat-qr-close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: transparent;
  border: none;
  color: var(--color-text-muted, var(--color-text-muted));
  font-size: 1.5rem;
  cursor: pointer;
  padding: 0.5rem;
  line-height: 1;
  transition: color 0.2s ease;
  z-index: 1;
}

.wechat-qr-close:hover {
  color: var(--color-text-main, var(--color-bg-card));
}

.wechat-qr-header {
  text-align: center;
  margin-bottom: 1.5rem;
}

.wechat-qr-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--color-text-main, var(--color-bg-card));
  margin: 0 0 0.5rem 0;
}

.wechat-qr-subtitle {
  font-size: 0.875rem;
  color: var(--color-text-muted, var(--color-text-muted));
  margin: 0;
}

.wechat-qr-image-wrapper {
  display: flex;
  justify-content: center;
  margin-bottom: 1.5rem;
}

.wechat-qr-image {
  width: auto;
  height: auto;
  max-width: 250px;
  max-height: 250px;
  border-radius: 0.5rem;
  border: 2px solid var(--color-border-subtle, rgba(255, 255, 255, 0.1));
  object-fit: contain;
}

.wechat-qr-info {
  text-align: center;
}

.wechat-qr-text {
  font-size: 1rem;
  color: var(--color-text-main, var(--color-bg-card));
  font-weight: 500;
  margin: 0 0 0.5rem 0;
}

.wechat-qr-hint {
  font-size: 0.875rem;
  color: var(--color-text-muted, var(--color-text-muted));
  margin: 0;
}
</style>
