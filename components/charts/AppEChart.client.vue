<template>
  <div class="app-echart-shell">
    <component
      :is="chartComponent"
      v-if="chartComponent"
      class="app-echart"
      :option="option"
      autoresize
    />
    <div v-else class="app-echart-loading">{{ loadingText }}</div>
  </div>
</template>

<script setup lang="ts">
import type { Component } from 'vue'
import type { EChartsOption } from 'echarts'

const props = withDefaults(defineProps<{
  option: EChartsOption
  loadingText?: string
}>(), {
  loadingText: 'Loading chart...'
})

const chartComponent = shallowRef<Component | null>(null)
let chartLoaderPromise: Promise<Component> | null = null

onMounted(async () => {
  if (!chartLoaderPromise) {
    chartLoaderPromise = import('./echarts-runtime').then(module => module.getEChartComponent())
  }

  chartComponent.value = await chartLoaderPromise
})

const option = computed(() => props.option)
</script>

<style scoped>
.app-echart-shell,
.app-echart {
  width: 100%;
  height: 100%;
  min-height: inherit;
}

.app-echart-loading {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  min-height: inherit;
  color: var(--color-text-muted, #64748b);
  font-size: 0.9375rem;
}
</style>
