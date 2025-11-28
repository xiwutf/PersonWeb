<template>
  <ModuleGuard module-key="about">
    <div class="min-h-screen bg-[#0f172a] text-slate-200 relative overflow-hidden font-['Outfit']">
    <!-- 全局背景噪点 -->
    <div class="fixed inset-0 opacity-[0.03] mix-blend-overlay pointer-events-none z-50"
         style="background-image: url(&quot;data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E&quot;);">
    </div>

    <!-- 动态背景光斑 -->
    <div class="fixed inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-0 left-1/2 -translate-x-1/2 w-[800px] h-[800px] bg-blue-600/10 rounded-full blur-[120px] animate-pulse mix-blend-screen"></div>
      <div class="absolute bottom-0 right-0 w-[600px] h-[600px] bg-purple-600/10 rounded-full blur-[120px] animate-blob mix-blend-screen"></div>
      <div class="absolute bottom-0 left-0 w-[600px] h-[600px] bg-emerald-600/10 rounded-full blur-[120px] animate-blob animation-delay-2000 mix-blend-screen"></div>
    </div>

    <div class="relative z-10 max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-20">
      
      <!-- 个人资料 Hero 区域 -->
      <div class="flex flex-col items-center text-center mb-24">
        <div class="relative mb-8 group">
          <!-- 动态光环 -->
          <div class="absolute -inset-4 bg-gradient-to-r from-blue-500 via-purple-500 to-emerald-500 rounded-full opacity-75 blur-lg group-hover:opacity-100 transition duration-1000 group-hover:duration-200 animate-tilt"></div>
          <div class="relative w-40 h-40 rounded-full overflow-hidden border-4 border-slate-800 shadow-2xl">
            <img src="/images/avatar.jpg" alt="溪午听风" class="w-full h-full object-cover transform group-hover:scale-110 transition duration-700" />
          </div>
          <div class="absolute bottom-2 right-2 w-8 h-8 bg-emerald-500 rounded-full border-4 border-slate-900 flex items-center justify-center" title="Online">
            <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75"></span>
          </div>
        </div>
        
        <h1 class="text-5xl md:text-6xl font-bold mb-4 bg-clip-text text-transparent bg-gradient-to-r from-blue-200 via-white to-purple-200 tracking-tight">
          溪午听风
        </h1>
        <p class="text-xl text-blue-400 font-medium mb-6">全栈开发者 & AI 探索者</p>
        <p class="max-w-2xl text-slate-400 leading-relaxed text-lg">
          热爱技术，痴迷代码。在数字世界中构建未来，在算法海洋里寻找真理。<br>
          专注于 Web 全栈、AI Agent 开发与 Revit 二次开发。
        </p>
        
        <!-- 社交链接 -->
        <div class="flex gap-4 mt-8">
          <a v-for="social in socialLinks" :key="social.name" :href="social.link" target="_blank" 
             class="w-12 h-12 rounded-full bg-slate-800/50 backdrop-blur-md border border-white/10 flex items-center justify-center text-slate-400 hover:bg-white hover:text-slate-900 hover:scale-110 transition-all duration-300 group">
            <i :class="social.icon" class="text-xl"></i>
          </a>
        </div>
      </div>

      <!-- 技能矩阵 -->
      <div class="mb-24">
        <h2 class="text-3xl font-bold text-center mb-12 flex items-center justify-center gap-3">
          <span class="text-blue-500">⚡</span> 技能矩阵
        </h2>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div v-for="category in skillCategories" :key="category.name" 
               class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-2xl p-6 hover:bg-slate-800/50 transition-all duration-500 hover:border-blue-500/30 group">
            <div class="w-12 h-12 rounded-xl bg-gradient-to-br mb-4 flex items-center justify-center text-2xl shadow-lg" :class="category.bgClass">
              {{ category.icon }}
            </div>
            <h3 class="text-xl font-bold text-slate-200 mb-4">{{ category.name }}</h3>
            <div class="flex flex-wrap gap-2">
              <span v-for="skill in category.skills" :key="skill" 
                    class="px-2 py-1 bg-slate-700/50 text-slate-300 text-xs rounded border border-white/5 group-hover:border-white/10 transition-colors">
                {{ skill }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- 经历与成就 -->
      <div class="grid md:grid-cols-2 gap-12 mb-24">
        <!-- 左侧：项目成就 -->
        <div>
          <h2 class="text-2xl font-bold mb-8 flex items-center gap-3">
            <span class="text-purple-500">🚀</span> 项目成就
          </h2>
          <div class="space-y-6">
            <div v-for="(item, index) in achievements" :key="index" 
                 class="relative pl-8 border-l-2 border-slate-700 hover:border-purple-500 transition-colors duration-300 group">
              <div class="absolute -left-[9px] top-0 w-4 h-4 rounded-full bg-slate-900 border-2 border-slate-600 group-hover:border-purple-500 transition-colors"></div>
              <h3 class="text-lg font-bold text-slate-200 group-hover:text-purple-400 transition-colors">{{ item.title }}</h3>
              <p class="text-sm text-purple-400/80 mb-2">{{ item.type }} | {{ item.year }}</p>
              <p class="text-slate-400 text-sm leading-relaxed">{{ item.desc }}</p>
            </div>
          </div>
        </div>

        <!-- 右侧：技术历程 -->
        <div>
          <h2 class="text-2xl font-bold mb-8 flex items-center gap-3">
            <span class="text-emerald-500">💡</span> 技术历程
          </h2>
          <div class="space-y-6">
            <div v-for="(item, index) in experience" :key="index" 
                 class="relative pl-8 border-l-2 border-slate-700 hover:border-emerald-500 transition-colors duration-300 group">
              <div class="absolute -left-[9px] top-0 w-4 h-4 rounded-full bg-slate-900 border-2 border-slate-600 group-hover:border-emerald-500 transition-colors"></div>
              <h3 class="text-lg font-bold text-slate-200 group-hover:text-emerald-400 transition-colors">{{ item.title }}</h3>
              <p class="text-sm text-emerald-400/80 mb-2">{{ item.duration }}</p>
              <p class="text-slate-400 text-sm leading-relaxed">{{ item.desc }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- 兴趣爱好 -->
      <div class="mb-24">
        <h2 class="text-3xl font-bold text-center mb-12 flex items-center justify-center gap-3">
          <span class="text-pink-500">🎯</span> 兴趣爱好
        </h2>
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
          <div v-for="hobby in hobbies" :key="hobby.name" 
               class="bg-slate-800/30 backdrop-blur-md border border-white/5 rounded-2xl p-6 text-center hover:bg-slate-800/50 hover:-translate-y-1 transition-all duration-300">
            <div class="text-4xl mb-3">{{ hobby.icon }}</div>
            <div class="font-medium text-slate-300">{{ hobby.name }}</div>
          </div>
        </div>
      </div>

      <!-- 联系方式 -->
      <div class="bg-gradient-to-r from-blue-900/40 to-purple-900/40 backdrop-blur-md border border-white/10 rounded-3xl p-8 md:p-12 text-center relative overflow-hidden">
        <div class="relative z-10">
          <h2 class="text-3xl font-bold mb-8">保持联系</h2>
          <div class="grid md:grid-cols-2 gap-8 max-w-2xl mx-auto">
            <div class="bg-slate-900/50 rounded-xl p-6 flex items-center gap-4 hover:bg-slate-800/50 transition-colors">
              <div class="w-12 h-12 bg-green-500/20 rounded-lg flex items-center justify-center text-2xl">💬</div>
              <div class="text-left">
                <div class="text-xs text-slate-400 uppercase tracking-wider">WeChat</div>
                <div class="text-lg font-mono text-green-400">LinXi-5152</div>
              </div>
            </div>
            <div class="bg-slate-900/50 rounded-xl p-6 flex items-center gap-4 hover:bg-slate-800/50 transition-colors">
              <div class="w-12 h-12 bg-blue-500/20 rounded-lg flex items-center justify-center text-2xl">📧</div>
              <div class="text-left">
                <div class="text-xs text-slate-400 uppercase tracking-wider">Email</div>
                <div class="text-lg font-mono text-blue-400">255106739@qq.com</div>
              </div>
            </div>
          </div>
          
          <!-- 二维码 -->
          <div class="mt-8 pt-8 border-t border-white/5 flex flex-col items-center">
             <div class="w-32 h-32 bg-white p-2 rounded-xl mb-4">
                <img src="/images/wechat-qr.png" alt="微信二维码" class="w-full h-full object-cover" />
             </div>
             <p class="text-sm text-slate-400">扫码加好友，请注明来意</p>
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
    bgClass: 'from-blue-500/20 to-cyan-500/20 text-blue-400',
    skills: ['Vue.js', 'Nuxt.js', 'Node.js', 'TypeScript', 'Tailwind CSS', 'MongoDB']
  },
  {
    name: 'AI & LLM',
    icon: '🧠',
    bgClass: 'from-purple-500/20 to-pink-500/20 text-purple-400',
    skills: ['LangChain', 'OpenAI API', 'RAG', 'Agent Design', 'Python', 'Prompt Engineering']
  },
  {
    name: 'Revit 开发',
    icon: '🏗️',
    bgClass: 'from-orange-500/20 to-red-500/20 text-orange-400',
    skills: ['Revit API', 'C#', '.NET', 'WPF', 'BIM', '自动化插件']
  },
  {
    name: '嵌入式 & IoT',
    icon: '🔌',
    bgClass: 'from-emerald-500/20 to-teal-500/20 text-emerald-400',
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
.animate-blob {
  animation: blob 7s infinite;
}
.animation-delay-2000 {
  animation-delay: 2s;
}
@keyframes blob {
  0% { transform: translate(0px, 0px) scale(1); }
  33% { transform: translate(30px, -50px) scale(1.1); }
  66% { transform: translate(-20px, 20px) scale(0.9); }
  100% { transform: translate(0px, 0px) scale(1); }
}

.animate-float {
  animation: float 6s ease-in-out infinite;
}
@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-10px); }
}

.animate-tilt {
  animation: tilt 10s infinite linear;
}
@keyframes tilt {
  0%, 50%, 100% { transform: rotate(0deg); }
  25% { transform: rotate(1deg); }
  75% { transform: rotate(-1deg); }
}
</style>
