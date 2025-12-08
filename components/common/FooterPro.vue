<template>
  <footer class="footer-pro">
    <div class="footer-pro-container">
      <!-- 底部版权信息 -->
      <div class="footer-bottom">
        <div class="footer-bottom-content">
          <div class="copyright">
            © {{ currentYear }} XIFG. All rights reserved.
          </div>
          <div class="built-with">
            Built by XIFG with <span class="built-with-highlight">Nuxt</span> & <span class="built-with-highlight">AI</span>
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
  border-top: 1px solid var(--color-border-subtle);
  padding-top: 2rem;
  padding-bottom: 2rem;
}

.footer-pro-container {
  max-width: 90rem;
  margin-left: auto;
  margin-right: auto;
  padding-left: 1rem;
  padding-right: 1rem;
}

@media (min-width: 640px) {
  .footer-pro-container {
    padding-left: 1.5rem;
    padding-right: 1.5rem;
  }
}

@media (min-width: 1024px) {
  .footer-pro-container {
    max-width: 100rem;
    padding-left: 2rem;
    padding-right: 2rem;
  }
}

.footer-bottom {
  padding-top: 1rem;
}

.footer-bottom-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

@media (min-width: 768px) {
  .footer-bottom-content {
    flex-direction: row;
    justify-content: space-between;
  }
}

.copyright {
  color: var(--color-text-muted);
  font-size: 0.875rem;
}

.built-with {
  color: var(--color-text-muted);
  font-size: 0.875rem;
}

.built-with-highlight {
  color: var(--color-text-main);
  font-weight: 500;
}
</style>

