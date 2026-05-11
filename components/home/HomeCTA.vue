<template>
  <section id="contact" class="contact-showcase">
    <div class="contact-frame">
      <div class="contact-stars" aria-hidden="true"></div>

      <div class="contact-layout reveal-up">
        <div class="contact-left">
          <div class="contact-title">
            <span>CONNECT &amp; COLLABORATE</span>
            <h2>合作 <em>&amp;</em> <strong>联系我</strong></h2>
            <p>如果你有有趣的想法、项目或合作机会，欢迎随时联系我。<br>一起探索 AI 与数字世界的更多可能。</p>
          </div>

          <div class="contact-methods-block">
            <h3>直接联系</h3>
            <div class="contact-methods">
              <article v-for="method in CONTACT_METHODS" :key="method.title" class="contact-card">
                <span class="contact-icon" :class="method.icon" aria-hidden="true"></span>
                <strong>{{ method.title }}</strong>
                <p>{{ method.value }}</p>
                <small>{{ method.note }}</small>
                <a
                  v-if="isNonRouterHref(method.href)"
                  :href="method.href"
                  class="contact-card-cta"
                  :target="method.href.startsWith('http') ? '_blank' : undefined"
                  :rel="method.href.startsWith('http') ? 'noreferrer noopener' : undefined"
                >
                  {{ method.action }}
                  <span aria-hidden="true">→</span>
                </a>
                <NuxtLink v-else :to="method.href" class="contact-card-cta">
                  {{ method.action }}
                  <span aria-hidden="true">→</span>
                </NuxtLink>
              </article>
            </div>
          </div>
        </div>

        <aside class="contact-right">
          <blockquote class="contact-quote">
            <span aria-hidden="true">“</span>
            <p>最好的合作，是互相成就，<br>一起把想法变成有价值的现实。</p>
            <cite>Xiuu</cite>
          </blockquote>

          <div class="collab-panel">
            <h3>合作方向</h3>
            <p>期待与你在以下方向合作</p>
            <div class="collab-list">
              <NuxtLink v-for="item in COLLAB_DIRECTIONS" :key="item.title" :to="item.href" class="collab-item">
                <span :class="item.icon" aria-hidden="true"></span>
                <span>
                  <strong>{{ item.title }}</strong>
                  <em>{{ item.description }}</em>
                </span>
                <i aria-hidden="true">→</i>
              </NuxtLink>
            </div>
          </div>
        </aside>
      </div>

      <div class="partners reveal-up reveal-delay-1">
        <h3>合作伙伴与客户</h3>
        <p>感谢以下伙伴的信任与支持</p>
        <div class="partner-row" aria-label="合作伙伴">
          <span v-for="partner in partners" :key="partner.name">
            <i :class="partner.icon" aria-hidden="true"></i>
            {{ partner.name }}
          </span>
          <span>以及更多合作伙伴...</span>
        </div>
      </div>

      <div class="contact-bottom reveal-up reveal-delay-2">
        <div class="contact-planet" aria-hidden="true">
          <span></span>
        </div>

        <div class="contact-bottom-copy">
          <h3>一起构建更好的数字未来</h3>
          <p>无论是一个想法、一个项目，还是一次合作，<br>都可能成为改变的开始。</p>
        </div>

        <div class="contact-bottom-action">
          <NuxtLink to="/contact">
            开始合作
            <span aria-hidden="true">→</span>
          </NuxtLink>
          <p>期待与你一起创造价值</p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { CONTACT_METHODS, COLLAB_DIRECTIONS } from '~/constants/homeContact'

/** mailto / tel / 绝对 URL 必须用原生 a，避免 NuxtLink 当作站内路由解析 */
const isNonRouterHref = (href: string) => /^(https?:|mailto:|tel:)/i.test(href)

const partners = [
  { name: 'OpenAI', icon: 'partner-openai' },
  { name: 'Vercel', icon: 'partner-vercel' },
  { name: 'Microsoft', icon: 'partner-microsoft' },
  { name: 'LangChain', icon: 'partner-langchain' },
  { name: 'Notion', icon: 'partner-notion' },
  { name: 'Cloudflare', icon: 'partner-cloudflare' },
  { name: 'GitHub', icon: 'partner-github' }
]
</script>

<style scoped>
.contact-showcase {
  padding: 1.7rem 2.4rem 2.1rem;
  background:
    radial-gradient(circle at 30% 14%, rgba(54, 88, 171, 0.13), transparent 31rem),
    radial-gradient(circle at 74% 22%, rgba(83, 62, 175, 0.1), transparent 30rem),
    #020713;
}

.contact-frame {
  position: relative;
  width: min(100%, 1840px);
  min-height: calc(100vh - 3.8rem);
  min-height: calc(100svh - 3.8rem);
  margin: 0 auto;
  padding: 8.35rem min(4.6vw, 5.8rem) 2.2rem;
  overflow: hidden;
  border: 1px solid var(--home-border);
  border-radius: 1.85rem;
  background:
    radial-gradient(circle at 30% 18%, rgba(44, 82, 174, 0.13), transparent 32rem),
    radial-gradient(circle at 68% 40%, rgba(65, 49, 162, 0.09), transparent 36rem),
    linear-gradient(180deg, rgba(4, 13, 34, 0.98), rgba(2, 8, 24, 0.99));
  box-shadow:
    0 28px 100px rgba(0, 0, 0, 0.34),
    inset 0 1px 0 rgba(255, 255, 255, 0.05);
}

.contact-stars {
  position: absolute;
  inset: 6.9rem 3rem auto 3rem;
  height: 35rem;
  pointer-events: none;
  background-image:
    radial-gradient(circle, rgba(129, 158, 255, 0.62) 0 1px, transparent 1.3px),
    radial-gradient(circle, rgba(149, 106, 255, 0.42) 0 1px, transparent 1.3px);
  background-position: 0 0, 4.8rem 2.8rem;
  background-size: 10rem 6.2rem, 12rem 7.4rem;
  opacity: 0.5;
}

.contact-layout,
.partners,
.contact-bottom {
  position: relative;
  z-index: 1;
}

.contact-layout {
  display: grid;
  grid-template-columns: minmax(0, 1.18fr) minmax(30rem, 0.78fr);
  gap: 2.5rem;
}

.contact-title > span {
  display: inline-flex;
  min-height: 1.7rem;
  align-items: center;
  padding: 0 0.72rem;
  border: 1px solid rgba(128, 151, 255, 0.22);
  border-radius: 999px;
  color: #afa4ff;
  background: rgba(83, 72, 184, 0.13);
  font-size: 0.78rem;
  font-weight: 760;
  letter-spacing: 0.08em;
}

.contact-title h2 {
  margin: 0.75rem 0 0;
  color: rgba(250, 252, 255, 0.98);
  font-size: clamp(2.55rem, 4vw, 4.1rem);
  line-height: 1.08;
  font-weight: 860;
}

.contact-title h2 em {
  color: rgba(232, 238, 255, 0.96);
  font-style: normal;
}

.contact-title h2 strong {
  color: #8f72ff;
  text-shadow: 0 0 25px rgba(118, 92, 255, 0.42);
}

.contact-title p {
  margin: 1.15rem 0 0;
  color: rgba(216, 226, 255, 0.74);
  font-size: 1.04rem;
  line-height: 1.85;
}

.contact-methods-block {
  margin-top: 4.35rem;
}

.contact-methods-block h3,
.partners h3,
.collab-panel h3 {
  margin: 0;
  color: rgba(250, 252, 255, 0.96);
  font-size: 1.22rem;
  font-weight: 820;
}

.contact-methods {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 1.2rem;
  margin-top: 1.45rem;
}

.contact-card,
.collab-panel,
.contact-bottom {
  border: 1px solid rgba(151, 179, 255, 0.17);
  border-radius: 1rem;
  background:
    linear-gradient(180deg, rgba(11, 25, 56, 0.72), rgba(5, 14, 34, 0.62)),
    rgba(6, 16, 39, 0.7);
  box-shadow:
    inset 0 1px 0 rgba(255, 255, 255, 0.05),
    0 18px 42px rgba(0, 0, 0, 0.2);
}

.contact-card {
  min-height: 17.6rem;
  display: grid;
  justify-items: center;
  align-content: start;
  padding: 2rem 1.5rem 1.35rem;
  text-align: center;
}

.contact-icon {
  position: relative;
  width: 4.8rem;
  height: 4.8rem;
  display: grid;
  place-items: center;
  margin-bottom: 1.25rem;
  border-radius: 1rem;
  background:
    radial-gradient(circle at 50% 42%, rgba(110, 149, 255, 0.65), transparent 2rem),
    linear-gradient(135deg, rgba(72, 105, 255, 0.6), rgba(54, 43, 171, 0.28));
  box-shadow: 0 18px 32px rgba(65, 95, 255, 0.22);
}

.contact-card:nth-child(2) .contact-icon {
  border-radius: 50%;
  background:
    radial-gradient(circle at 50% 45%, rgba(118, 231, 181, 0.92), transparent 2.4rem),
    rgba(14, 82, 62, 0.28);
}

.contact-card:nth-child(3) .contact-icon {
  border-radius: 50%;
  background:
    radial-gradient(circle at 35% 28%, rgba(140, 202, 255, 0.92), transparent 1.2rem),
    linear-gradient(135deg, #4e9dff, #315cd6);
}

.contact-card:nth-child(4) .contact-icon {
  background:
    radial-gradient(circle at 50% 38%, rgba(153, 108, 255, 0.78), transparent 2.1rem),
    linear-gradient(135deg, rgba(136, 86, 255, 0.62), rgba(51, 34, 127, 0.42));
}

.contact-card strong {
  color: rgba(250, 252, 255, 0.96);
  font-size: 1.18rem;
  font-weight: 820;
}

.contact-card p {
  min-height: 1.6rem;
  margin: 0.8rem 0 0;
  color: rgba(221, 230, 255, 0.78);
  font-size: 0.95rem;
}

.contact-card small {
  min-height: 1.5rem;
  margin-top: 0.7rem;
  color: rgba(188, 203, 244, 0.62);
  font-size: 0.86rem;
}

.contact-card a.contact-card-cta,
.contact-card a.contact-card-cta:visited {
  min-width: 11.5rem;
  min-height: 3rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.7rem;
  margin-top: auto;
  border: 1px solid rgba(138, 164, 255, 0.18);
  border-radius: 999px;
  color: #aab8ff;
  background: rgba(255, 255, 255, 0.028);
  font-weight: 720;
  text-decoration: none;
}

.contact-right {
  display: grid;
  align-content: start;
  gap: 3.1rem;
}

.contact-quote {
  position: relative;
  min-height: 8.85rem;
  margin: 0;
  padding: 1.75rem 2.25rem 1.2rem 3.35rem;
  border: 1px solid rgba(151, 179, 255, 0.13);
  border-radius: 1.25rem;
  background: linear-gradient(180deg, rgba(7, 18, 43, 0.54), rgba(5, 13, 34, 0.28));
}

.contact-quote > span {
  position: absolute;
  left: 1.18rem;
  top: 0.98rem;
  color: rgba(142, 157, 230, 0.68);
  font-family: Georgia, serif;
  font-size: 3.65rem;
  line-height: 1;
}

.contact-quote p {
  margin: 0;
  color: rgba(235, 241, 255, 0.86);
  font-size: 1.05rem;
  line-height: 1.85;
}

.contact-quote cite {
  position: absolute;
  right: 4.2rem;
  bottom: -1rem;
  color: #a277ff;
  font-family: "Brush Script MT", "Segoe Script", cursive;
  font-size: 2.5rem;
  font-style: italic;
  font-weight: 300;
}

.collab-panel {
  min-height: 28.2rem;
  padding: 2rem 1.65rem 1.55rem;
}

.collab-panel > p,
.partners p {
  margin: 0.82rem 0 0;
  color: rgba(197, 211, 249, 0.64);
  font-size: 0.93rem;
}

.collab-list {
  display: grid;
  gap: 0.42rem;
  margin-top: 1.55rem;
}

.collab-item {
  min-height: 4.65rem;
  display: grid;
  grid-template-columns: 3.2rem minmax(0, 1fr) auto;
  align-items: center;
  gap: 1rem;
  padding: 0.78rem 1rem;
  border: 1px solid rgba(146, 171, 255, 0.12);
  border-radius: 0.9rem;
  color: inherit;
  background: rgba(255, 255, 255, 0.018);
}

.collab-item > span:first-child {
  position: relative;
  width: 2.7rem;
  height: 2.7rem;
  border-radius: 0.55rem;
  background: rgba(69, 98, 255, 0.16);
}

.collab-item span span {
  display: grid;
}

.collab-item strong {
  color: rgba(250, 252, 255, 0.96);
  font-size: 0.98rem;
  font-weight: 780;
}

.collab-item em {
  margin-top: 0.35rem;
  overflow: hidden;
  color: rgba(196, 211, 249, 0.64);
  font-size: 0.86rem;
  font-style: normal;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.collab-item i {
  color: #aab8ff;
  font-style: normal;
  font-size: 1.25rem;
}

.partners {
  margin-top: 2.25rem;
}

.partner-row {
  display: grid;
  grid-template-columns: repeat(8, minmax(0, auto));
  align-items: center;
  justify-content: space-between;
  gap: 1.3rem;
  margin-top: 2rem;
  padding: 0 1rem;
}

.partner-row span {
  display: inline-flex;
  align-items: center;
  gap: 0.48rem;
  color: rgba(198, 208, 233, 0.66);
  font-size: 1.08rem;
  font-weight: 700;
  white-space: nowrap;
}

.partner-row i {
  position: relative;
  width: 1.45rem;
  height: 1.45rem;
  display: inline-block;
  color: rgba(198, 208, 233, 0.7);
}

.contact-bottom {
  min-height: 11.8rem;
  display: grid;
  grid-template-columns: 23rem minmax(24rem, 1fr) 19rem;
  align-items: center;
  gap: 2.3rem;
  margin-top: 2.45rem;
  overflow: hidden;
}

.contact-planet {
  position: relative;
  align-self: stretch;
  min-height: 11.8rem;
  overflow: hidden;
  background:
    radial-gradient(circle at 16% 92%, rgba(96, 102, 255, 0.78), transparent 8rem),
    radial-gradient(circle at 0% 108%, #0e1e5f 0 10rem, transparent 10.2rem),
    linear-gradient(180deg, rgba(34, 55, 128, 0.18), rgba(4, 13, 35, 0.14));
}

.contact-planet::before {
  content: '';
  position: absolute;
  left: -5.1rem;
  bottom: -7.4rem;
  width: 20rem;
  height: 20rem;
  border-radius: 50%;
  background:
    radial-gradient(circle at 48% 30%, rgba(127, 148, 255, 0.95), transparent 0.45rem),
    radial-gradient(circle at 50% 50%, rgba(55, 72, 171, 0.95), #06153d 63%);
  box-shadow:
    0 0 64px rgba(102, 112, 255, 0.52),
    inset 1rem 1rem 2.6rem rgba(210, 220, 255, 0.16);
}

.contact-planet span {
  position: absolute;
  left: 6.2rem;
  bottom: 3.1rem;
  width: 2.3rem;
  height: 2.3rem;
  border-radius: 50%;
  background: radial-gradient(circle at 35% 30%, #8fa8ff, #172b80 72%);
  box-shadow: 0 0 18px rgba(111, 133, 255, 0.46);
}

.contact-bottom-copy h3 {
  margin: 0;
  color: rgba(218, 229, 255, 0.98);
  font-size: 1.45rem;
  font-weight: 820;
}

.contact-bottom-copy p {
  margin: 0.9rem 0 0;
  color: rgba(199, 213, 249, 0.66);
  font-size: 1rem;
  line-height: 1.75;
}

.contact-bottom-action {
  display: grid;
  justify-items: center;
  gap: 0.85rem;
  padding-right: 2.2rem;
}

.contact-bottom-action a {
  min-width: 12.8rem;
  min-height: 3.35rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.8rem;
  border-radius: 999px;
  color: #fff;
  background: linear-gradient(135deg, #5f8cff, #6a55ff);
  box-shadow: 0 18px 38px rgba(79, 93, 255, 0.36);
  font-weight: 820;
}

.contact-bottom-action p {
  margin: 0;
  color: rgba(196, 210, 249, 0.58);
  font-size: 0.92rem;
}

.contact-icon::before,
.contact-icon::after,
.collab-item > span:first-child::before,
.collab-item > span:first-child::after,
.partner-row i::before,
.partner-row i::after {
  content: '';
  position: absolute;
}

.icon-mail::before {
  left: 1rem;
  top: 1.45rem;
  width: 2.8rem;
  height: 1.9rem;
  border: 2px solid #9fbaff;
  border-radius: 0.25rem;
  background: linear-gradient(180deg, rgba(107, 135, 255, 0.38), rgba(47, 63, 194, 0.48));
}

.icon-mail::after {
  left: 1.16rem;
  top: 1.55rem;
  width: 2.48rem;
  height: 1.36rem;
  border-left: 2px solid #9fbaff;
  border-bottom: 2px solid #9fbaff;
  transform: rotate(-45deg) skew(8deg, 8deg);
}

.icon-wechat::before,
.icon-wechat::after {
  border-radius: 50%;
  background: #89f0b8;
  box-shadow: 0 0 20px rgba(137, 240, 184, 0.35);
}

.icon-wechat::before {
  left: 1.1rem;
  top: 1.28rem;
  width: 1.9rem;
  height: 1.5rem;
}

.icon-wechat::after {
  right: 1.05rem;
  top: 1.7rem;
  width: 1.6rem;
  height: 1.25rem;
}

.icon-telegram::before {
  left: 1.18rem;
  top: 1.18rem;
  width: 2.55rem;
  height: 2.55rem;
  border-radius: 50%;
  background: linear-gradient(135deg, #88c7ff, #356fe7);
}

.icon-telegram::after {
  left: 1.9rem;
  top: 1.82rem;
  width: 1.1rem;
  height: 1.1rem;
  border-top: 0.42rem solid transparent;
  border-bottom: 0.42rem solid transparent;
  border-left: 1.25rem solid #eaf2ff;
  transform: rotate(-34deg);
}

.icon-calendar::before {
  left: 1.04rem;
  top: 1.15rem;
  width: 2.7rem;
  height: 2.35rem;
  border-radius: 0.35rem;
  background: linear-gradient(180deg, #9d7cff, #5d39ce);
}

.icon-calendar::after {
  left: 1.25rem;
  top: 1.72rem;
  width: 2.28rem;
  height: 1px;
  background: rgba(238, 232, 255, 0.75);
  box-shadow:
    0 0.55rem 0 rgba(238, 232, 255, 0.55),
    0.7rem 0.55rem 0 rgba(238, 232, 255, 0.55);
}

.collab-ai::before {
  content: '♟';
  inset: 0;
  display: grid;
  place-items: center;
  color: #76a0ff;
  font-size: 1.25rem;
}

.collab-product {
  background: rgba(28, 158, 122, 0.15) !important;
}

.collab-product::before {
  inset: 0.72rem;
  border: 2px solid #69e0b8;
  border-radius: 0.2rem;
}

.collab-product::after {
  left: 0.9rem;
  top: 1rem;
  width: 1rem;
  height: 1px;
  background: #69e0b8;
  box-shadow: 0 0.45rem 0 #69e0b8;
}

.collab-knowledge::before {
  left: 0.9rem;
  top: 0.75rem;
  width: 1.05rem;
  height: 1.05rem;
  border: 2px solid #7791ff;
  transform: rotate(30deg) skew(-8deg, -8deg);
}

.collab-consult {
  background: rgba(162, 75, 205, 0.16) !important;
}

.collab-consult::before {
  content: '▣';
  inset: 0;
  display: grid;
  place-items: center;
  color: #db7cff;
  font-size: 1.2rem;
}

.partner-openai::before {
  inset: 0.16rem;
  border: 2px solid currentColor;
  border-radius: 50%;
}

.partner-openai::after {
  content: '✺';
  inset: -0.08rem 0 0;
  display: grid;
  place-items: center;
  font-size: 1rem;
}

.partner-vercel::before {
  left: 0.1rem;
  top: 0.22rem;
  border-left: 0.68rem solid transparent;
  border-right: 0.68rem solid transparent;
  border-bottom: 1.18rem solid currentColor;
}

.partner-microsoft::before {
  inset: 0.15rem;
  background:
    linear-gradient(currentColor 0 0) left top / 0.56rem 0.56rem no-repeat,
    linear-gradient(currentColor 0 0) right top / 0.56rem 0.56rem no-repeat,
    linear-gradient(currentColor 0 0) left bottom / 0.56rem 0.56rem no-repeat,
    linear-gradient(currentColor 0 0) right bottom / 0.56rem 0.56rem no-repeat;
}

.partner-langchain::before {
  inset: 0.28rem;
  border: 2px solid currentColor;
  border-radius: 999px;
}

.partner-langchain::after {
  content: '∞';
  inset: -0.05rem 0 0;
  display: grid;
  place-items: center;
  font-size: 1rem;
}

.partner-notion::before {
  inset: 0.12rem;
  border: 2px solid currentColor;
  border-radius: 0.12rem;
}

.partner-notion::after {
  content: 'N';
  inset: 0;
  display: grid;
  place-items: center;
  font-family: Georgia, serif;
  font-size: 1rem;
}

.partner-cloudflare::before {
  left: 0.05rem;
  top: 0.58rem;
  width: 1.35rem;
  height: 0.65rem;
  border-radius: 999px;
  background: currentColor;
}

.partner-github::before {
  content: '◖';
  inset: -0.38rem 0 0;
  display: grid;
  place-items: center;
  font-size: 2.4rem;
}

@media (max-width: 1320px) {
  .contact-layout,
  .contact-bottom {
    grid-template-columns: 1fr;
  }

  .contact-methods {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .partner-row {
    grid-template-columns: repeat(4, minmax(0, 1fr));
  }

  .contact-bottom-action {
    justify-items: start;
    padding: 0 0 1.5rem 2rem;
  }
}

@media (max-width: 860px) {
  .contact-showcase {
    padding: 0.8rem;
  }

  .contact-frame {
    padding: 6.2rem 1rem 1rem;
    border-radius: 1.2rem;
  }

  .contact-methods,
  .partner-row {
    grid-template-columns: 1fr;
  }

  .contact-card {
    min-height: 15rem;
  }
}
</style>
