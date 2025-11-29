<template>
  <div class="frontend-styles-page">
    <div class="page-header">
      <h1 class="page-title">前端页面样式管理</h1>
      <p class="page-description">统一管理前端页面的样式配置，支持动态修改颜色、字体、间距等样式属性</p>
    </div>

    <!-- 页面选择标签页 -->
    <div class="tabs-container">
      <div class="tabs">
        <button
          v-for="page in pages"
          :key="page.key"
          @click="selectPage(page.key)"
          :class="['tab-button', { 'tab-button-active': activePageKey === page.key }]"
        >
          <span class="tab-icon">{{ page.icon }}</span>
          {{ page.name }}
        </button>
      </div>
    </div>

    <!-- 样式配置区域 -->
    <div v-if="activePageKey" class="styles-config-section">
      <!-- 样式配置卡片 -->
      <div class="config-card">
        <div class="card-header">
          <h2 class="card-title">基础样式配置</h2>
          <button @click="saveStyleConfig" class="btn-primary" :disabled="saving">
            <i class="fas fa-save mr-2"></i>
            {{ saving ? '保存中...' : '保存配置' }}
          </button>
        </div>

        <div class="card-body">
          <div class="config-grid">
            <!-- 主色调 -->
            <div class="config-item">
              <label class="config-label">
                <span class="label-text">主色调</span>
                <input
                  v-model="styleConfig.primaryColor"
                  type="color"
                  class="color-input"
                />
              </label>
              <input
                v-model="styleConfig.primaryColor"
                type="text"
                class="config-input"
                placeholder="#f97316"
              />
            </div>

            <!-- 次色调 -->
            <div class="config-item">
              <label class="config-label">
                <span class="label-text">次色调</span>
                <input
                  v-model="styleConfig.secondaryColor"
                  type="color"
                  class="color-input"
                />
              </label>
              <input
                v-model="styleConfig.secondaryColor"
                type="text"
                class="config-input"
                placeholder="#dc2626"
              />
            </div>

            <!-- 背景色 -->
            <div class="config-item">
              <label class="config-label">
                <span class="label-text">背景色</span>
                <input
                  v-model="styleConfig.backgroundColor"
                  type="color"
                  class="color-input"
                />
              </label>
              <input
                v-model="styleConfig.backgroundColor"
                type="text"
                class="config-input"
                placeholder="#0f172a"
              />
            </div>

            <!-- 文字颜色 -->
            <div class="config-item">
              <label class="config-label">
                <span class="label-text">文字颜色</span>
                <input
                  v-model="styleConfig.textColor"
                  type="color"
                  class="color-input"
                />
              </label>
              <input
                v-model="styleConfig.textColor"
                type="text"
                class="config-input"
                placeholder="#e2e8f0"
              />
            </div>

            <!-- 卡片背景 -->
            <div class="config-item">
              <label class="config-label">
                <span class="label-text">卡片背景</span>
              </label>
              <input
                v-model="styleConfig.cardBackground"
                type="text"
                class="config-input"
                placeholder="rgba(30, 41, 59, 0.3)"
              />
            </div>

            <!-- 边框颜色 -->
            <div class="config-item">
              <label class="config-label">
                <span class="label-text">边框颜色</span>
                <input
                  v-model="styleConfig.borderColor"
                  type="color"
                  class="color-input"
                />
              </label>
              <input
                v-model="styleConfig.borderColor"
                type="text"
                class="config-input"
                placeholder="rgba(255, 255, 255, 0.05)"
              />
            </div>

            <!-- 圆角 -->
            <div class="config-item">
              <label class="config-label">
                <span class="label-text">圆角大小</span>
              </label>
              <input
                v-model="styleConfig.borderRadius"
                type="text"
                class="config-input"
                placeholder="1.5rem"
              />
            </div>

            <!-- 字体 -->
            <div class="config-item">
              <label class="config-label">
                <span class="label-text">字体族</span>
              </label>
              <input
                v-model="styleConfig.fontFamily"
                type="text"
                class="config-input"
                placeholder='"Outfit", sans-serif'
              />
            </div>
          </div>
        </div>
      </div>

      <!-- 样式变量管理 -->
      <div class="config-card">
        <div class="card-header">
          <h2 class="card-title">样式变量</h2>
          <button @click="showVariableModal = true" class="btn-secondary">
            <i class="fas fa-plus mr-2"></i>
            新增变量
          </button>
        </div>

        <div class="card-body">
          <div v-if="styleVariables.length === 0" class="empty-state">
            暂无样式变量
          </div>
          <div v-else class="variables-list">
            <div
              v-for="variable in styleVariables"
              :key="variable.id"
              class="variable-item"
            >
              <div class="variable-info">
                <code class="variable-key">{{ variable.variableKey }}</code>
                <span class="variable-value">{{ variable.variableValue }}</span>
                <span class="variable-type">{{ variable.variableType }}</span>
              </div>
              <div class="variable-actions">
                <button @click="editVariable(variable)" class="btn-link">编辑</button>
                <button @click="deleteVariable(variable.id)" class="btn-link btn-link-red">删除</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 样式规则管理 -->
      <div class="config-card">
        <div class="card-header">
          <h2 class="card-title">自定义样式规则</h2>
          <button @click="showRuleModal = true" class="btn-secondary">
            <i class="fas fa-plus mr-2"></i>
            新增规则
          </button>
        </div>

        <div class="card-body">
          <div v-if="styleRules.length === 0" class="empty-state">
            暂无样式规则
          </div>
          <div v-else class="rules-list">
            <div
              v-for="rule in styleRules"
              :key="rule.id"
              class="rule-item"
            >
              <div class="rule-info">
                <code class="rule-selector">{{ rule.selector }}</code>
                <div class="rule-properties">
                  <span
                    v-for="(value, key) in rule.cssProperties"
                    :key="key"
                    class="property-item"
                  >
                    <strong>{{ key }}:</strong> {{ value }}
                  </span>
                </div>
                <span class="rule-priority">优先级: {{ rule.priority }}</span>
              </div>
              <div class="rule-actions">
                <button @click="editRule(rule)" class="btn-link">编辑</button>
                <button @click="deleteRule(rule.id)" class="btn-link btn-link-red">删除</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 实时预览 -->
      <div class="config-card">
        <div class="card-header">
          <h2 class="card-title">实时预览</h2>
        </div>
        <div class="card-body">
          <div class="preview-container" :style="previewStyles">
            <div class="preview-card">
              <h3 class="preview-title">示例卡片</h3>
              <p class="preview-text">这是样式预览效果</p>
              <button class="preview-button">示例按钮</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 变量编辑模态框 -->
    <div v-if="showVariableModal" class="modal-overlay" @click="showVariableModal = false">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>{{ editingVariable ? '编辑变量' : '新增变量' }}</h2>
          <button @click="closeVariableModal" class="modal-close">×</button>
        </div>
        <form @submit.prevent="saveVariable" class="modal-body">
          <div class="form-group">
            <label class="form-label">变量键名 *</label>
            <input v-model="variableForm.variableKey" type="text" class="form-input" required />
          </div>
          <div class="form-group">
            <label class="form-label">变量值 *</label>
            <input v-model="variableForm.variableValue" type="text" class="form-input" required />
          </div>
          <div class="form-group">
            <label class="form-label">变量类型</label>
            <select v-model="variableForm.variableType" class="form-input">
              <option value="color">颜色</option>
              <option value="size">尺寸</option>
              <option value="spacing">间距</option>
              <option value="font">字体</option>
              <option value="other">其他</option>
            </select>
          </div>
          <div class="form-group">
            <label class="form-label">描述</label>
            <textarea v-model="variableForm.description" class="form-input" rows="2"></textarea>
          </div>
          <div class="modal-footer">
            <button type="button" @click="closeVariableModal" class="btn-secondary">取消</button>
            <button type="submit" class="btn-primary">保存</button>
          </div>
        </form>
      </div>
    </div>

    <!-- 规则编辑模态框 -->
    <div v-if="showRuleModal" class="modal-overlay" @click="showRuleModal = false">
      <div class="modal-content modal-content-large" @click.stop>
        <div class="modal-header">
          <h2>{{ editingRule ? '编辑规则' : '新增规则' }}</h2>
          <button @click="closeRuleModal" class="modal-close">×</button>
        </div>
        <form @submit.prevent="saveRule" class="modal-body">
          <div class="form-group">
            <label class="form-label">CSS 选择器 *</label>
            <input v-model="ruleForm.selector" type="text" class="form-input" placeholder=".tools-card" required />
          </div>
          <div class="form-group">
            <label class="form-label">CSS 属性 (JSON格式) *</label>
            <textarea
              v-model="ruleForm.cssPropertiesJson"
              class="form-input"
              rows="6"
              placeholder='{"color": "#fff", "font-size": "1.5rem"}'
              required
            ></textarea>
          </div>
          <div class="form-group">
            <label class="form-label">优先级</label>
            <input v-model.number="ruleForm.priority" type="number" class="form-input" />
          </div>
          <div class="form-group">
            <label class="form-label">
              <input v-model="ruleForm.enabled" type="checkbox" />
              启用
            </label>
          </div>
          <div class="form-group">
            <label class="form-label">描述</label>
            <textarea v-model="ruleForm.description" class="form-input" rows="2"></textarea>
          </div>
          <div class="modal-footer">
            <button type="button" @click="closeRuleModal" class="btn-secondary">取消</button>
            <button type="submit" class="btn-primary">保存</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const api = useApi()

interface Page {
  key: string
  name: string
  icon: string
}

interface StyleConfig {
  primaryColor: string
  secondaryColor: string
  backgroundColor: string
  textColor: string
  cardBackground: string
  borderColor: string
  borderRadius: string
  fontFamily: string
}

interface StyleVariable {
  id: number
  pageKey: string
  variableKey: string
  variableValue: string
  variableType: string
  description?: string
}

interface StyleRule {
  id: number
  pageKey: string
  selector: string
  cssProperties: Record<string, string>
  priority: number
  enabled: boolean
  description?: string
}

const pages: Page[] = [
  { key: 'tools', name: '工具页面', icon: '🔧' },
  { key: 'blog', name: '博客页面', icon: '📝' },
  { key: 'life', name: '生活随笔', icon: '☕' },
  { key: 'projects', name: '项目页面', icon: '📦' },
  { key: 'about', name: '关于页面', icon: '👤' }
]

const activePageKey = ref<string>('tools')
const styleConfig = ref<StyleConfig>({
  primaryColor: '#f97316',
  secondaryColor: '#dc2626',
  backgroundColor: '#0f172a',
  textColor: '#e2e8f0',
  cardBackground: 'rgba(30, 41, 59, 0.3)',
  borderColor: 'rgba(255, 255, 255, 0.05)',
  borderRadius: '1.5rem',
  fontFamily: '"Outfit", sans-serif'
})
const styleVariables = ref<StyleVariable[]>([])
const styleRules = ref<StyleRule[]>([])
const loading = ref(false)
const saving = ref(false)

const showVariableModal = ref(false)
const showRuleModal = ref(false)
const editingVariable = ref<StyleVariable | null>(null)
const editingRule = ref<StyleRule | null>(null)

const variableForm = ref({
  variableKey: '',
  variableValue: '',
  variableType: 'color',
  description: ''
})

const ruleForm = ref({
  selector: '',
  cssPropertiesJson: '{}',
  priority: 0,
  enabled: true,
  description: ''
})

// 预览样式
const previewStyles = computed(() => ({
  backgroundColor: styleConfig.value.backgroundColor,
  color: styleConfig.value.textColor,
  fontFamily: styleConfig.value.fontFamily,
  padding: '2rem'
}))

// 选择页面
const selectPage = async (pageKey: string) => {
  activePageKey.value = pageKey
  await fetchPageStyle()
}

// 获取页面样式配置
const fetchPageStyle = async () => {
  try {
    loading.value = true

    // 获取样式配置
    const configRes = await api.get<{
      pageKey: string
      pageName: string
      styleConfig: StyleConfig
      enabled: boolean
      version: number
    }>(`/frontend-styles/page/${activePageKey.value}`)

    if (configRes && configRes.styleConfig) {
      styleConfig.value = configRes.styleConfig
    }

    // 获取样式变量
    const variablesRes = await api.get<StyleVariable[]>(
      `/frontend-styles/variables/${activePageKey.value}`
    )
    if (variablesRes && Array.isArray(variablesRes)) {
      styleVariables.value = variablesRes
    }

    // 获取样式规则
    const rulesRes = await api.get<StyleRule[]>(
      `/frontend-styles/rules/${activePageKey.value}`
    )
    if (rulesRes && Array.isArray(rulesRes)) {
      styleRules.value = rulesRes
    }
  } catch (e: any) {
    console.error('Failed to fetch page style:', e)
  } finally {
    loading.value = false
  }
}

// 保存样式配置
const saveStyleConfig = async () => {
  try {
    saving.value = true
    await api.put(`/frontend-styles/page/${activePageKey.value}`, {
      pageName: pages.find(p => p.key === activePageKey.value)?.name,
      styleConfig: styleConfig.value
    })
    alert('保存成功！')
  } catch (e: any) {
    console.error('Failed to save style config:', e)
    alert('保存失败：' + e.message)
  } finally {
    saving.value = false
  }
}

// 变量相关操作
const editVariable = (variable: StyleVariable) => {
  editingVariable.value = variable
  variableForm.value = {
    variableKey: variable.variableKey,
    variableValue: variable.variableValue,
    variableType: variable.variableType,
    description: variable.description || ''
  }
  showVariableModal.value = true
}

const closeVariableModal = () => {
  showVariableModal.value = false
  editingVariable.value = null
  variableForm.value = {
    variableKey: '',
    variableValue: '',
    variableType: 'color',
    description: ''
  }
}

const saveVariable = async () => {
  try {
    await api.put(
      `/frontend-styles/variables/${activePageKey.value}/${variableForm.value.variableKey}`,
      variableForm.value
    )
    await fetchPageStyle()
    closeVariableModal()
  } catch (e: any) {
    console.error('Failed to save variable:', e)
    alert('保存失败：' + e.message)
  }
}

const deleteVariable = async (id: number) => {
  if (!confirm('确定要删除这个变量吗？')) return
  // 需要添加删除接口
  await fetchPageStyle()
}

// 规则相关操作
const editRule = (rule: StyleRule) => {
  editingRule.value = rule
  ruleForm.value = {
    selector: rule.selector,
    cssPropertiesJson: JSON.stringify(rule.cssProperties, null, 2),
    priority: rule.priority,
    enabled: rule.enabled,
    description: rule.description || ''
  }
  showRuleModal.value = true
}

const closeRuleModal = () => {
  showRuleModal.value = false
  editingRule.value = null
  ruleForm.value = {
    selector: '',
    cssPropertiesJson: '{}',
    priority: 0,
    enabled: true,
    description: ''
  }
}

const saveRule = async () => {
  try {
    const cssProperties = JSON.parse(ruleForm.value.cssPropertiesJson)
    await api.put(`/frontend-styles/rules/${activePageKey.value}`, {
      id: editingRule.value?.id,
      selector: ruleForm.value.selector,
      cssProperties,
      priority: ruleForm.value.priority,
      enabled: ruleForm.value.enabled,
      description: ruleForm.value.description
    })
    await fetchPageStyle()
    closeRuleModal()
  } catch (e: any) {
    console.error('Failed to save rule:', e)
    alert('保存失败：' + e.message)
  }
}

const deleteRule = async (id: number) => {
  if (!confirm('确定要删除这个规则吗？')) return
  try {
    await api.delete(`/frontend-styles/rules/${id}`)
    await fetchPageStyle()
  } catch (e: any) {
    console.error('Failed to delete rule:', e)
    alert('删除失败：' + e.message)
  }
}

// 初始化
onMounted(() => {
  fetchPageStyle()
})
</script>

<style scoped>
/* 页面样式已使用全局 admin 样式 */
</style>

