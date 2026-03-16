<template>
  <div class="font-management-page">
    <div class="page-header">
      <h1 class="page-title">字体管理</h1>
      <p class="page-description">管理系统字体配置，包括字体族、字号、字重等设置</p>
    </div>

    <div class="font-settings-container">
      <!-- 全局字体设置 -->
      <div class="settings-section">
        <div class="section-header">
          <h2 class="section-title">
            <i class="fas fa-font mr-2"></i>
            全局字体设置
          </h2>
          <p class="section-description">这些设置将应用到整个网站</p>
        </div>

        <div class="settings-form">
          <!-- 字体族设置 -->
          <div class="form-group">
            <label class="form-label">
              <i class="fas fa-font mr-2"></i>
              字体族 (Font Family)
            </label>
            <p class="form-hint">选择网站的主要字体</p>
            <select
              v-model="fontSettings.fontFamily"
              class="form-input"
            >
              <option value="-apple-system, BlinkMacSystemFont, 'Segoe UI', 'Microsoft YaHei', 'PingFang SC', 'Hiragino Sans GB', 'Helvetica Neue', Arial, sans-serif">
                系统默认（推荐）
              </option>
              <option value="'Microsoft YaHei', 'PingFang SC', 'Hiragino Sans GB', 'SimHei', sans-serif">
                微软雅黑 / 苹方
              </option>
              <option value="'SimSun', 'STSong', serif">
                宋体
              </option>
              <option value="'SimHei', 'STHeiti', sans-serif">
                黑体
              </option>
              <option value="'KaiTi', 'STKaiti', serif">
                楷体
              </option>
              <option value="'FangSong', 'STFangsong', serif">
                仿宋
              </option>
              <option value="'Courier New', 'Monaco', monospace">
                等宽字体
              </option>
            </select>
            <div class="font-preview">
              <p class="preview-label">预览效果：</p>
              <p class="preview-text" :style="{ fontFamily: fontSettings.fontFamily }">
                这是字体预览文字：溪午听风 - 数字花园
              </p>
            </div>
          </div>

          <!-- 字号设置 -->
          <div class="form-group">
            <label class="form-label">
              <i class="fas fa-text-height mr-2"></i>
              基础字号 (Base Font Size)
            </label>
            <p class="form-hint">选择网站的基础字号</p>
            <select
              v-model.number="fontSettings.baseFontSize"
              class="form-input"
            >
              <option :value="12">12px - 较小</option>
              <option :value="14">14px - 小</option>
              <option :value="16">16px - 标准（推荐）</option>
              <option :value="18">18px - 大</option>
              <option :value="20">20px - 较大</option>
            </select>
            <div class="font-preview">
              <p class="preview-label">预览效果：</p>
              <p class="preview-text" :style="{ fontSize: `${fontSettings.baseFontSize}px` }">
                这是字号预览文字：溪午听风 - 数字花园
              </p>
            </div>
          </div>

          <!-- 字重设置 -->
          <div class="form-group">
            <label class="form-label">
              <i class="fas fa-bold mr-2"></i>
              默认字重 (Font Weight)
            </label>
            <p class="form-hint">设置默认文字粗细</p>
            <select
              v-model="fontSettings.defaultFontWeight"
              class="form-input"
            >
              <option value="300">Light (300)</option>
              <option value="400">Normal (400)</option>
              <option value="500">Medium (500)</option>
              <option value="600">Semi Bold (600)</option>
              <option value="700">Bold (700)</option>
            </select>
            <div class="font-preview">
              <p class="preview-label">预览效果：</p>
              <p class="preview-text" :style="{ fontWeight: fontSettings.defaultFontWeight }">
                这是字重预览文字：溪午听风 - 数字花园
              </p>
            </div>
          </div>

          <!-- 行高设置 -->
          <div class="form-group">
            <label class="form-label">
              <i class="fas fa-arrows-alt-v mr-2"></i>
              默认行高 (Line Height)
            </label>
            <p class="form-hint">选择默认行高，影响文字行间距</p>
            <select
              v-model.number="fontSettings.defaultLineHeight"
              class="form-input"
            >
              <option :value="1.2">1.2 - 紧凑</option>
              <option :value="1.4">1.4 - 较紧凑</option>
              <option :value="1.5">1.5 - 标准</option>
              <option :value="1.6">1.6 - 舒适（推荐）</option>
              <option :value="1.8">1.8 - 宽松</option>
              <option :value="2.0">2.0 - 很宽松</option>
            </select>
            <div class="font-preview">
              <p class="preview-label">预览效果：</p>
              <p class="preview-text" :style="{ lineHeight: fontSettings.defaultLineHeight }">
                这是行高预览文字：溪午听风 - 数字花园<br>
                第二行文字用于展示行高效果
              </p>
            </div>
          </div>

          <!-- 字体颜色设置 -->
          <div class="form-group">
            <label class="form-label">
              <i class="fas fa-palette mr-2"></i>
              主文字颜色 (Main Text Color)
            </label>
            <p class="form-hint">选择网站主要文字的颜色</p>
            <select
              v-model="fontSettings.textMainColor"
              class="form-input"
            >
              <option value="var(--color-text-main)">深色（浅色主题）</option>
              <option value="var(--color-border-default)">深灰（浅色主题）</option>
              <option value="var(--color-slate-700)">中灰（浅色主题）</option>
              <option value="rgba(255, 255, 255, 0.95)">亮白（深色主题）</option>
              <option value="rgba(255, 255, 255, 0.85)">白色（深色主题，推荐）</option>
              <option value="rgba(255, 255, 255, 0.75)">浅白（深色主题）</option>
            </select>
            <div class="font-preview">
              <p class="preview-label">预览效果：</p>
              <p class="preview-text" :style="{ color: fontSettings.textMainColor }">
                这是主文字颜色预览：溪午听风 - 数字花园
              </p>
            </div>
          </div>

          <!-- 次要文字颜色设置 -->
          <div class="form-group">
            <label class="form-label">
              <i class="fas fa-palette mr-2"></i>
              次要文字颜色 (Muted Text Color)
            </label>
            <p class="form-hint">选择次要文字（如描述、提示文字）的颜色</p>
            <select
              v-model="fontSettings.textMutedColor"
              class="form-input"
            >
              <option value="var(--color-text-sec)">中灰（浅色主题）</option>
              <option value="var(--color-text-muted)">浅灰（浅色主题）</option>
              <option value="var(--color-slate-300)">很浅灰（浅色主题）</option>
              <option value="rgba(255, 255, 255, 0.7)">亮白（深色主题）</option>
              <option value="rgba(255, 255, 255, 0.6)">白色（深色主题，推荐）</option>
              <option value="rgba(255, 255, 255, 0.5)">浅白（深色主题）</option>
            </select>
            <div class="font-preview">
              <p class="preview-label">预览效果：</p>
              <p class="preview-text" :style="{ color: fontSettings.textMutedColor }">
                这是次要文字颜色预览：溪午听风 - 数字花园
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- 标题字号设置 -->
      <div class="settings-section">
        <div class="section-header">
          <h2 class="section-title">
            <i class="fas fa-heading mr-2"></i>
            标题字号设置
          </h2>
          <p class="section-description">设置各级标题的字号大小</p>
        </div>

        <div class="settings-form">
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">H1 标题字号</label>
              <select
                v-model.number="fontSettings.h1FontSize"
                class="form-input"
              >
                <option :value="24">24px - 小</option>
                <option :value="28">28px - 中小</option>
                <option :value="32">32px - 标准（推荐）</option>
                <option :value="36">36px - 大</option>
                <option :value="40">40px - 较大</option>
                <option :value="48">48px - 很大</option>
              </select>
              <div class="font-preview-small">
                <h1 :style="{ fontSize: `${fontSettings.h1FontSize}px` }">H1 标题预览</h1>
              </div>
            </div>

            <div class="form-group">
              <label class="form-label">H2 标题字号</label>
              <select
                v-model.number="fontSettings.h2FontSize"
                class="form-input"
              >
                <option :value="20">20px - 小</option>
                <option :value="22">22px - 中小</option>
                <option :value="24">24px - 标准（推荐）</option>
                <option :value="28">28px - 大</option>
                <option :value="32">32px - 较大</option>
                <option :value="36">36px - 很大</option>
              </select>
              <div class="font-preview-small">
                <h2 :style="{ fontSize: `${fontSettings.h2FontSize}px` }">H2 标题预览</h2>
              </div>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label">H3 标题字号</label>
              <select
                v-model.number="fontSettings.h3FontSize"
                class="form-input"
              >
                <option :value="18">18px - 小</option>
                <option :value="19">19px - 中小</option>
                <option :value="20">20px - 标准（推荐）</option>
                <option :value="22">22px - 大</option>
                <option :value="24">24px - 较大</option>
                <option :value="28">28px - 很大</option>
              </select>
              <div class="font-preview-small">
                <h3 :style="{ fontSize: `${fontSettings.h3FontSize}px` }">H3 标题预览</h3>
              </div>
            </div>

            <div class="form-group">
              <label class="form-label">H4 标题字号</label>
              <select
                v-model.number="fontSettings.h4FontSize"
                class="form-input"
              >
                <option :value="16">16px - 小</option>
                <option :value="17">17px - 中小</option>
                <option :value="18">18px - 标准（推荐）</option>
                <option :value="20">20px - 大</option>
                <option :value="22">22px - 较大</option>
                <option :value="24">24px - 很大</option>
              </select>
              <div class="font-preview-small">
                <h4 :style="{ fontSize: `${fontSettings.h4FontSize}px` }">H4 标题预览</h4>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 操作按钮 -->
      <div class="actions-section">
        <button
          @click="saveFontSettings"
          class="btn-primary"
          :disabled="saving"
        >
          <i class="fas fa-save mr-2"></i>
          {{ saving ? '保存中...' : '保存字体设置' }}
        </button>
        <button
          @click="resetToDefaults"
          class="btn-secondary"
          :disabled="saving"
        >
          <i class="fas fa-undo mr-2"></i>
          恢复默认设置
        </button>
        <button
          @click="previewFontSettings"
          class="btn-secondary"
          :disabled="saving"
        >
          <i class="fas fa-eye mr-2"></i>
          预览效果
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useNotification } from '~/composables/useToast'
import { useErrorHandler } from '~/composables/useErrorHandler'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()
const { success, error } = useNotification()
const { handleError } = useErrorHandler()

// 字体设置
const fontSettings = ref({
  fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", "Microsoft YaHei", "PingFang SC", "Hiragino Sans GB", "Helvetica Neue", Arial, sans-serif',
  baseFontSize: 16,
  defaultFontWeight: '400',
  defaultLineHeight: 1.6,
  h1FontSize: 32,
  h2FontSize: 24,
  h3FontSize: 20,
  h4FontSize: 18,
  textMainColor: 'var(--color-text-main)',
  textMutedColor: 'var(--color-text-sec)'
})

const saving = ref(false)
const loading = ref(false)

// 默认字体设置
const defaultFontSettings = {
  fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", "Microsoft YaHei", "PingFang SC", "Hiragino Sans GB", "Helvetica Neue", Arial, sans-serif',
  baseFontSize: 16,
  defaultFontWeight: '400',
  defaultLineHeight: 1.6,
  h1FontSize: 32,
  h2FontSize: 24,
  h3FontSize: 20,
  h4FontSize: 18,
  textMainColor: 'var(--color-text-main)',
  textMutedColor: 'var(--color-text-sec)'
}

// 获取字体设置
const fetchFontSettings = async () => {
  loading.value = true
  try {
    // 从配置中获取字体设置
    const configs = await api.get<Record<string, string>>('/Config')
    
    if (configs) {
      // 解析字体设置
      if (configs.font_family) {
        fontSettings.value.fontFamily = configs.font_family
      }
      if (configs.base_font_size) {
        fontSettings.value.baseFontSize = parseInt(configs.base_font_size) || 16
      }
      if (configs.default_font_weight) {
        fontSettings.value.defaultFontWeight = configs.default_font_weight
      }
      if (configs.default_line_height) {
        fontSettings.value.defaultLineHeight = parseFloat(configs.default_line_height) || 1.6
      }
      if (configs.h1_font_size) {
        fontSettings.value.h1FontSize = parseInt(configs.h1_font_size) || 32
      }
      if (configs.h2_font_size) {
        fontSettings.value.h2FontSize = parseInt(configs.h2_font_size) || 24
      }
      if (configs.h3_font_size) {
        fontSettings.value.h3FontSize = parseInt(configs.h3_font_size) || 20
      }
      if (configs.h4_font_size) {
        fontSettings.value.h4FontSize = parseInt(configs.h4_font_size) || 18
      }
      if (configs.text_main_color) {
        fontSettings.value.textMainColor = configs.text_main_color
      } else {
        // 根据当前主题设置默认颜色
        const currentTheme = document.documentElement.dataset.theme || 'light'
        if (currentTheme === 'dark' || currentTheme === 'hybrid-super-dark') {
          fontSettings.value.textMainColor = 'rgba(255, 255, 255, 0.85)'
        }
      }
      if (configs.text_muted_color) {
        fontSettings.value.textMutedColor = configs.text_muted_color
      } else {
        // 根据当前主题设置默认颜色
        const currentTheme = document.documentElement.dataset.theme || 'light'
        if (currentTheme === 'dark' || currentTheme === 'hybrid-super-dark') {
          fontSettings.value.textMutedColor = 'rgba(255, 255, 255, 0.6)'
        }
      }
    }
  } catch (e) {
    handleError(e, '获取字体设置失败')
  } finally {
    loading.value = false
  }
}

  // 应用字体设置到页面
  const applyFontSettings = () => {
    if (!process.client) return
    
    const root = document.documentElement
    
    // 应用字体族
    root.style.setProperty('--font-family-base', fontSettings.value.fontFamily)
    
    // 应用基础字号
    root.style.setProperty('--font-size-base', `${fontSettings.value.baseFontSize}px`)
    
    // 应用默认字重
    root.style.setProperty('--font-weight-base', fontSettings.value.defaultFontWeight)
    
    // 应用默认行高
    root.style.setProperty('--line-height-base', String(fontSettings.value.defaultLineHeight))
    
    // 应用标题字号
    root.style.setProperty('--font-size-h1', `${fontSettings.value.h1FontSize}px`)
    root.style.setProperty('--font-size-h2', `${fontSettings.value.h2FontSize}px`)
    root.style.setProperty('--font-size-h3', `${fontSettings.value.h3FontSize}px`)
    root.style.setProperty('--font-size-h4', `${fontSettings.value.h4FontSize}px`)
    
    // 应用字体颜色
    root.style.setProperty('--color-text-main', fontSettings.value.textMainColor)
    root.style.setProperty('--color-text-muted', fontSettings.value.textMutedColor)
    
    // 应用到 body
    document.body.style.fontFamily = fontSettings.value.fontFamily
    document.body.style.fontSize = `${fontSettings.value.baseFontSize}px`
    document.body.style.fontWeight = fontSettings.value.defaultFontWeight
    document.body.style.lineHeight = String(fontSettings.value.defaultLineHeight)
    document.body.style.color = fontSettings.value.textMainColor
  }

// 保存字体设置
const saveFontSettings = async () => {
  saving.value = true
  try {
    // 保存到配置中
    await Promise.all([
      api.put('/Config/font_family', { value: fontSettings.value.fontFamily }),
      api.put('/Config/base_font_size', { value: String(fontSettings.value.baseFontSize) }),
      api.put('/Config/default_font_weight', { value: fontSettings.value.defaultFontWeight }),
      api.put('/Config/default_line_height', { value: String(fontSettings.value.defaultLineHeight) }),
      api.put('/Config/h1_font_size', { value: String(fontSettings.value.h1FontSize) }),
      api.put('/Config/h2_font_size', { value: String(fontSettings.value.h2FontSize) }),
      api.put('/Config/h3_font_size', { value: String(fontSettings.value.h3FontSize) }),
      api.put('/Config/h4_font_size', { value: String(fontSettings.value.h4FontSize) }),
      api.put('/Config/text_main_color', { value: fontSettings.value.textMainColor }),
      api.put('/Config/text_muted_color', { value: fontSettings.value.textMutedColor })
    ])

    // 立即应用字体设置
    applyFontSettings()
    
    success('字体设置保存成功，已立即应用到当前页面')
  } catch (e) {
    handleError(e, '保存字体设置失败')
  } finally {
    saving.value = false
  }
}

// 恢复默认字体族
const resetFontFamily = () => {
  fontSettings.value.fontFamily = defaultFontSettings.fontFamily
  success('已恢复默认字体族')
}

// 恢复所有默认设置
const resetToDefaults = () => {
  if (confirm('确定要恢复所有默认字体设置吗？')) {
    fontSettings.value = { ...defaultFontSettings }
    success('已恢复默认设置')
  }
}

// 预览字体设置
const previewFontSettings = () => {
  // 创建预览窗口
  const previewWindow = window.open('', '_blank', 'width=800,height=600')
  if (previewWindow) {
    previewWindow.document.write(`
      <!DOCTYPE html>
      <html>
        <head>
          <title>字体设置预览</title>
          <style>
            body {
              font-family: ${fontSettings.value.fontFamily};
              font-size: ${fontSettings.value.baseFontSize}px;
              font-weight: ${fontSettings.value.defaultFontWeight};
              line-height: ${fontSettings.value.defaultLineHeight};
              padding: 2rem;
              background: var(--color-gray-900);
              color: rgba(255, 255, 255, 0.85);
            }
            h1 { font-size: ${fontSettings.value.h1FontSize}px; margin: 1rem 0; }
            h2 { font-size: ${fontSettings.value.h2FontSize}px; margin: 1rem 0; }
            h3 { font-size: ${fontSettings.value.h3FontSize}px; margin: 1rem 0; }
            h4 { font-size: ${fontSettings.value.h4FontSize}px; margin: 1rem 0; }
            p { margin: 1rem 0; }
          </style>
        </head>
        <body>
          <h1>H1 标题：溪午听风 - 数字花园</h1>
          <h2>H2 标题：AI 创作空间</h2>
          <h3>H3 标题：技术博客</h3>
          <h4>H4 标题：项目展示</h4>
          <p>这是正文内容。我是一名软件与算法开发者，正在打造一个能持续增长的个人数字资产平台。这里记录我的作品、思考、实验项目和未来计划。</p>
          <p>同时，我正探索 AI 技术如何助力个人成长、创作与商业化。通过整合内容、项目、工具和 AI 实验室，构建一个围绕个人 IP 的创作操作系统。</p>
        </body>
      </html>
    `)
    previewWindow.document.close()
  }
}

// 监听字体设置变化，实时预览
watch(() => fontSettings.value, () => {
  if (process.client) {
    applyFontSettings()
  }
}, { deep: true })

onMounted(() => {
  fetchFontSettings().then(() => {
    // 加载后立即应用
    applyFontSettings()
  })
})
</script>

<style scoped>
.font-management-page {
  width: 100%;
  padding: var(--spacing-md);
}

.page-header {
  margin-bottom: var(--spacing-2xl);
}

.page-title {
  font-size: var(--text-2xl);
  font-weight: 700;
  color: var(--color-text-main, var(--color-bg-card));
  margin-bottom: var(--spacing-sm);
}

.page-description {
  color: var(--color-text-muted, rgba(255, 255, 255, 0.6));
  font-size: var(--text-sm);
}

.font-settings-container {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-2xl);
}

.settings-section {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
}

.section-header {
  margin-bottom: var(--spacing-xl);
}

.section-title {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-text-main, var(--color-bg-card));
  margin-bottom: var(--spacing-sm);
  display: flex;
  align-items: center;
}

.section-description {
  color: var(--color-text-muted, rgba(255, 255, 255, 0.6));
  font-size: var(--text-sm);
}

.settings-form {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
}

.form-label {
  color: var(--color-text-main, var(--color-bg-card));
  font-size: var(--text-sm);
  font-weight: 500;
  display: flex;
  align-items: center;
}

.form-hint {
  color: var(--color-text-muted, rgba(255, 255, 255, 0.5));
  font-size: var(--text-xs);
  margin-top: calc(var(--spacing-sm) * -0.5);
}

.form-input {
  padding: var(--spacing-md);
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: var(--radius-md);
  color: var(--color-text-main, var(--color-bg-card));
  font-size: var(--text-sm);
  transition: all 0.2s;
}

.form-input:focus {
  outline: none;
  border-color: var(--color-primary, var(--color-primary));
  background: rgba(255, 255, 255, 0.15);
}

.form-input::placeholder {
  color: rgba(255, 255, 255, 0.4);
}

.form-row {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--spacing-xl);
}

@media (max-width: 768px) {
  .form-row {
    grid-template-columns: 1fr;
  }
}

.font-family-input-group {
  display: flex;
  gap: var(--spacing-sm);
}

.font-family-input-group .form-input {
  flex: 1;
}

.font-size-input-group {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.font-size-input-group .form-input {
  flex: 1;
}

.input-unit {
  color: var(--color-text-muted, rgba(255, 255, 255, 0.6));
  font-size: var(--text-sm);
  min-width: var(--spacing-2xl);
}

.font-preview {
  margin-top: var(--spacing-md);
  padding: var(--spacing-lg);
  background: rgba(0, 0, 0, 0.3);
  border-radius: var(--radius-md);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.font-preview-small {
  margin-top: var(--spacing-sm);
  padding: var(--spacing-md);
  background: rgba(0, 0, 0, 0.3);
  border-radius: var(--radius-md);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.preview-label {
  color: var(--color-text-muted, rgba(255, 255, 255, 0.6));
  font-size: var(--text-xs);
  margin-bottom: var(--spacing-sm);
}

.preview-text {
  color: var(--color-text-main, rgba(255, 255, 255, 0.85));
  margin: 0;
}

.font-preview-small h1,
.font-preview-small h2,
.font-preview-small h3,
.font-preview-small h4 {
  margin: 0;
  color: var(--color-text-main, rgba(255, 255, 255, 0.85));
}

.actions-section {
  display: flex;
  gap: var(--spacing-md);
  padding-top: var(--spacing-md);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.btn-primary,
.btn-secondary {
  padding: var(--spacing-md) var(--spacing-xl);
  border-radius: var(--radius-md);
  font-size: var(--text-sm);
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
  display: inline-flex;
  align-items: center;
}

.btn-primary {
  background: var(--color-primary, var(--color-primary));
  color: var(--color-bg-card);
}

.btn-primary:hover:not(:disabled) {
  background: var(--color-primary-hover, var(--color-primary-hover));
  transform: translateY(-1px);
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-secondary {
  background: rgba(255, 255, 255, 0.1);
  color: var(--color-text-main, var(--color-bg-card));
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.btn-secondary:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 255, 255, 0.3);
}

.btn-secondary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>

