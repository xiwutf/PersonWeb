<template>
  <header class="life-header">
    <div class="life-header-inner">
      <NuxtLink to="/life" class="life-header-brand">
        <LifeIcon name="branch" class="life-header-mark" />
        溪午听风
      </NuxtLink>

      <nav class="life-header-nav" aria-label="生活站导航">
        <NuxtLink to="/life" :class="{ 'is-active': isHome }">首页</NuxtLink>
        <NuxtLink to="/life/about" :class="{ 'is-active': isAbout }">关于</NuxtLink>
        <NuxtLink to="/life/notes" :class="{ 'is-active': isNotes }">随笔</NuxtLink>
        <NuxtLink to="/life/thoughts" :class="{ 'is-active': isThoughts }">摘句</NuxtLink>
        <NuxtLink to="/life/cognition" :class="{ 'is-active': isCognition }">说明书</NuxtLink>
      </nav>

      <nav class="life-header-switch" aria-label="切换站点">
        <NuxtLink to="/work">Work</NuxtLink>
        <NuxtLink to="/">入口</NuxtLink>
        <button
          v-if="loaded"
          type="button"
          class="life-header-auth"
          :class="{ 'is-on': authenticated }"
          :title="authenticated ? '双击退出登录' : '双击登录'"
          :aria-label="authenticated ? '双击退出登录' : '双击登录'"
          @dblclick.prevent="onAuthDoubleClick"
        >
          <LifeIcon name="seal" class="life-header-auth-icon" />
        </button>
      </nav>
    </div>
  </header>
</template>

<script setup lang="ts">
const route = useRoute()
const router = useRouter()
const {
  authenticated,
  loaded,
  logout,
} = useAdminSession()

const isHome = computed(() => route.path === '/life')
const isAbout = computed(() => route.path === '/life/about' || route.path.startsWith('/life/about/'))
const isThoughts = computed(() =>
  route.path === '/life/thoughts'
  || route.path.startsWith('/life/thoughts/')
  || route.path === '/life/margin'
  || route.path.startsWith('/life/margin/'),
)
const isCognition = computed(() => route.path === '/life/cognition' || route.path.startsWith('/life/cognition/'))
const isNotes = computed(() => {
  const path = route.path
  if (
    path === '/life'
    || path === '/life/about'
    || path.startsWith('/life/about/')
    || isThoughts.value
    || isCognition.value
  ) {
    return false
  }
  return path === '/life/notes' || path.startsWith('/life/notes/') || path.startsWith('/life/')
})

async function onAuthDoubleClick() {
  if (authenticated.value) {
    await logout()
    return
  }
  await router.push('/admin/login?redirect=/life')
}
</script>
