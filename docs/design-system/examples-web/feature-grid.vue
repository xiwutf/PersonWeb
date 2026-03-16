<!-- FeatureGrid 交互式示例 -->
<template>
  <div class="example-container">
    <div class="example-header">
      <h2>FeatureGrid 示例</h2>
      <p>功能网格组件交互式演示</p>
    </div>

    <div class="controls">
      <n-space :size="16">
        <div>
          <span class="label">列数：</span>
          <n-radio-group v-model:value="columns" :name="'columns'">
            <n-radio-button value="2">2 列</n-radio-button>
            <n-radio-button value="3">3 列</n-radio-button>
            <n-radio-button value="4">4 列</n-radio-button>
          </n-radio-group>
        </div>

        <div>
          <span class="label">显示标题：</span>
          <n-switch v-model:value="showTitle" />
        </div>

        <div>
          <span class="label">显示标签：</span>
          <n-switch v-model:value="showTags" />
        </div>
      </n-space>
    </div>

    <div class="preview-area">
      <FeatureGrid
        :features="displayFeatures"
        :title="showTitle ? '核心功能' : undefined"
        subtitle="为什么选择我们的产品"
        :columns="columns"
        class="feature-preview"
      />
    </div>

    <div class="add-feature">
      <n-input-group>
        <n-input v-model:value="newFeatureTitle" placeholder="输入功能标题" />
        <n-input v-model:value="newFeatureDesc" placeholder="输入功能描述" />
        <n-button type="primary" @click="addFeature">添加</n-button>
      </n-input-group>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import FeatureGrid from '~/components/web/FeatureGrid.vue'

const columns = ref<2 | 3 | 4>(3)
const showTitle = ref(true)
const showTags = ref(true)

const newFeatureTitle = ref('')
const newFeatureDesc = ref('')

const features = ref([
  {
    id: '1',
    title: '智能搜索',
    description: '基于 AI 的全文搜索，快速定位内容',
    icon: 'fa-search',
    iconColor: 'var(--color-primary)',
    tags: ['AI', '搜索']
  },
  {
    id: '2',
    title: '实时同步',
    description: '多端数据实时同步，随时随地访问',
    icon: 'fa-sync',
    iconColor: 'var(--color-success)',
    tags: ['同步', '云端']
  },
  {
    id: '3',
    title: '安全可靠',
    description: '企业级安全保障，保护数据安全',
    icon: 'fa-shield-alt',
    iconColor: 'var(--color-warning)',
    tags: ['安全', '加密']
  },
  {
    id: '4',
    title: '数据分析',
    description: '强大的数据分析工具，洞察业务趋势',
    icon: 'fa-chart-line',
    iconColor: 'var(--color-info)',
    tags: ['分析', '可视化']
  },
  {
    id: '5',
    title: '团队协作',
    description: '支持多人协作，提高团队效率',
    icon: 'fa-users',
    iconColor: 'var(--color-purple-500)',
    tags: ['协作', '团队']
  },
  {
    id: '6',
    title: '自动化工作流',
    description: '自定义工作流，自动化重复任务',
    icon: 'fa-magic',
    iconColor: 'var(--color-pink-500)',
    tags: ['自动化', '效率']
  }
])

const displayFeatures = computed(() => {
  return features.value.map(f => ({
    ...f,
    tags: showTags.value ? f.tags : undefined
  }))
})

const addFeature = () => {
  if (!newFeatureTitle.value.trim() || !newFeatureDesc.value.trim()) {
    window.$message?.warning('请输入功能标题和描述')
    return
  }

  const newFeature = {
    id: String(Date.now()),
    title: newFeatureTitle.value,
    description: newFeatureDesc.value,
    icon: 'fa-star',
    iconColor: 'var(--color-primary)',
    tags: showTags.value ? ['自定义'] : undefined
  }

  features.value.push(newFeature)
  newFeatureTitle.value = ''
  newFeatureDesc.value = ''

  window.$message?.success('功能添加成功')
}
</script>

<style scoped>
.example-container {
  min-height: 100vh;
  background: var(--color-bg-body);
  padding: var(--spacing-xl);
}

.example-header {
  text-align: center;
  margin-bottom: var(--spacing-2xl);
}

.example-header h2 {
  font-size: var(--text-3xl);
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm);
}

.example-header p {
  font-size: var(--text-lg);
  color: var(--color-text-sec);
  margin: 0;
}

.controls {
  max-width: 800px;
  margin: 0 auto var(--spacing-2xl);
  padding: var(--spacing-lg);
  background: var(--color-bg-card);
  border-radius: var(--radius-lg);
  border: 1px solid var(--color-border-default);
}

.label {
  font-weight: 600;
  color: var(--color-text-main);
  margin-right: var(--spacing-sm);
}

.preview-area {
  margin-bottom: var(--spacing-2xl);
}

.add-feature {
  max-width: 800px;
  margin: 0 auto;
}

.feature-preview {
  transition: all 0.3s;
}
</style>
