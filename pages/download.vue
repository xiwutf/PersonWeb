<template>
  <div class="download-page">
    <div class="download-page-container">

      <!-- Header -->
      <div class="download-page-header">
        <p class="download-page-kicker">Download</p>
        <h1 class="download-page-title">下载桌面陪伴助手</h1>
        <p class="download-page-subtitle">支持 Windows，安装后即可使用。</p>
      </div>

      <!-- Download Card -->
      <div class="download-info-card">
        <div class="download-info-meta">
          <div class="download-info-row">
            <span class="download-info-label">当前版本</span>
            <span class="download-info-value download-info-version">{{ download.version }}</span>
          </div>
          <div class="download-info-row">
            <span class="download-info-label">支持平台</span>
            <span class="download-info-value">
              <i class="fab fa-windows"></i>
              {{ download.platform }}
            </span>
          </div>
          <div class="download-info-row">
            <span class="download-info-label">发布时间</span>
            <span class="download-info-value">{{ download.releaseDate }}</span>
          </div>
          <div class="download-info-row">
            <span class="download-info-label">安装包大小</span>
            <span class="download-info-value">{{ download.fileSize }}</span>
          </div>
        </div>

        <div class="download-btn-wrap">
          <a :href="download.downloadUrl" class="download-btn-main" :class="{ 'download-btn-disabled': download.downloadUrl === '#' }">
            <i class="fas fa-download"></i>
            Windows 版本下载
          </a>
          <p v-if="download.downloadUrl === '#'" class="download-btn-tip">
            正式下载链接即将开放，可先
            <a href="mailto:linxiwanting@gmail.com" class="download-contact-link">联系作者</a>
            申请内测。
          </p>
        </div>
      </div>

      <!-- Install Steps -->
      <section class="download-steps-section">
        <h2 class="download-section-title">安装说明</h2>
        <div class="download-steps">
          <div
            v-for="(step, index) in steps"
            :key="index"
            class="download-step"
          >
            <div class="download-step-num">{{ index + 1 }}</div>
            <div class="download-step-content">
              <h3 class="download-step-title">{{ step.title }}</h3>
              <p class="download-step-desc">{{ step.desc }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- FAQ -->
      <section class="download-faq-section">
        <h2 class="download-section-title">常见问题</h2>
        <div class="download-faq-list">
          <div
            v-for="(item, index) in faq"
            :key="index"
            class="download-faq-item"
          >
            <h3 class="download-faq-q">
              <i class="fas fa-circle-question download-faq-icon"></i>
              {{ item.question }}
            </h3>
            <p class="download-faq-a">{{ item.answer }}</p>
          </div>
        </div>
      </section>

      <!-- Back link -->
      <div class="download-back">
        <NuxtLink to="/products/desktop-pet" class="download-back-link">
          <i class="fas fa-arrow-left"></i>
          返回产品介绍
        </NuxtLink>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { DESKTOP_PET } from '~/constants/products/desktopPet'

const download = DESKTOP_PET.download
const faq = DESKTOP_PET.faq

const steps = [
  { title: '下载安装包', desc: '点击上方下载按钮，获取 Windows 安装包。' },
  { title: '双击安装', desc: '运行下载的安装程序，按照提示完成安装。' },
  { title: '启动使用', desc: '安装完成后启动应用，即可常驻桌面开始使用。' }
]

useHead({
  title: '下载 AI 桌面陪伴助手',
  meta: [
    {
      name: 'description',
      content: '下载 AI 桌面陪伴助手 Windows 版，支持 Windows 10/11，安装后即可使用。'
    }
  ]
})
</script>

<style scoped>
.download-page {
  padding: 4rem 0 6rem;
}

.download-page-container {
  max-width: 56rem;
  margin: 0 auto;
  padding: 0 var(--spacing-lg);
}

.download-page-header {
  text-align: center;
  margin-bottom: 3rem;
}

.download-page-kicker {
  font-size: 0.8rem;
  font-weight: 600;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--color-primary);
  margin: 0 0 var(--spacing-sm);
}

.download-page-title {
  font-size: var(--font-size-h1);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-md);
}

.download-page-subtitle {
  font-size: 1rem;
  color: var(--color-text-muted);
  margin: 0;
}

.download-info-card {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-xl);
  padding: var(--spacing-xl);
  margin-bottom: 3rem;
  display: flex;
  gap: var(--spacing-xl);
  align-items: flex-start;
  flex-wrap: wrap;
}

.download-info-meta {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  min-width: 200px;
}

.download-info-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-md);
  padding-bottom: var(--spacing-md);
  border-bottom: 1px solid var(--color-border-subtle);
}

.download-info-row:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

.download-info-label {
  font-size: 0.9rem;
  color: var(--color-text-muted);
}

.download-info-value {
  font-size: 0.9rem;
  color: var(--color-text-main);
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 6px;
}

.download-info-version {
  font-family: monospace;
  font-size: 1rem;
  color: var(--color-primary);
}

.download-btn-wrap {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-md);
  padding-top: 4px;
}

.download-btn-main {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  padding: 12px 28px;
  background: var(--color-primary);
  color: #fff;
  border-radius: var(--radius-lg);
  font-size: 0.95rem;
  font-weight: 600;
  text-decoration: none;
  transition: opacity 0.2s, transform 0.2s;
  white-space: nowrap;
}

.download-btn-main:hover:not(.download-btn-disabled) {
  opacity: 0.9;
  transform: translateY(-1px);
}

.download-btn-disabled {
  opacity: 0.6;
  cursor: default;
  pointer-events: none;
}

.download-btn-tip {
  font-size: 0.85rem;
  color: var(--color-text-muted);
  text-align: center;
  margin: 0;
}

.download-contact-link {
  color: var(--color-primary);
  text-decoration: none;
}

.download-section-title {
  font-size: var(--font-size-h3);
  font-weight: 700;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-xl);
}

.download-steps-section {
  margin-bottom: 3rem;
}

.download-steps {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
}

.download-step {
  display: flex;
  gap: var(--spacing-lg);
  align-items: flex-start;
}

.download-step-num {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: var(--color-primary);
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1rem;
  flex-shrink: 0;
}

.download-step-title {
  font-size: 1rem;
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 4px;
}

.download-step-desc {
  font-size: 0.9rem;
  color: var(--color-text-muted);
  margin: 0;
}

.download-faq-section {
  margin-bottom: 3rem;
}

.download-faq-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.download-faq-item {
  background: var(--color-bg-card);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-lg) var(--spacing-xl);
}

.download-faq-q {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--color-text-main);
  margin: 0 0 var(--spacing-sm);
}

.download-faq-icon {
  color: var(--color-primary);
  font-size: 0.9rem;
}

.download-faq-a {
  font-size: 0.9rem;
  color: var(--color-text-muted);
  line-height: 1.6;
  margin: 0;
  padding-left: calc(0.9rem + var(--spacing-sm));
}

.download-back {
  text-align: center;
}

.download-back-link {
  display: inline-flex;
  align-items: center;
  gap: var(--spacing-sm);
  font-size: 0.9rem;
  color: var(--color-text-muted);
  text-decoration: none;
  transition: color 0.2s;
}

.download-back-link:hover {
  color: var(--color-primary);
}
</style>
