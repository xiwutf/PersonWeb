<template>
  <footer class="bg-gray-900 text-white">
    <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
        <!-- 品牌信息 -->
        <div class="md:col-span-2">
          <div class="flex items-center space-x-3 mb-4">
            <div class="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center">
              <span class="text-white text-lg font-bold">XF</span>
            </div>
            <div>
              <span class="text-xl font-bold">溪午听风</span>
              <p class="text-sm text-gray-400">开发让生活更高效</p>
            </div>
          </div>
          <p class="text-gray-300 text-sm leading-relaxed max-w-md">
            专注于Web开发、Revit插件开发和移动端应用开发，用技术解决实际问题，分享知识与经验。代码就是我的魔方，每一次重构都是对完美的追求。
          </p>
        </div>

        <!-- 快速链接 -->
        <div>
          <h3 class="text-lg font-semibold mb-4">快速链接</h3>
          <ul class="space-y-2">
            <li v-for="item in quickLinks" :key="item.path">
              <NuxtLink
                :to="item.path"
                class="text-gray-300 hover:text-white transition-colors text-sm"
              >
                {{ item.title }}
              </NuxtLink>
            </li>
          </ul>
        </div>

        <!-- 联系方式 -->
        <div>
          <h3 class="text-lg font-semibold mb-4">联系我</h3>
          <div class="space-y-3">
            <a
              v-for="contact in contactInfo"
              :key="contact.type"
              :href="contact.link"
              class="flex items-center space-x-3 text-gray-300 hover:text-white transition-colors text-sm"
            >
              <span class="text-lg">{{ contact.icon }}</span>
              <span>{{ contact.label }}</span>
            </a>
          </div>
        </div>
      </div>

      <!-- 分割线 -->
      <div class="border-t border-gray-800 mt-8 pt-8">
        <div class="flex flex-col md:flex-row justify-between items-center">
          <p class="text-gray-400 text-sm">
            © {{ currentYear }} 溪午听风. 保留所有权利。
          </p>
          <div class="flex items-center space-x-4 mt-4 md:mt-0">
            <span class="text-gray-400 text-sm">Built with</span>
            <div class="flex items-center space-x-2">
              <span class="text-green-400">💚</span>
              <span class="text-gray-400 text-sm">Nuxt 3</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
// 获取当前年份
const currentYear = new Date().getFullYear()

// 快速链接
const quickLinks = [
  { title: '插件工具', path: '/tools' },
  { title: '项目展示', path: '/projects' },
  { title: '技术博客', path: '/blog' },
  { title: '关于我', path: '/about' }
]

// 联系方式
const contactInfo = [
  {
    type: 'email',
    icon: '📧',
    label: '邮箱联系',
    link: 'mailto:contact@溪午听风.com'
  },
  {
    type: 'wechat',
    icon: '💬',
    label: '微信咨询',
    link: '#'
  },
  {
    type: 'github',
    icon: '🐙',
    label: 'GitHub',
    link: 'https://github.com/溪午听风'
  }
]
</script> 