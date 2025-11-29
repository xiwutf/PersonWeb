<template>
  <div>
    <!-- ==================== 统一主题系统示例区域 ==================== -->
    <!-- 
      这部分是统一主题系统的示例接入，用于验证主题切换功能是否正常工作
      后续可以移除或整合到其他位置
    -->
    <div class="fixed top-4 right-4 z-50">
      <AppCard class="w-80" hover>
        <div class="space-y-4">
          <h3 class="text-text-main font-bold text-lg">主题切换示例</h3>
          
          <!-- 主题选择下拉框 -->
          <div>
            <label class="block text-text-muted text-sm mb-2">选择主题</label>
            <select
              :value="currentTheme"
              @change="(e) => setTheme((e.target as HTMLSelectElement).value as ThemeName)"
              class="w-full px-3 py-2 bg-bg-card border border-border-default rounded-md text-text-main focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option value="light">浅色 (Light)</option>
              <option value="dark">深色 (Dark)</option>
              <option value="tech-blue">科技蓝 (Tech Blue)</option>
            </select>
          </div>

          <!-- 快速切换按钮 -->
          <div class="flex gap-2">
            <AppButton variant="secondary" size="sm" @click="toggleDark">
              {{ currentTheme === 'dark' ? '切换到浅色' : '切换到深色' }}
            </AppButton>
          </div>

          <!-- 当前主题显示 -->
          <div class="pt-2 border-t border-border-subtle">
            <p class="text-text-muted text-sm">
              当前主题: <span class="text-text-main font-medium">{{ currentTheme }}</span>
            </p>
          </div>
        </div>
      </AppCard>
    </div>

    <!-- 基础组件展示区域（用于验证主题效果） -->
    <div class="fixed bottom-4 right-4 z-50">
      <AppCard class="w-80" elevated>
        <div class="space-y-4">
          <h3 class="text-text-main font-bold text-lg">基础组件预览</h3>
          
          <!-- 按钮示例 -->
          <div class="space-y-2">
            <AppButton variant="primary" size="md" full-width>
              主要按钮
            </AppButton>
            <AppButton variant="secondary" size="md" full-width>
              次要按钮
            </AppButton>
            <AppButton variant="ghost" size="md" full-width>
              幽灵按钮
            </AppButton>
          </div>

          <!-- 文字颜色示例 -->
          <div class="pt-2 border-t border-border-subtle space-y-1">
            <p class="text-text-main">主要文字颜色</p>
            <p class="text-text-muted">次要文字颜色</p>
            <p class="text-text-disabled">禁用文字颜色</p>
          </div>
        </div>
      </AppCard>
    </div>

    <!-- 动态加载风格组件 -->
    <component :is="currentComponent" v-if="currentComponent" />
    <div v-else class="min-h-screen flex items-center justify-center">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
        <p class="text-gray-600">加载中...</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { ThemeName } from '~/constants/design/tokens'
import HomeDarkLab from '~/components/home/HomeDarkLab.vue'
import HomeLightPortfolio from '~/components/home/HomeLightPortfolio.vue'

const api = useApi()
const style = ref<string>('dark-lab')

// ==================== 统一主题系统示例 ====================
// 使用 useTheme composable 获取主题切换功能
const { currentTheme, setTheme, toggleDark } = useTheme()

// 风格组件映射
const componentMap: Record<string, any> = {
  'dark-lab': HomeDarkLab,
  'light-portfolio': HomeLightPortfolio
}

// 当前组件
const currentComponent = computed(() => {
  return componentMap[style.value] || componentMap['dark-lab']
})

// 获取当前启用的首页风格
const fetchHomeStyle = async () => {
  try {
    const res = await api.get<{ style: string }>('/config/home-style')
    if (res && res.style) {
      style.value = res.style
    }
  } catch (e) {
    console.error('Failed to fetch home style:', e)
    // 默认使用 dark-lab
    style.value = 'dark-lab'
  }
}

onMounted(() => {
  fetchHomeStyle()
})

definePageMeta({
  layout: 'home'
})

useHead({
  title: '溪午听风 - 数字花园',
  meta: [{ name: 'description', content: '溪午听风的个人网站，分享技术、生活与思考。' }]
})
</script>
