<template>
  <div class="tools-page">
    <!-- 背景动画 -->
    <div class="tools-background-noise"></div>
    <div class="tools-background-container">
      <div class="tools-background-blob tools-background-blob--orange"></div>
      <div class="tools-background-blob tools-background-blob--red"></div>
      <div class="tools-background-blob tools-background-blob--amber"></div>
    </div>

    <!-- 页面内容 -->
    <div class="tools-content">
      <!-- 面包屑和返回按钮 -->
      <nav class="mb-8 flex items-center justify-between">
        <n-breadcrumb>
          <n-breadcrumb-item>
            <NuxtLink to="/">首页</NuxtLink>
          </n-breadcrumb-item>
          <n-breadcrumb-item>
            <NuxtLink to="/tools">插件工具</NuxtLink>
          </n-breadcrumb-item>
          <n-breadcrumb-item>{{ tool?.title || '...' }}</n-breadcrumb-item>
        </n-breadcrumb>
        <NuxtLink
          to="/tools"
          class="tools-card-button tools-card-button--secondary"
          style="height: 36px; padding: 0 16px;"
        >
          <i class="fas fa-arrow-left mr-2"></i>
          返回
        </NuxtLink>
      </nav>

      <!-- 加载状态 -->
      <div v-if="loading" class="tools-loading">
        <div class="tools-loading-spinner"></div>
        <p class="tools-loading-text">正在解构工具数据...</p>
      </div>

      <!-- 错误状态 -->
       <div v-else-if="error" class="tools-error">
        <n-result
          status="warning"
          title="无法加载工具"
          :description="error || `未找到 slug 为 '${$route.params.slug}' 的工具。`"
        >
          <template #footer>
            <n-button @click="router.push('/tools')" type="primary">返回工具列表</n-button>
          </template>
        </n-result>
      </div>


      <!-- 工具内容 -->
      <div v-else-if="tool" class="space-y-8">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- 左侧-详细介绍 -->
          <div class="lg:col-span-2 space-y-8">
            <div class="tools-card !p-8">
              <h1 class="tools-card-title text-3xl mb-4">{{ tool.title }}</h1>
              <p class="tools-card-description text-base leading-relaxed">{{ tool.description }}</p>
              <div class="tools-card-tags">
                <span v-for="tag in tool.tags" :key="tag" class="tools-card-tag">{{ tag }}</span>
              </div>
            </div>

            <div class="tools-card !p-8">
              <h2 class="tools-card-title text-2xl mb-6">详细介绍</h2>
              <div
                v-if="tool.content"
                class="prose-content"
                v-html="renderMarkdown(tool.content)"
              ></div>
              <div v-else class="text-slate-400 italic">暂无详细描述</div>
            </div>
          </div>

          <!-- 右侧-价格和适用性 -->
          <div class="lg:col-span-1 space-y-8">
             <div class="tools-card !p-6 sticky top-8">
                <div class="tools-card-price mb-6">
                  <span class="tools-card-price-current">¥{{ tool.price }}</span>
                  <span class="tools-card-price-original">¥{{ Math.round(tool.price * 1.5) }}</span>
                </div>
                <button
                  @click="showConsultationDialog = true"
                  class="tools-card-button tools-card-button--primary w-full"
                >
                  <i class="fas fa-comments"></i>
                  咨询详情
                </button>
             </div>
             <div class="tools-card !p-6">
                <h3 class="tools-card-title text-lg mb-4">适用性</h3>
                <div class="space-y-4">
                  <div v-if="tool.fitFor" class="bg-green-900/30 border border-green-500/30 rounded-lg p-4">
                    <h4 class="font-semibold text-green-300 mb-2 flex items-center gap-2"><i class="fas fa-check-circle"></i>适合人群</h4>
                    <div class="text-sm text-green-300/80 whitespace-pre-line">{{ tool.fitFor }}</div>
                  </div>
                  <div v-if="tool.notFitFor" class="bg-red-900/30 border border-red-500/30 rounded-lg p-4">
                    <h4 class="font-semibold text-red-300 mb-2 flex items-center gap-2"><i class="fas fa-times-circle"></i>不适合情况</h4>
                    <div class="text-sm text-red-300/80 whitespace-pre-line">{{ tool.notFitFor }}</div>
                  </div>
                </div>
             </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 咨询弹窗 -->
    <ConsultationDialog
      v-model:visible="showConsultationDialog"
      :product-id="tool?.id || 0"
      :product-name="tool?.title || ''"
      @success="handleConsultationSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import MarkdownIt from 'markdown-it';
import {
  NBreadcrumb,
  NBreadcrumbItem,
  NButton,
  NResult,
  useMessage
} from 'naive-ui';

const router = useRouter();
const route = useRoute();
const api = useApi();
const message = useMessage();
const slug = route.params.slug as string;

const tool = ref<any>(null);
const loading = ref(true);
const error = ref<string | null>(null);
const showConsultationDialog = ref(false);

const md = new MarkdownIt({
  html: true,
  linkify: true,
  typographer: true
});

const fetchTool = async () => {
  loading.value = true;
  error.value = null;
  try {
    const searchRes = await api.get<any>('/Toolbox/marketplace', {
      params: { search: slug }
    });
    
    if (searchRes && searchRes.tools && searchRes.tools.length > 0) {
      const matchedTool = searchRes.tools.find((t: any) => t.slug === slug) || searchRes.tools[0];
      const currentToolId = matchedTool.id;
      
      const detailRes = await api.get<any>(`/Toolbox/${currentToolId}`);
      if (detailRes) {
        tool.value = {
          id: detailRes.id,
          title: detailRes.name,
          description: detailRes.description || '',
          price: detailRes.price || 0,
          date: detailRes.createdAt || new Date().toISOString(),
          tags: detailRes.tags || [],
          _path: `/tools/${detailRes.slug}`,
          content: detailRes.detailedDescription || detailRes.description || '',
          fitFor: detailRes.fitFor || null,
          notFitFor: detailRes.notFitFor || null,
        };
      } else {
        throw new Error('无法获取工具的详细信息');
      }
    } else {
      throw new Error('未在市场中找到该工具');
    }
  } catch (e: any) {
    console.error('获取工具详情失败:', e);
    error.value = e.message || '获取工具数据失败';
  } finally {
    loading.value = false;
  }
};

const formatDate = (dateString: string) => {
  if (!dateString) return '';
  return new Date(dateString).toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
};

const renderMarkdown = (markdown: string) => {
  if (!markdown) return '';
  return md.render(markdown);
};

const handleConsultationSuccess = () => {
  message.success('咨询请求已发送，我们会尽快与您联系！');
};

onMounted(() => {
  fetchTool();
});

useHead({
  title: computed(() => `${tool.value?.title || '工具详情'} - 插件工具 - 溪午听风`),
  meta: [
    { name: 'description', content: computed(() => tool.value?.description || '工具详情页面') },
    { name: 'keywords', content: computed(() => tool.value?.tags?.join(', ') || '插件工具') }
  ]
});
</script>

<style scoped>
/* 使用 a 标签替代 n-breadcrumb-item 中的 span，以便 NuxtLink 生效 */
:deep(.n-breadcrumb-item .n-breadcrumb-item__link) {
  display: contents;
}
:deep(.n-breadcrumb-item .n-breadcrumb-item__separator) {
  color: #64748b; /* slate-500 */
}
:deep(.n-breadcrumb a) {
  color: #94a3b8; /* slate-400 */
  transition: color 0.2s;
}
:deep(.n-breadcrumb a:hover) {
  color: #f1f5f9; /* slate-100 */
}
:deep(.n-breadcrumb .n-breadcrumb-item:last-child .n-breadcrumb-item__link) {
  color: #f8fafc; /* slate-50 */
  font-weight: 600;
}
:deep(.n-result .n-result-header__title) {
  color: white;
}
:deep(.n-result .n-result-header__description) {
  color: #94a3b8; /* slate-400 */
}

.prose-content {
  color: #cbd5e1; /* slate-300 */
  line-height: 1.8;
}

:deep(.prose-content h1),
:deep(.prose-content h2),
:deep(.prose-content h3),
:deep(.prose-content h4) {
  color: #f1f5f9; /* slate-100 */
  font-weight: 600;
  margin-bottom: 1rem;
  border-bottom: 1px solid #334155; /* slate-700 */
  padding-bottom: 0.5rem;
}
:deep(.prose-content h2) { margin-top: 2rem; }
:deep(.prose-content h3) { margin-top: 1.5rem; }

:deep(.prose-content p) {
  margin-bottom: 1rem;
}

:deep(.prose-content ul),
:deep(.prose-content ol) {
  padding-left: 1.5rem;
  margin-bottom: 1rem;
}
:deep(.prose-content li::marker) {
  color: #64748b; /* slate-500 */
}

:deep(.prose-content a) {
  color: #fb923c; /* orange-400 */
  text-decoration: none;
  transition: color 0.2s, border-bottom 0.2s;
  border-bottom: 1px solid transparent;
}
:deep(.prose-content a:hover) {
  color: #fdba74; /* orange-300 */
  border-bottom-color: #fdba74; /* orange-300 */
}

:deep(.prose-content blockquote) {
  border-left: 4px solid #f97316; /* orange-500 */
  background-color: rgba(15, 23, 42, 0.5); /* slate-900/50 */
  padding: 0.5rem 1rem;
  margin: 1rem 0;
  color: #94a3b8; /* slate-400 */
  border-radius: 0 8px 8px 0;
}

:deep(.prose-content code) {
  background-color: #334155; /* slate-700 */
  color: #e2e8f0; /* slate-200 */
  padding: 0.2em 0.4em;
  border-radius: 4px;
  font-size: 0.9em;
}

:deep(.prose-content pre) {
  background-color: #0f172a; /* slate-900 */
  border: 1px solid #334155; /* slate-700 */
  border-radius: 12px;
  padding: 1rem;
  overflow-x: auto;
}

:deep(.prose-content pre code) {
  background-color: transparent;
  padding: 0;
  border: none;
}
</style>
