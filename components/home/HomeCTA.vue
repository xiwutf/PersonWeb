<template>
  <section id="contact" class="home-section home-cta-section">
    <div class="home-shell">
      <div class="home-cta reveal-up">
        <div class="home-cta-copy">
          <p class="home-cta-kicker">Let's Build Together</p>
          <h2>有 AI 项目想法？欢迎聊聊</h2>
          <p>无论是 AI 应用落地、企业软件定制，还是个人产品合作，都可以找我。</p>
        </div>
        <div class="home-cta-actions">
          <NuxtLink to="/contact" class="home-button home-button-primary">
            联系我 →
          </NuxtLink>
          <NuxtLink to="/projects" class="home-button home-button-secondary">
            查看案例
          </NuxtLink>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.home-cta-section {
  padding-bottom: 6rem;
}

.home-cta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 2.5rem;
  padding: clamp(2rem, 5vw, 3.5rem);
  border: 1px solid var(--home-border);
  border-radius: var(--home-radius-lg);
  background:
    radial-gradient(circle at 8% 20%, rgba(72, 111, 255, 0.14), transparent 42%),
    radial-gradient(circle at 92% 80%, rgba(130, 90, 255, 0.1), transparent 38%),
    var(--home-card);
  box-shadow: var(--home-shadow-soft);
  overflow: hidden;
}

.home-cta-kicker {
  margin: 0 0 0.75rem;
  color: var(--home-accent);
  font-size: 0.78rem;
  font-weight: 700;
  letter-spacing: 0.06em;
  text-transform: uppercase;
}

.home-cta h2 {
  margin: 0 0 0.75rem;
  color: var(--home-text-main);
  font-size: clamp(1.8rem, 3.5vw, 3rem);
  font-weight: 760;
  line-height: 1.1;
}

.home-cta p {
  margin: 0;
  color: var(--home-text-muted);
  font-size: 1rem;
  line-height: 1.75;
}

.home-cta-actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  flex-shrink: 0;
}

@media (max-width: 760px) {
  .home-cta {
    align-items: flex-start;
    flex-direction: column;
  }
  .home-cta-actions {
    flex-direction: row;
    flex-wrap: wrap;
  }
}
</style>
