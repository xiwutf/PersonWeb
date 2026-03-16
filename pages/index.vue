<template>
  <div>
    <!-- ��̬���ط����� -->
    <component :is="currentComponent" v-if="currentComponent" />
    <div v-else class="min-h-screen flex items-center justify-center bg-black text-var(--color-bg-light, white)">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-400 mx-auto mb-4"></div>
        <p class="text-var(--color-bg-light, white)/60">加载首页组件中...</p>
        <p class="text-var(--color-bg-light, white)/40 text-sm mt-2">当前风格: {{ style }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import HomeDarkLab from '~/components/home/HomeDarkLab.vue'
import HomeLightPortfolio from '~/components/home/HomeLightPortfolio.vue'
import HomeHybridSuper from '~/components/home/HomeHybridSuper.vue'
import HomeCreative from '~/components/home/HomeCreative.vue'

const api = useApi()
const style = ref<string>('creative') // Ĭ��ʹ���µĴ���ѵ����

// ������ӳ��
const componentMap: Record<string, any> = {
  'dark-lab': HomeDarkLab,
  'light-portfolio': HomeLightPortfolio,
  'hybrid-super': HomeHybridSuper,
  'creative': HomeCreative // �������
}

// ��ǰ���
const currentComponent = computed(() => {
  return componentMap[style.value] || componentMap['creative']
})

// ��ȡ��ǰ���õ���ҳ���
const fetchHomeStyle = async () => {
  try {
    const res = await api.get<{ style: string }>('/config/home-style')
    if (res && res.style) {
      // ����ʹ�ú�����ã����û����ʹ��Ĭ��
      style.value = res.style || 'creative'
    } else {
      style.value = 'creative'
    }
  } catch (e) {
    console.error('Failed to fetch home style:', e)
    // Ĭ��ʹ�� creative
    style.value = 'creative'
  }
}

onMounted(() => {
  fetchHomeStyle()
})

definePageMeta({
  layout: 'home'
})

useHead({
  title: 'Ϫ������ - ���ֻ�԰',
  meta: [{ name: 'description', content: 'Ϫ������ĸ�����վ������������������˼����' }]
})
</script>
