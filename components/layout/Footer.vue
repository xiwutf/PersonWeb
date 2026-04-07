<template>
  <footer class="footer-container">
    <!-- 动态背景装饰 -->
    <div class="footer-background-decorations">
      <div class="footer-bg-decoration footer-bg-decoration-purple"></div>
      <div class="footer-bg-decoration footer-bg-decoration-blue"></div>
      <div class="footer-grid-bg"></div>
      <div class="footer-top-gradient-line"></div>
    </div>

    <div class="footer-content-wrapper">
      <div class="footer-main-grid">
        <!-- 品牌区域 (占5列) -->
        <div class="footer-brand-section">
          <div class="footer-brand-eyebrow">Independent Builder · Digital Craft</div>
          <div class="footer-brand-logo-container">
            <div class="footer-brand-logo-icon">
              <span class="footer-brand-logo-icon-text">XF</span>
            </div>
            <span class="footer-brand-name">
              溪午听风
            </span>
          </div>
          <p class="footer-brand-description">
            这里不是简单的个人主页，而是一套围绕产品、工程、内容与工具持续生长的数字作品集。希望每一个页面都能同时传达审美、判断力和落地能力。
          </p>
          <div class="footer-brand-highlights">
            <span
              v-for="highlight in brandHighlights"
              :key="highlight"
              class="footer-brand-highlight"
            >
              {{ highlight }}
            </span>
          </div>
          <div class="footer-social-links">
            <a 
              v-for="social in socialLinks" 
              :key="social.name" 
              :href="social.link" 
              :target="social.name === 'WeChat' || social.name === 'Email' ? undefined : '_blank'"
              @click.prevent="handleSocialClick(social.name)"
              class="footer-social-link"
            >
              <i :class="social.icon"></i>
            </a>
          </div>
          
          <!-- 微信二维码弹窗 -->
          <div v-if="showWeChatQR" class="wechat-qr-modal" @click="showWeChatQR = false">
            <div class="wechat-qr-content" @click.stop>
              <button class="wechat-qr-close" @click="showWeChatQR = false">
                <i class="fas fa-times"></i>
              </button>
              <div class="wechat-qr-image-wrapper">
                <img src="/images/wechat-qr.png" alt="微信二维码" class="wechat-qr-image" />
              </div>
              <p class="wechat-qr-text">扫码加好友，请注明来意</p>
            </div>
          </div>
          
          <!-- 邮箱弹窗 -->
          <div v-if="showEmailModal" class="email-modal" @click="showEmailModal = false">
            <div class="email-modal-content" @click.stop>
              <button class="email-modal-close" @click="showEmailModal = false">
                <i class="fas fa-times"></i>
              </button>
              <div class="email-modal-header">
                <i class="fas fa-envelope email-modal-icon"></i>
                <h3 class="email-modal-title">联系邮箱</h3>
              </div>
              <div class="email-modal-body">
                <div class="email-address-display">
                  <span class="email-address-text">linxiwanting@gmail.com</span>
                  <button 
                    class="email-copy-btn" 
                    @click="copyEmail"
                    title="复制邮箱地址"
                  >
                    <i class="fas fa-copy"></i>
                  </button>
                </div>
                <a 
                  href="mailto:linxiwanting@gmail.com" 
                  class="email-send-btn"
                >
                  <i class="fas fa-paper-plane"></i>
                  直接发送邮件
                </a>
              </div>
            </div>
          </div>
        </div>

        <!-- 链接区域 (占7列) -->
        <div class="footer-links-section">
          <div
            v-for="group in footerLinkGroups"
            :key="group.title"
            class="footer-link-group"
          >
            <h3 class="footer-link-group-title">{{ group.title }}</h3>
            <ul class="footer-link-list">
              <li v-for="item in group.items" :key="item.path">
                <NuxtLink :to="item.path" class="footer-link-item">
                  <i class="fas fa-chevron-right footer-link-icon"></i>
                  {{ item.title }}
                </NuxtLink>
              </li>
            </ul>
          </div>

          <div class="footer-cta-panel">
            <div>
              <div class="footer-cta-label">Site Direction</div>
              <h3 class="footer-cta-title">从展示站升级为有结构、有判断力的数字名片</h3>
              <p class="footer-cta-description">
                项目、博客、工具、生活内容都在同一套叙事里协同出现，既能看见审美，也能看见方法与执行。
              </p>
            </div>
            <div class="footer-cta-actions">
              <NuxtLink to="/projects" class="footer-cta-button footer-cta-button-primary">
                查看项目
              </NuxtLink>
              <NuxtLink to="/about" class="footer-cta-button footer-cta-button-secondary">
                了解我
              </NuxtLink>
            </div>
          </div>
        </div>
      </div>

      <!-- 底部版权与统计 -->
      <div class="footer-bottom">
        <div class="footer-copyright">
          © {{ currentYear }} 溪午听风. All rights reserved.
        </div>
        
        <div class="footer-bottom-right">
          <div v-if="stats" class="footer-stats-container">
            <div class="footer-stat-item">
              <span class="footer-stat-indicator"></span>
              <span>今日: <span class="footer-stat-number">{{ stats.todayVisits }}</span></span>
            </div>
            <div class="footer-stat-divider"></div>
            <div class="footer-stat-item">
              <span>总计: <span class="footer-stat-number">{{ stats.totalVisits }}</span></span>
            </div>
          </div>
          
          <div class="footer-made-with">
            <span>Made with</span>
            <i class="fas fa-heart footer-heart-icon"></i>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const currentYear = new Date().getFullYear()

const primaryLinks = [
  { title: '插件工具', path: '/tools' },
  { title: '项目展示', path: '/projects' },
  { title: '技术博客', path: '/blog' },
  { title: '关于我', path: '/about' }
]

const contentLinks = [
  { title: '生活随笔', path: '/life' },
  { title: '订单查询', path: '/order/query' },
  { title: '全站搜索', path: '/search' },
  { title: '联系入口', path: '/contact' }
]

const footerLinkGroups = [
  { title: 'Explore', items: primaryLinks },
  { title: 'Content', items: contentLinks }
]

const brandHighlights = [
  'AI 工作流',
  '全栈产品实现',
  '系统化内容呈现'
]

const showWeChatQR = ref(false)
const showEmailModal = ref(false)

const socialLinks = [
  { name: 'GitHub', icon: 'fab fa-github', link: 'https://github.com/Lijing327' },
  { name: 'WeChat', icon: 'fab fa-weixin', link: '#' }, // 点击显示二维码
  { name: 'Email', icon: 'fas fa-envelope', link: 'mailto:linxiwanting@gmail.com' }
]

// 处理社交链接点击
const handleSocialClick = (name: string) => {
  if (name === 'WeChat') {
    showWeChatQR.value = true
  } else if (name === 'Email') {
    showEmailModal.value = true
  }
}

// 复制邮箱地址
const copyEmail = async () => {
  const email = 'linxiwanting@gmail.com'
  try {
    await navigator.clipboard.writeText(email)
    // 可以添加提示消息
    alert('邮箱地址已复制到剪贴板')
  } catch (err) {
    console.error('复制失败:', err)
    // 降级方案：使用传统方法
    const textArea = document.createElement('textarea')
    textArea.value = email
    document.body.appendChild(textArea)
    textArea.select()
    document.execCommand('copy')
    document.body.removeChild(textArea)
    alert('邮箱地址已复制到剪贴板')
  }
}

// 获取统计数据（从后端API获取实时数据）
const stats = ref<{ todayVisits: number; totalVisits: number } | null>(null)

// 获取统计数据
const fetchStats = async () => {
  try {
    const api = useApi()
    
    // 获取或创建 Visitor ID
    let visitorId = localStorage.getItem('visitor_id')
    if (!visitorId) {
      visitorId = crypto.randomUUID()
      localStorage.setItem('visitor_id', visitorId)
    }
    
    // 调用后端 API：/api/Tracking/visit
    // 这个接口既记录访问又返回统计数据
    const data = await api.post<{
      totalVisits: number
      todayVisits: number
      visitorId: string
    }>('/Tracking/visit', {
      visitorId,
      path: window.location.pathname
    })
    
    // 后端返回的字段名是 TotalVisits 和 TodayVisits（大写开头）
    // 但 TypeScript 类型定义是小写，需要兼容两种格式
    const totalVisits = Number(data?.totalVisits || (data as any)?.TotalVisits || 0)
    const todayVisits = Number(data?.todayVisits || (data as any)?.TodayVisits || 0)
    
    stats.value = {
      todayVisits,
      totalVisits
    }
    
    // 如果返回了新的 visitorId，更新本地存储
    if (data?.visitorId && data.visitorId !== visitorId) {
      localStorage.setItem('visitor_id', data.visitorId)
    }
  } catch (err: any) {
    // 错误时使用默认值，不阻塞页面显示
    stats.value = { todayVisits: 0, totalVisits: 0 }
  }
}

onMounted(() => {
  if (process.client) {
    // 直接调用后端接口，既记录访问又获取统计数据
    fetchStats()
  }
})
</script>

<style scoped>
.animation-delay-2000 {
  animation-delay: 2s;
}

/* 微信二维码弹窗 */
.wechat-qr-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: var(--color-overlay, rgba(0, 0, 0, 0.7));
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
  box-shadow: var(--shadow-xl, 0 20px 60px rgba(0, 0, 0, 0.5));
  animation: slideUp 0.3s ease-out;
  max-width: 90vw;
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

.wechat-qr-image-wrapper {
  display: flex;
  justify-content: center;
  margin-bottom: 1rem;
}

.wechat-qr-image {
  width: auto;
  height: auto;
  max-width: 300px;
  max-height: 300px;
  border-radius: 0.5rem;
  border: 2px solid var(--color-border-subtle, rgba(255, 255, 255, 0.1));
  object-fit: contain;
}

.wechat-qr-text {
  text-align: center;
  color: var(--color-text-muted, var(--color-text-muted));
  font-size: 0.875rem;
  margin: 0;
}

/* 邮箱弹窗 */
.email-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: var(--color-overlay, rgba(0, 0, 0, 0.7));
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  animation: fadeIn 0.2s ease-out;
}

.email-modal-content {
  position: relative;
  background: var(--color-bg-card, var(--color-border-default));
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: var(--shadow-xl, 0 20px 60px rgba(0, 0, 0, 0.5));
  animation: slideUp 0.3s ease-out;
  min-width: 320px;
  max-width: 90vw;
}

.email-modal-close {
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

.email-modal-close:hover {
  color: var(--color-text-main, var(--color-bg-card));
}

.email-modal-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.email-modal-icon {
  font-size: 1.5rem;
  color: var(--color-primary, var(--color-primary));
}

.email-modal-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--color-text-main, var(--color-bg-card));
  margin: 0;
}

.email-modal-body {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.email-address-display {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: var(--color-bg-elevated, #334155);
  border: 1px solid var(--color-border-subtle, rgba(255, 255, 255, 0.1));
  border-radius: 0.5rem;
  padding: 1rem;
  gap: 1rem;
}

.email-address-text {
  font-size: 1rem;
  color: var(--color-text-main, var(--color-bg-card));
  font-family: 'Courier New', monospace;
  flex: 1;
  word-break: break-all;
}

.email-copy-btn {
  background: var(--color-primary, var(--color-primary));
  border: none;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.email-copy-btn:hover {
  background: var(--color-primary-hover, var(--color-primary-hover));
  transform: translateY(-1px);
}

.email-send-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  background: linear-gradient(135deg, var(--color-primary, var(--color-primary)), var(--color-primary-hover, var(--color-primary-hover)));
  color: white;
  text-decoration: none;
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  font-weight: 500;
  transition: all 0.2s ease;
}

.email-send-btn:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-primary, 0 4px 12px rgba(59, 130, 246, 0.4));
}
</style>
