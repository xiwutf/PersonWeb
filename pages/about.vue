<template>
  <ModuleGuard module-key="about">
    <div class="about-page">
      <!-- 全局背景噪点 -->
      <div class="about-background-noise"></div>

      <!-- 动态背景光斑 -->
      <div class="about-background-container">
        <div class="about-background-blob about-background-blob--blue"></div>
        <div class="about-background-blob about-background-blob--purple"></div>
        <div class="about-background-blob about-background-blob--emerald"></div>
      </div>

      <div class="about-content">
        
        <!-- 个人资料 Hero 区域 -->
        <div class="about-hero">
          <div class="about-avatar-container">
            <!-- 动态光环 -->
            <div class="about-avatar-glow"></div>
            <div class="about-avatar">
              <img src="/images/avatar.jpg" alt="溪午听风" />
            </div>
            <div class="about-status-indicator" title="Online">
              <span class="about-status-ping"></span>
            </div>
          </div>
          
          <h1 class="about-title">溪午听风</h1>
          <p class="about-subtitle">全栈开发者 & AI 探索者</p>
          <p class="about-description">
            热爱技术，痴迷代码。在数字世界中构建未来，在算法海洋里寻找真理。<br>
            专注于 Web 全栈、AI Agent 开发与 Revit 二次开发。
          </p>
          
          <!-- 社交链接 -->
          <div class="about-social-links">
            <a 
              v-for="social in socialLinks" 
              :key="social.name" 
              :href="social.link" 
              target="_blank" 
              class="about-social-link"
            >
              <i :class="social.icon"></i>
            </a>
          </div>
        </div>

        <!-- 技能矩阵 -->
        <div class="about-section">
          <h2 class="about-section-title">
            <span class="about-section-icon about-section-icon--blue">⚡</span> 技能矩阵
          </h2>
          <div class="about-skills-grid">
            <div 
              v-for="category in skillCategories" 
              :key="category.name" 
              class="about-skill-card"
            >
              <div class="about-skill-icon" :class="`about-skill-icon--${category.color}`">
                {{ category.icon }}
              </div>
              <h3 class="about-skill-name">{{ category.name }}</h3>
              <div class="about-skill-tags">
                <span 
                  v-for="skill in category.skills" 
                  :key="skill" 
                  class="about-skill-tag"
                >
                  {{ skill }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- 经历与成就 -->
        <div class="about-timeline-grid">
          <!-- 左侧：项目成就 -->
          <div>
            <h2 class="about-timeline-section-title">
              <span class="about-section-icon about-section-icon--purple">🚀</span> 项目成就
            </h2>
            <div class="about-timeline-list">
              <div 
                v-for="(item, index) in achievements" 
                :key="index" 
                class="about-timeline-item"
              >
                <div class="about-timeline-dot"></div>
                <h3 class="about-timeline-title">{{ item.title }}</h3>
                <p class="about-timeline-meta">{{ item.type }} | {{ item.year }}</p>
                <p class="about-timeline-desc">{{ item.desc }}</p>
              </div>
            </div>
          </div>

          <!-- 右侧：技术历程 -->
          <div>
            <h2 class="about-timeline-section-title">
              <span class="about-section-icon about-section-icon--emerald">💡</span> 技术历程
            </h2>
            <div class="about-timeline-list">
              <div 
                v-for="(item, index) in experience" 
                :key="index" 
                class="about-timeline-item about-timeline-item--emerald"
              >
                <div class="about-timeline-dot"></div>
                <h3 class="about-timeline-title">{{ item.title }}</h3>
                <p class="about-timeline-meta">{{ item.duration }}</p>
                <p class="about-timeline-desc">{{ item.desc }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- 兴趣爱好 -->
        <div class="about-section">
          <h2 class="about-section-title">
            <span class="about-section-icon about-section-icon--pink">🎯</span> 兴趣爱好
          </h2>
          <div class="about-hobbies-grid">
            <div 
              v-for="hobby in hobbies" 
              :key="hobby.name" 
              class="about-hobby-card"
            >
              <div class="about-hobby-icon">{{ hobby.icon }}</div>
              <div class="about-hobby-name">{{ hobby.name }}</div>
            </div>
          </div>
        </div>

        <!-- 联系方式 -->
        <div class="about-contact-section">
          <div class="about-contact-content">
            <h2 class="about-contact-title">保持联系</h2>
            <div class="about-contact-grid">
              <div class="about-contact-card">
                <div class="about-contact-icon about-contact-icon--green">💬</div>
                <div class="about-contact-info">
                  <div class="about-contact-label">WeChat</div>
                  <div class="about-contact-value about-contact-value--green">LinXi-5152</div>
                </div>
              </div>
              <div class="about-contact-card">
                <div class="about-contact-icon about-contact-icon--blue">📧</div>
                <div class="about-contact-info">
                  <div class="about-contact-label">Email</div>
                  <div class="about-contact-value about-contact-value--blue">255106739@qq.com</div>
                </div>
              </div>
            </div>
            
            <!-- 二维码 -->
            <div class="about-contact-qr">
              <div class="about-contact-qr-image">
                <img src="/images/wechat-qr.png" alt="微信二维码" />
              </div>
              <p class="about-contact-qr-text">扫码加好友，请注明来意</p>
            </div>
          </div>
        </div>

      </div>
    </div>
  </ModuleGuard>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

const socialLinks = [
  { name: 'GitHub', icon: 'fab fa-github', link: 'https://github.com/Lijing327' },
  { name: 'Email', icon: 'fas fa-envelope', link: 'mailto:255106739@qq.com' },
  { name: 'Bilibili', icon: 'fas fa-play-circle', link: '#' }
]

const skillCategories = [
  {
    name: 'Web 全栈',
    icon: '💻',
    color: 'blue',
    skills: ['Vue.js', 'Nuxt.js', 'Node.js', 'TypeScript', 'Tailwind CSS', 'MongoDB']
  },
  {
    name: 'AI & LLM',
    icon: '🧠',
    color: 'purple',
    skills: ['LangChain', 'OpenAI API', 'RAG', 'Agent Design', 'Python', 'Prompt Engineering']
  },
  {
    name: 'Revit 开发',
    icon: '🏗️',
    color: 'orange',
    skills: ['Revit API', 'C#', '.NET', 'WPF', 'BIM', '自动化插件']
  },
  {
    name: '嵌入式 & IoT',
    icon: '🔌',
    color: 'emerald',
    skills: ['C/C++', 'STM32', 'Arduino', 'ESP32', '传感器应用', '物联网协议']
  }
]

const achievements = [
  {
    title: 'SmartAssistantAgent',
    type: 'AI 项目',
    year: '2024',
    desc: '基于 LangChain 的智能助手系统，支持多工具集成和知识库检索，实现复杂的任务自动化。'
  },
  {
    title: '恋爱魔方小程序',
    type: '个人项目',
    year: '2024',
    desc: '累计用户 5000+，日活跃用户 800+，好评率 98% 的情侣互动小程序，提供独特的情感连接体验。'
  },
  {
    title: 'Revit 插件工具集',
    type: '商业产品',
    year: '2023-2024',
    desc: '开发多款 Revit 插件，累计销售 1000+ 份，帮助建筑师自动化繁琐流程，显著提升工作效率。'
  }
]

const experience = [
  {
    title: '全栈开发',
    duration: '3年+ 经验',
    desc: '深耕现代 Web 技术栈，具备从前端交互到后端架构的完整开发能力，注重用户体验与代码质量。'
  },
  {
    title: '移动端开发',
    duration: '2年+ 经验',
    desc: '专注于微信小程序生态，具备完整的产品设计、开发与运营能力，打造过数款受欢迎的小程序产品。'
  },
  {
    title: 'AI 应用开发',
    duration: '1年+ 经验',
    desc: '紧跟 AI 浪潮，深入研究 LLM 应用落地，在 Agent 开发、知识库构建等方面积累了实战经验。'
  }
]

const hobbies = [
  { name: '编程开发', icon: '💻' },
  { name: '技术学习', icon: '📚' },
  { name: '投资理财', icon: '💰' },
  { name: '旅行摄影', icon: '📸' }
]

useHead({
  title: '关于我 - 溪午听风',
  meta: [
    { name: 'description', content: '全栈开发者 & AI 探索者' }
  ]
})
</script>

<style scoped>
/* 页面特有样式已移至 assets/css/about.css */
/* 这里只保留组件特有的样式（如果有） */
</style>
