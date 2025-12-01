<template>
  <footer class="footer-pro py-16 sm:py-20 relative border-t border-border-subtle">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-12 mb-12">
        <!-- 左侧：品牌介绍 -->
        <div class="footer-brand">
          <h3 class="brand-title text-2xl font-bold text-text-main mb-4">XIFG</h3>
          <p class="brand-description text-text-muted leading-relaxed">
            围绕个人 IP 构建的创作操作系统，集内容、项目、AI 工具于一体的创作者平台。
          </p>
        </div>

        <!-- 中间/右侧：导航链接 -->
        <div class="footer-links">
          <h4 class="links-title text-lg font-semibold text-text-main mb-4">快速导航</h4>
          <nav class="links-grid">
            <NuxtLink to="/" class="footer-link">首页</NuxtLink>
            <NuxtLink to="/blog" class="footer-link">博客</NuxtLink>
            <NuxtLink to="/projects" class="footer-link">项目</NuxtLink>
            <NuxtLink to="/tools" class="footer-link">工具</NuxtLink>
            <NuxtLink to="/lab" class="footer-link">AI Lab</NuxtLink>
            <NuxtLink to="/about" class="footer-link">关于</NuxtLink>
          </nav>
        </div>

        <!-- 右侧：联系方式或额外信息 -->
        <div class="footer-contact">
          <h4 class="contact-title text-lg font-semibold text-text-main mb-4">联系我</h4>
          <div class="contact-links space-y-2">
            <a href="mailto:contact@xifg.com.cn" class="contact-link">
              <i class="fas fa-envelope mr-2"></i>
              Email
            </a>
            <a href="https://github.com" target="_blank" rel="noopener" class="contact-link">
              <i class="fab fa-github mr-2"></i>
              GitHub
            </a>
          </div>
        </div>
      </div>

      <!-- 底部版权信息 -->
      <div class="footer-bottom pt-8 border-t border-border-subtle">
        <div class="flex flex-col md:flex-row justify-between items-center gap-4">
          <div class="copyright text-text-muted text-sm">
            © {{ currentYear }} XIFG. All rights reserved.
          </div>
          <div class="built-with text-text-muted text-sm">
            Built by XIFG with <span class="text-text-main font-medium">Nuxt</span> & <span class="text-text-main font-medium">AI</span>
          </div>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
const currentYear = new Date().getFullYear()
</script>

<style scoped>
.footer-pro {
  background-color: var(--color-bg-body);
}

.brand-title {
  background: linear-gradient(to right, var(--color-primary), var(--color-primary-hover), var(--color-primary));
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
}

.footer-link {
  @apply block text-text-muted hover:text-text-main transition-colors duration-200 py-1;
}

.footer-link:hover {
  @apply transform translate-x-1;
}

.links-grid {
  @apply grid grid-cols-2 gap-2;
}

.contact-link {
  @apply inline-flex items-center text-text-muted hover:text-text-main transition-colors duration-200;
}

.contact-link:hover {
  @apply transform translate-x-1;
}
</style>

