<template>
  <ModuleGuard module-key="about">
    <div class="about-page">
      <!-- ?????? -->
      <div class="about-background-noise"></div>

      <!-- ???????-->
      <div class="about-background-container">
        <div class="about-background-blob about-background-blob--blue"></div>
        <div class="about-background-blob about-background-blob--purple"></div>
        <div class="about-background-blob about-background-blob--emerald"></div>
      </div>

      <div class="about-content">
        
        <!-- ???? Hero ?? -->
        <div class="about-hero">
          <div class="about-avatar-container">
            <!-- ?????-->
            <div class="about-avatar-glow"></div>
            <div class="about-avatar">
              <img src="/images/avatar.jpg" alt="????" />
            </div>
            <div class="about-status-indicator" title="Online">
              <span class="about-status-ping"></span>
            </div>
          </div>
          
          <h1 class="about-title">????</h1>
          <p class="about-subtitle">????? & AI ???</p>
          <p class="about-description">
            ????????????????????????????????<br>
            ??? Web ???AI Agent ??? Revit ?????
          </p>
          
          <!-- ???? -->
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

        <!-- ?????-->
        <div class="about-section">
          <h2 class="about-section-title">
            <span class="about-section-icon about-section-icon--blue">??</span> ????
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

        <!-- ??????-->
        <div class="about-timeline-grid">
          <!-- ????????-->
          <div>
            <h2 class="about-timeline-section-title">
              <span class="about-section-icon about-section-icon--purple">??</span> ????
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

          <!-- ????????-->
          <div>
            <h2 class="about-timeline-section-title">
              <span class="about-section-icon about-section-icon--emerald">??</span> ?????            </h2>
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

        <!-- ???? -->
        <div class="about-section">
          <h2 class="about-section-title">
            <span class="about-section-icon about-section-icon--pink">??</span> ????
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

        <!-- ??????-->
        <div class="about-section">
          <h2 class="about-section-title">
            <span class="about-section-icon about-section-icon--purple">??</span> ??????          </h2>
          <div class="about-cognition-card">
            <p class="about-cognition-desc">
              ???????????????????????????????????????????????            </p>
            <NuxtLink to="/cognition" class="about-cognition-link">
              <i class="fas fa-arrow-right mr-2"></i>
              ????????            </NuxtLink>
          </div>
        </div>

        <!-- ???? -->
        <div class="about-contact-section">
          <div class="about-contact-content">
            <h2 class="about-contact-title">????</h2>
            <div class="about-contact-grid">
              <div class="about-contact-card">
                <div class="about-contact-icon about-contact-icon--green">??</div>
                <div class="about-contact-info">
                  <div class="about-contact-label">WeChat</div>
                  <div class="about-contact-value about-contact-value--green">LinXi-5152</div>
                </div>
              </div>
              <div class="about-contact-card">
                <div class="about-contact-icon about-contact-icon--blue">??</div>
                <div class="about-contact-info">
                  <div class="about-contact-label">Email</div>
                  <div class="about-contact-value about-contact-value--blue">linxiwanting@gmail.com</div>
                </div>
              </div>
            </div>
            
            <!-- ????-->
            <div class="about-contact-qr">
              <div class="about-contact-qr-image">
                <img src="/images/wechat-qr.png" alt="?????" />
              </div>
              <p class="about-contact-qr-text">??????</p>
            </div>
          </div>
        </div>

      </div>
    </div>
  </ModuleGuard>
</template>

<script setup lang="ts">
// ???? ModuleGuard ????????????import ModuleGuard from '~/components/admin/ModuleGuard.vue'

definePageMeta({
  layout: 'default'
})

const socialLinks = [
  { name: 'GitHub', icon: 'fab fa-github', link: 'https://github.com/Lijing327' },
  { name: 'Email', icon: 'fas fa-envelope', link: 'mailto:linxiwanting@gmail.com' },
  { name: 'Bilibili', icon: 'fas fa-play-circle', link: '#' }
]

const skillCategories = [
  {
    name: 'Web ??',
    icon: '??',
    color: 'blue',
    skills: ['Vue.js', 'Nuxt.js', 'Node.js', 'TypeScript', 'Tailwind CSS', 'MongoDB']
  },
  {
    name: 'AI & LLM',
    icon: '??',
    color: 'purple',
    skills: ['LangChain', 'OpenAI API', 'RAG', 'Agent Design', 'Python', 'Prompt Engineering']
  },
  {
    name: 'Revit ???,
    icon: '????,
    color: 'orange',
    skills: ['Revit API', 'C#', '.NET', 'WPF', 'BIM', '??????]
  },
  {
    name: '????& IoT',
    icon: '??',
    color: 'emerald',
    skills: ['C/C++', 'STM32', 'Arduino', 'ESP32', '??????, '??????]
  }
]

const achievements = [
  {
    title: 'SmartAssistantAgent',
    type: 'AI ??',
    year: '2024',
    desc: '?? LangChain ??????????????????????????????????
  },
  {
    title: '????????,
    type: '????',
    year: '2024',
    desc: '???? 5000+?????? 800+???? 98% ??????????????????????
  },
  {
    title: 'Revit ??????,
    type: '????',
    year: '2023-2024',
    desc: '?????Revit ????????1000+ ?????????????????????????
  }
]

const experience = [
  {
    title: '?????,
    duration: '3?? ??',
    desc: '?????Web ?????????????????????????????????????
  },
  {
    title: '??????,
    duration: '2?? ??',
    desc: '?????????????????????????????????????????????
  },
  {
    title: 'AI ?????,
    duration: '1?? ??',
    desc: '?? AI ????????LLM ?????? Agent ????????????????????
  }
]

const hobbies = [
  { name: '?????, icon: '??' },
  { name: '?????, icon: '??' },
  { name: '????', icon: '??' },
  { name: '????', icon: '??' }
]

useHead({
  title: '????- ????',
  meta: [
    { name: 'description', content: '??????& AI ???? }
  ]
})
</script>

<style scoped>
/* ??????????assets/css/about.css */
/* ??????????????????*/

/* ??????????*/
.about-cognition-card {
  @apply bg-white/80 dark:bg-gray-800/80 backdrop-blur-sm;
  @apply rounded-2xl shadow-lg;
  @apply border border-gray-200 dark:border-gray-700;
  @apply p-6 md:p-8;
  @apply transition-all duration-200;
  @apply hover:shadow-xl;
}

.about-cognition-desc {
  @apply text-gray-600 dark:text-gray-300 mb-4 leading-7;
}

.about-cognition-link {
  @apply inline-flex items-center px-6 py-3 rounded-lg;
  @apply bg-gradient-to-r from-purple-500 to-blue-500;
  @apply text-var(--color-bg-light, white) font-medium;
  @apply transition-all duration-200;
  @apply hover:from-purple-600 hover:to-blue-600;
  @apply hover:shadow-lg hover:scale-105;
}
</style>
