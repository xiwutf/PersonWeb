<template>
  <!-- 
    AI 瀹為獙瀹ゆā锟?    moduleId: ai_lab
    
    濡備綍鍦ㄥ悗鍙伴厤缃腑鎺у埗杩欎釜妯″潡鐨勪富棰橈細
    1. 鐧诲綍鍚庡彴锛岃繘鍏ヤ富棰樼鐞嗛〉锟?    2. 鎵惧埌"AI 瀹為獙瀹ゆā锟?閰嶇疆锟?    3. 閫夋嫨"璺熼殢鍏ㄥ眬"鎴栨寚瀹氱嫭绔嬩富棰橈紙锟?"tech-blue"锟?    4. 淇濆瓨鍚庯紝鍒锋柊椤甸潰鍗冲彲鐪嬪埌鏁堟灉
  -->
  <div 
    class="min-h-screen bg-bg-body text-text-main"
    :data-module-theme="moduleTheme || undefined"
  >
    <!-- 椤堕儴 Hero -->
    <section class="relative pt-24 pb-16 border-b border-border-subtle overflow-hidden">
      <div class="absolute inset-0 pointer-events-none">
        <div class="absolute -top-32 -left-32 w-80 h-80 bg-blue-600/30 rounded-full blur-3xl"></div>
        <div class="absolute -bottom-40 right-0 w-96 h-96 bg-purple-600/30 rounded-full blur-3xl"></div>
        <div class="absolute inset-0 bg-[url('/images/grid.svg')] opacity-10"></div>
      </div>

      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid lg:grid-cols-2 gap-10 items-center">
          <div>
            <!-- 鏍囩鍜屾爣棰橈細浣跨敤涓婚鏂囧瓧棰滆壊锛屾浛鎹㈠啓姝荤殑 var(--color-bg-light, white)/slate 棰滆壊 -->
            <p class="inline-flex items-center px-3 py-1 rounded-full bg-primary-soft border border-border-subtle text-xs mb-4 text-text-muted">
              <span class="w-1.5 h-1.5 rounded-full bg-primary mr-2"></span>
              AI 路 3D 路 浜や簰瀹為獙
            </p>
            <h1 class="text-3xl sm:text-4xl lg:text-5xl font-bold mb-4 text-text-main">
              AI 瀹為獙锟?              <span class="block text-lg sm:text-xl font-normal text-text-muted mt-3">
                鍦ㄨ繖閲岀帺涓€鐜╂湁瓒ｇ殑 3D 鍦烘櫙锟?AI 灏忓疄楠岋拷?              </span>
            </h1>
            <p class="text-sm sm:text-base text-text-muted leading-relaxed mb-4">
              姣忎竴锟?3D 鐗╀綋閮戒唬琛ㄤ竴涓叆鍙ｏ細鍗氬銆侀」鐩€佹暟鎹华琛ㄧ洏鈥︼拷?              鏈潵浼氶€愭鎺ュ叆 AI 鏅鸿兘浣撱€佸皬宸ュ叿鍜屽疄楠屾€у姛鑳斤拷?            </p>
            <div class="mt-4">
              <NuxtLink to="/ai" class="inline-flex items-center text-primary hover:text-primary-hover text-sm transition-colors">
                <i class="fas fa-robot mr-2"></i>
                鏌ョ湅 AI 宸ヤ綔鍙帮紙鏅鸿兘浣撲笌宸ヤ綔娴侊級锟?              </NuxtLink>
            </div>
          </div>

          <div class="hidden lg:block">
            <!-- 鍙互鏀句竴涓缉锟?3D 鍦烘櫙鎴栫畝鍗曟彃锟?-->
            <div class="relative w-full aspect-square">
              <div class="absolute inset-0 bg-gradient-to-br from-cyan-500/20 to-purple-500/20 rounded-3xl blur-3xl"></div>
              <!-- 3D 鍦烘櫙瀹瑰櫒锛氫娇鐢ㄤ富棰樿儗鏅拰杈规棰滆壊 -->
              <div class="relative bg-bg-card backdrop-blur-xl rounded-3xl border border-border-subtle p-8 h-full flex items-center justify-center">
                <div class="text-center">
                  <div class="text-6xl mb-4">馃敩</div>
                  <p class="text-text-muted text-sm">瀹為獙鎬у姛鑳藉睍绀哄尯</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 3D 瀹為獙锟?-->
    <section class="py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <!-- 鍗氬鍦扮悆锛氫娇鐢ㄤ富棰樿儗鏅拰杈规棰滆壊锛屾浛鎹㈠啓姝荤殑 slate 棰滆壊 -->
          <AppCard class="p-6 backdrop-blur-xl">
            <h2 class="text-base font-semibold mb-3 flex items-center justify-between text-text-main">
              <span>馃實 鍗氬鏄熺悆</span>
              <NuxtLink to="/blog" class="text-xs text-primary hover:text-primary-hover transition-colors">杩涘叆鍗氬</NuxtLink>
            </h2>
            <p class="text-xs text-text-muted mb-4">
              鍥寸粫鎶€鏈€佹€濊€冧笌瀹炶返鐨勮褰曘€傜偣鍑诲湴鐞冨彲浠ヨ烦杞嚦鍗氬鍒楄〃锟?            </p>
            <Scene3D type="earth" :show-hint="false" height="260px" />
          </AppCard>

          <!-- 椤圭洰椋炶埞锛氫娇鐢ㄤ富棰樿儗鏅拰杈规棰滆壊 -->
          <AppCard class="p-6 backdrop-blur-xl">
            <h2 class="text-base font-semibold mb-3 flex items-center justify-between text-text-main">
              <span>馃殌 椤圭洰椋炶埞</span>
              <NuxtLink to="/projects" class="text-xs text-primary hover:text-primary-hover transition-colors">鏌ョ湅椤圭洰</NuxtLink>
            </h2>
            <p class="text-xs text-text-muted mb-4">
              瀹炴垬椤圭洰涓庡紑婧愬皾璇曘€傜偣鍑婚鑸硅繘鍏ラ」鐩綔鍝侀泦锟?            </p>
            <Scene3D type="spaceship" :show-hint="false" height="260px" />
          </AppCard>

          <!-- 鏁版嵁鏄熺悆锛氫娇鐢ㄤ富棰樿儗鏅拰杈规棰滆壊 -->
          <AppCard class="p-6 backdrop-blur-xl">
            <h2 class="text-base font-semibold mb-3 flex items-center justify-between text-text-main">
              <span>馃拵 鏁版嵁鏄熺悆</span>
              <NuxtLink to="/dashboard" class="text-xs text-primary hover:text-primary-hover transition-colors">
                鎵撳紑浠〃锟?              </NuxtLink>
            </h2>
            <p class="text-xs text-text-muted mb-4">
              灞曠ず绔欑偣璁块棶銆佹枃绔犳暟鎹瓑鎸囨爣銆傚悗缁彲浠ユ帴鍏ユ洿澶氬彲瑙嗗寲缁勪欢锟?            </p>
            <Scene3D type="datasphere" :show-hint="false" height="260px" />
          </AppCard>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import AppCard from '~/components/ui/AppCard.vue'
import Scene3D from '~/components/three/Scene3D.vue'

// 浣跨敤妯″潡涓婚 composable
const { moduleTheme } = useModuleTheme('ai_lab')

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'AI 实验室 - 溪午听风',
  meta: [{ name: 'description', content: '3D 场景与 AI 小实验的集合地。' }]
})
</script>

<style scoped>
/* 椤甸潰鏍峰紡 */
</style>


