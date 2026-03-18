<template>
  <div class="settings-page">
    <div class="page-header">
      <h1 class="page-title">系统设置</h1>
      <p class="text-gray-400 text-sm">统一管理系统配置和风格设置</p>
    </div>

    <div class="settings-grid">
      <!-- 基础设置 -->
      <div class="settings-section">
        <h2 class="section-title">
          <i class="fas fa-cog mr-2"></i>
          基础设置
        </h2>
        <div class="settings-cards">
          <NuxtLink to="/admin/config" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-cog"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">站点配置</h3>
              <p class="card-desc">管理网站的基本配置信息</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>

          <NuxtLink to="/admin/users" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-users"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">用户管理</h3>
              <p class="card-desc">管理系统用户和权限</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>

          <NuxtLink to="/admin/settings/change-password" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-key"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">修改密码</h3>
              <p class="card-desc">修改当前管理员账户的登录密码</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>

          <NuxtLink to="/admin/friend-links" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-link"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">友情链接</h3>
              <p class="card-desc">管理友情链接列表</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>
        </div>
      </div>

      <!-- 风格设置 -->
      <div class="settings-section">
        <h2 class="section-title">
          <i class="fas fa-palette mr-2"></i>
          风格设置
        </h2>
        <div class="settings-cards">
          <NuxtLink to="/admin/home-styles" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-home"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">首页风格管理</h3>
              <p class="card-desc">管理首页的视觉风格和主题</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>

          <NuxtLink to="/admin/admin-styles" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-paint-brush"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">后台风格管理</h3>
              <p class="card-desc">管理后台的视觉风格和主题</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>

          <NuxtLink to="/admin/settings/styles" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-palette"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">样式管理</h3>
              <p class="card-desc">统一管理系统样式配置，支持动态修改</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>

          <NuxtLink to="/admin/settings/themes" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-paint-brush"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">主题风格管理</h3>
              <p class="card-desc">管理网站主题风格和动态背景效果</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>

          <NuxtLink to="/admin/settings/modules" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-puzzle-piece"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">模块管理</h3>
              <p class="card-desc">管理功能模块的启用和禁用，实现按需加载</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>

          <NuxtLink to="/admin/settings/fonts" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-font"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">字体管理</h3>
              <p class="card-desc">管理系统字体配置，包括字体族、字号、字重等设置</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>
        </div>
      </div>

      <!-- 系统维护 -->
      <div class="settings-section">
        <h2 class="section-title">
          <i class="fas fa-tools mr-2"></i>
          系统维护
        </h2>
        <div class="settings-cards">
          <NuxtLink to="/admin/error-logs" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-exclamation-triangle"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">错误日志</h3>
              <p class="card-desc">查看和管理系统错误日志</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>
        </div>
      </div>

      <!-- 访客互动 -->
      <div class="settings-section">
        <h2 class="section-title">
          <i class="fas fa-users mr-2"></i>
          访客互动
        </h2>
        <div class="settings-cards">
          <NuxtLink to="/admin/visitor-messages" class="settings-card">
            <div class="card-icon">
              <i class="fas fa-comments"></i>
            </div>
            <div class="card-content">
              <h3 class="card-title">访客留言管理</h3>
              <p class="card-desc">审核和管理访客发送的弹幕、留言、心情和祝福</p>
            </div>
            <i class="fas fa-chevron-right card-arrow"></i>
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})
</script>

<style scoped>
.settings-page {
  width: 100%;
}

.page-header {
  margin-bottom: var(--spacing-2xl);
}

.page-title {
  font-size: var(--text-xl);
  font-weight: 700;
  color: var(--color-text-main);
  margin-bottom: var(--spacing-md);
}

.settings-grid {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-2xl);
}

.settings-section {
  background: var(--color-bg-elevated, var(--color-bg-card));
  backdrop-filter: blur(10px);
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-lg);
  padding: var(--spacing-xl);
}

.section-title {
  font-size: var(--text-lg);
  font-weight: 600;
  color: var(--color-text-main);
  margin-bottom: var(--spacing-xl);
  display: flex;
  align-items: center;
}

.settings-cards {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: var(--spacing-md);
}

.settings-card {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-xl);
  background: var(--color-bg-elevated, var(--color-bg-card));
  border: 1px solid var(--color-border-subtle);
  border-radius: var(--radius-md);
  transition: all 0.3s ease;
  text-decoration: none;
  color: inherit;
  cursor: pointer;
}

.settings-card:hover {
  background: var(--color-bg-elevated);
  border-color: var(--color-border-default);
  transform: translateY(-2px);
}

.card-icon {
  width: var(--text-3xl);
  height: var(--text-3xl);
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-primary-soft);
  border-radius: var(--radius-md);
  color: var(--color-primary);
  font-size: var(--text-3xl);
  flex-shrink: 0;
}

.card-content {
  flex: 1;
  min-width: 0;
}

.card-title {
  font-size: var(--text-base);
  font-weight: 600;
  color: var(--color-text-main);
  margin-bottom: var(--spacing-xs);
}

.card-desc {
  font-size: var(--text-sm);
  color: var(--color-text-muted);
  line-height: 1.4;
}

.card-arrow {
  color: var(--color-text-muted);
  font-size: var(--text-sm);
  flex-shrink: 0;
}

.settings-card:hover .card-arrow {
  color: var(--color-text-main);
  transform: translateX(var(--spacing-sm));
}
</style>

