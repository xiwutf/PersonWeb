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
  margin-bottom: 2rem;
}

.page-title {
  font-size: 1.875rem;
  font-weight: 700;
  color: #ffffff;
  margin-bottom: 0.5rem;
}

.settings-grid {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.settings-section {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.75rem;
  padding: 1.5rem;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #ffffff;
  margin-bottom: 1.5rem;
  display: flex;
  align-items: center;
}

.settings-cards {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1rem;
}

.settings-card {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1.25rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  transition: all 0.3s ease;
  text-decoration: none;
  color: inherit;
  cursor: pointer;
}

.settings-card:hover {
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
}

.card-icon {
  width: 3rem;
  height: 3rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(59, 130, 246, 0.2);
  border-radius: 0.5rem;
  color: #60a5fa;
  font-size: 1.5rem;
  flex-shrink: 0;
}

.card-content {
  flex: 1;
  min-width: 0;
}

.card-title {
  font-size: 1rem;
  font-weight: 600;
  color: #ffffff;
  margin-bottom: 0.25rem;
}

.card-desc {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.6);
  line-height: 1.4;
}

.card-arrow {
  color: rgba(255, 255, 255, 0.4);
  font-size: 0.875rem;
  flex-shrink: 0;
}

.settings-card:hover .card-arrow {
  color: rgba(255, 255, 255, 0.8);
  transform: translateX(4px);
}
</style>

