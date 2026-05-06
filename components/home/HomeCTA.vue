<template>
  <section id="contact" class="home-section home-cta-section">
    <div class="home-shell">
      <div class="home-cta reveal-up">
        <div>
          <h2>一起构建更好的数字世界</h2>
          <p>如果你对 AI、产品、合作、交流感兴趣，欢迎联系我。</p>
        </div>
        <NuxtLink to="/contact" class="home-button home-button-primary">
          联系我
          <span aria-hidden="true">→</span>
        </NuxtLink>
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
  gap: 2rem;
  padding: clamp(2rem, 5vw, 3.5rem);
  border: 1px solid var(--home-border);
  border-radius: var(--home-radius-lg);
  background:
    radial-gradient(circle at 12% 15%, rgba(72, 111, 255, 0.18), transparent 36%),
    var(--home-card);
  box-shadow: var(--home-shadow-soft);
}

.home-cta h2 {
  margin: 0;
  color: var(--home-text-main);
  font-size: clamp(1.9rem, 4vw, 3.6rem);
  font-weight: 760;
  line-height: 1.08;
}

.home-cta p {
  margin: 1rem 0 0;
  color: var(--home-text-muted);
  font-size: 1.05rem;
}

@media (max-width: 760px) {
  .home-cta {
    align-items: flex-start;
    flex-direction: column;
  }
}
</style>
