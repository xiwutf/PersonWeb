<template>
  <div class="settings-page">
    <PageHeader
      title="系统设置"
      description="统一管理站点基础配置、后台风格、模块开关与日常维护入口。"
      class="settings-header"
    >
      <template #actions>
        <div class="settings-header-badge">
          <span class="settings-header-badge__label">控制台</span>
          <strong>{{ totalItems }} 个配置入口</strong>
        </div>
      </template>
    </PageHeader>

    <section class="settings-hero">
      <div class="settings-hero__copy">
        <p class="settings-hero__eyebrow">System Control Center</p>
        <h2>把常用配置集中到一处，减少来回切页的负担</h2>
        <p>
          这里保留后台最常用的管理入口，按“基础配置 / 风格系统 / 系统维护 / 互动审核”分区整理，
          方便快速定位。
        </p>
      </div>
      <div class="settings-hero__stats">
        <div class="hero-stat-card">
          <span>基础配置</span>
          <strong>{{ sections[0]?.items.length || 0 }}</strong>
        </div>
        <div class="hero-stat-card">
          <span>风格与主题</span>
          <strong>{{ sections[1]?.items.length || 0 }}</strong>
        </div>
        <div class="hero-stat-card">
          <span>维护与审核</span>
          <strong>{{ maintenanceEntryCount }}</strong>
        </div>
      </div>
    </section>

    <div class="settings-grid">
      <section
        v-for="section in sections"
        :key="section.key"
        class="settings-section"
      >
        <div class="settings-section__header">
          <div class="settings-section__meta">
            <span class="settings-section__icon">
              <i :class="section.icon"></i>
            </span>
            <div>
              <h2 class="settings-section__title">{{ section.title }}</h2>
              <p class="settings-section__desc">{{ section.description }}</p>
            </div>
          </div>
          <span class="settings-section__count">{{ section.items.length }} 项</span>
        </div>

        <div class="settings-cards">
          <NuxtLink
            v-for="item in section.items"
            :key="item.to"
            :to="item.to"
            class="settings-card"
          >
            <div class="settings-card__icon" :class="item.tone">
              <i :class="item.icon"></i>
            </div>
            <div class="settings-card__content">
              <div class="settings-card__top">
                <h3 class="settings-card__title">{{ item.title }}</h3>
                <span v-if="item.tag" class="settings-card__tag">{{ item.tag }}</span>
              </div>
              <p class="settings-card__desc">{{ item.description }}</p>
              <div class="settings-card__footer">
                <span class="settings-card__hint">{{ item.hint }}</span>
                <i class="fas fa-arrow-right settings-card__arrow"></i>
              </div>
            </div>
          </NuxtLink>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import PageHeader from '~/components/admin/patterns/PageHeader.vue'

definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const sections = [
  {
    key: 'base',
    title: '基础设置',
    description: '站点运行、账户权限与基础资料配置。',
    icon: 'fas fa-cog',
    items: [
      {
        to: '/admin/config',
        title: '站点配置',
        description: '管理网站的名称、描述、基础信息与公共配置。',
        icon: 'fas fa-cog',
        tone: 'tone-blue',
        tag: 'Core',
        hint: '基础配置'
      },
      {
        to: '/admin/users',
        title: '用户管理',
        description: '查看后台用户、权限分配和管理范围。',
        icon: 'fas fa-users',
        tone: 'tone-cyan',
        hint: '权限与账户'
      },
      {
        to: '/admin/settings/change-password',
        title: '修改密码',
        description: '修改当前管理员账号的登录密码与安全信息。',
        icon: 'fas fa-key',
        tone: 'tone-amber',
        hint: '账号安全'
      },
      {
        to: '/admin/friend-links',
        title: '友情链接',
        description: '统一管理站外友情链接和展示顺序。',
        icon: 'fas fa-link',
        tone: 'tone-indigo',
        hint: '外链维护'
      }
    ]
  },
  {
    key: 'style',
    title: '风格设置',
    description: '站点主题、首页视觉与后台风格总入口。',
    icon: 'fas fa-palette',
    items: [
      {
        to: '/admin/home-styles',
        title: '首页风格管理',
        description: '调整首页视觉结构、主题与首屏呈现。',
        icon: 'fas fa-home',
        tone: 'tone-pink',
        tag: 'Popular',
        hint: '首页体验'
      },
      {
        to: '/admin/admin-styles',
        title: '后台风格管理',
        description: '统一后台布局、色板和组件视觉层级。',
        icon: 'fas fa-paint-brush',
        tone: 'tone-purple',
        hint: '后台视觉'
      },
      {
        to: '/admin/settings/styles',
        title: '样式管理',
        description: '集中维护系统样式变量与全局视觉配置。',
        icon: 'fas fa-swatchbook',
        tone: 'tone-blue',
        hint: '全局样式'
      },
      {
        to: '/admin/settings/themes',
        title: '主题风格管理',
        description: '管理主题方案与动态背景等主题能力。',
        icon: 'fas fa-palette',
        tone: 'tone-emerald',
        hint: '主题系统'
      },
      {
        to: '/admin/settings/modules',
        title: '模块管理',
        description: '按需启用或停用模块，控制站点能力范围。',
        icon: 'fas fa-puzzle-piece',
        tone: 'tone-cyan',
        hint: '模块开关'
      },
      {
        to: '/admin/settings/fonts',
        title: '字体管理',
        description: '维护字体族、字号、字重与排版层级。',
        icon: 'fas fa-font',
        tone: 'tone-amber',
        hint: '排版系统'
      }
    ]
  },
  {
    key: 'maintenance',
    title: '系统维护',
    description: '异常排查、日志检查与日常维护入口。',
    icon: 'fas fa-screwdriver-wrench',
    items: [
      {
        to: '/admin/error-logs',
        title: '错误日志',
        description: '查看系统异常、报错记录与维护日志。',
        icon: 'fas fa-exclamation-triangle',
        tone: 'tone-red',
        tag: 'Alert',
        hint: '问题排查'
      }
    ]
  },
  {
    key: 'visitor',
    title: '访客互动',
    description: '审核访客反馈与互动内容，维持前台体验。',
    icon: 'fas fa-comments',
    items: [
      {
        to: '/admin/visitor-messages',
        title: '访客留言管理',
        description: '审核弹幕、留言、心情和祝福等访客互动内容。',
        icon: 'fas fa-comments',
        tone: 'tone-indigo',
        hint: '内容审核'
      }
    ]
  }
] as const

const totalItems = computed(() => sections.reduce((sum, section) => sum + section.items.length, 0))
const maintenanceEntryCount = computed(() => (sections[2]?.items.length || 0) + (sections[3]?.items.length || 0))
</script>

<style scoped>
.settings-page {
  width: 100%;
}

.settings-header {
  margin-bottom: var(--spacing-lg);
}

.settings-header-badge {
  min-width: 160px;
  padding: 14px 18px;
  border-radius: var(--radius-xl);
  background: linear-gradient(135deg, rgba(37, 99, 235, 0.16), rgba(14, 165, 233, 0.08));
  border: 1px solid rgba(96, 165, 250, 0.22);
  text-align: right;
}

.settings-header-badge__label {
  display: block;
  font-size: 0.78rem;
  color: var(--color-text-muted);
}

.settings-header-badge strong {
  display: block;
  margin-top: 4px;
  color: var(--color-text-main);
  font-size: 1rem;
}

.settings-hero {
  display: grid;
  grid-template-columns: minmax(0, 1.4fr) minmax(300px, 0.8fr);
  gap: var(--spacing-lg);
  margin-bottom: var(--spacing-xl);
  padding: 28px 30px;
  border-radius: var(--radius-xl);
  border: 1px solid rgba(96, 165, 250, 0.18);
  background:
    radial-gradient(circle at top right, rgba(37, 99, 235, 0.18), transparent 28%),
    linear-gradient(180deg, rgba(15, 23, 42, 0.96), rgba(17, 24, 39, 0.94));
}

.settings-hero__eyebrow {
  margin: 0 0 10px;
  font-size: 0.78rem;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: #93c5fd;
}

.settings-hero h2 {
  margin: 0 0 10px;
  font-size: clamp(1.4rem, 2vw, 1.9rem);
  line-height: 1.3;
  color: var(--color-text-main);
}

.settings-hero p {
  margin: 0;
  color: var(--color-text-muted);
  line-height: 1.7;
}

.settings-hero__stats {
  display: grid;
  gap: 12px;
}

.hero-stat-card {
  padding: 18px;
  border-radius: var(--radius-lg);
  border: 1px solid rgba(148, 163, 184, 0.14);
  background: rgba(255, 255, 255, 0.03);
}

.hero-stat-card span {
  display: block;
  color: var(--color-text-muted);
  font-size: 0.84rem;
}

.hero-stat-card strong {
  display: block;
  margin-top: 6px;
  color: var(--color-text-main);
  font-size: 1.7rem;
  line-height: 1;
}

.settings-grid {
  display: grid;
  gap: var(--spacing-xl);
}

.settings-section {
  padding: 24px;
  border-radius: var(--radius-xl);
  border: 1px solid var(--color-border-subtle);
  background:
    linear-gradient(180deg, rgba(20, 27, 45, 0.96), rgba(15, 23, 42, 0.92)),
    var(--color-bg-card);
}

.settings-section__header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: var(--spacing-md);
  margin-bottom: var(--spacing-lg);
}

.settings-section__meta {
  display: flex;
  align-items: flex-start;
  gap: 14px;
}

.settings-section__icon {
  width: 42px;
  height: 42px;
  border-radius: 14px;
  display: grid;
  place-items: center;
  background: rgba(59, 130, 246, 0.14);
  color: #60a5fa;
  flex-shrink: 0;
}

.settings-section__title {
  margin: 0;
  color: var(--color-text-main);
  font-size: 1.15rem;
  line-height: 1.3;
}

.settings-section__desc {
  margin: 6px 0 0;
  color: var(--color-text-muted);
  font-size: 0.92rem;
}

.settings-section__count {
  padding: 6px 12px;
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(148, 163, 184, 0.14);
  color: var(--color-text-muted);
  font-size: 0.8rem;
}

.settings-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(270px, 1fr));
  gap: var(--spacing-md);
}

.settings-card {
  display: flex;
  gap: 14px;
  min-height: 152px;
  padding: 18px;
  border-radius: var(--radius-lg);
  border: 1px solid rgba(148, 163, 184, 0.14);
  background: rgba(255, 255, 255, 0.025);
  text-decoration: none;
  color: inherit;
  transition: transform 0.2s ease, border-color 0.2s ease, background 0.2s ease;
}

.settings-card:hover {
  transform: translateY(-2px);
  border-color: rgba(96, 165, 250, 0.3);
  background: rgba(59, 130, 246, 0.08);
}

.settings-card__icon {
  width: 48px;
  height: 48px;
  border-radius: 16px;
  display: grid;
  place-items: center;
  color: white;
  font-size: 1.05rem;
  flex-shrink: 0;
}

.tone-blue {
  background: linear-gradient(135deg, #2563eb, #3b82f6);
}

.tone-cyan {
  background: linear-gradient(135deg, #0891b2, #06b6d4);
}

.tone-amber {
  background: linear-gradient(135deg, #d97706, #f59e0b);
}

.tone-indigo {
  background: linear-gradient(135deg, #4f46e5, #6366f1);
}

.tone-pink {
  background: linear-gradient(135deg, #db2777, #f472b6);
}

.tone-purple {
  background: linear-gradient(135deg, #7c3aed, #a855f7);
}

.tone-emerald {
  background: linear-gradient(135deg, #059669, #22c55e);
}

.tone-red {
  background: linear-gradient(135deg, #dc2626, #f97316);
}

.settings-card__content {
  display: flex;
  flex: 1;
  min-width: 0;
  flex-direction: column;
}

.settings-card__top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: var(--spacing-sm);
}

.settings-card__title {
  margin: 0;
  color: var(--color-text-main);
  font-size: 1rem;
  line-height: 1.35;
}

.settings-card__tag {
  padding: 4px 8px;
  border-radius: 999px;
  background: rgba(96, 165, 250, 0.12);
  color: #93c5fd;
  font-size: 0.74rem;
  white-space: nowrap;
}

.settings-card__desc {
  margin: 10px 0 0;
  color: var(--color-text-muted);
  line-height: 1.6;
  font-size: 0.9rem;
}

.settings-card__footer {
  margin-top: auto;
  padding-top: 16px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-sm);
}

.settings-card__hint {
  color: #93c5fd;
  font-size: 0.8rem;
}

.settings-card__arrow {
  color: var(--color-text-muted);
  transition: transform 0.2s ease, color 0.2s ease;
}

.settings-card:hover .settings-card__arrow {
  color: var(--color-text-main);
  transform: translateX(4px);
}

@media (max-width: 960px) {
  .settings-hero {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .settings-section {
    padding: 18px;
  }

  .settings-section__header,
  .settings-card__top {
    flex-direction: column;
  }

  .settings-card {
    min-height: auto;
  }
}
</style>
