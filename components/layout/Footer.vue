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
          <div class="footer-brand-logo-container">
            <div class="footer-brand-logo-icon">
              <span class="footer-brand-logo-icon-text">XF</span>
            </div>
            <span class="footer-brand-name">
              溪午听风
            </span>
          </div>
          <p class="footer-brand-description">
            用代码构建未来，用技术解决难题。专注于全栈开发与AI应用探索，致力于创造优雅的数字体验。
          </p>
          <div class="footer-social-links">
            <a v-for="social in socialLinks" :key="social.name" :href="social.link" target="_blank" class="footer-social-link">
              <i :class="social.icon"></i>
            </a>
          </div>
        </div>

        <!-- 链接区域 (占7列) -->
        <div class="footer-links-section">
          <!-- 快速导航 -->
          <div class="footer-link-group">
            <h3 class="footer-link-group-title">探索</h3>
            <ul class="footer-link-list">
              <li v-for="item in quickLinks" :key="item.path">
                <NuxtLink :to="item.path" class="footer-link-item">
                  <i class="fas fa-chevron-right footer-link-icon"></i>
                  {{ item.title }}
                </NuxtLink>
              </li>
            </ul>
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
const currentYear = new Date().getFullYear()

const quickLinks = [
  { title: '插件工具', path: '/tools' },
  { title: '项目展示', path: '/projects' },
  { title: '技术博客', path: '/blog' },
  { title: '生活随笔', path: '/life' },
  { title: '关于我', path: '/about' }
]

const socialLinks = [
  { name: 'GitHub', icon: 'fab fa-github', link: 'https://github.com/Lijing327' },
  { name: 'Bilibili', icon: 'fas fa-play-circle', link: '#' }, // 替换为实际链接
  { name: 'Email', icon: 'fas fa-envelope', link: 'mailto:contact@溪午听风.com' }
]

const contactInfo = [
  {
    type: 'email',
    icon: '📧',
    label: 'Email Me',
    link: 'mailto:contact@溪午听风.com'
  },
  {
    type: 'github',
    icon: '🐙',
    label: 'GitHub',
    link: 'https://github.com/Lijing327'
  }
]

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
    if (process.env.NODE_ENV === 'development') {
      console.warn('[Footer Stats] 获取统计数据失败:', err?.message || err)
    }
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
</style>
