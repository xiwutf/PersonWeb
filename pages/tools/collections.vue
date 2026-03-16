<template>
  <div class="min-h-screen bg-[var(--color-text-main)] text-slate-200 relative overflow-hidden font-['Outfit']">
    <div class="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- čżĺćéŽ -->
      <div class="tools-back-button-container">
        <NuxtLink to="/tools" class="tools-back-button">
          <svg class="tools-back-button-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
          </svg>
          <span>čżĺćäťśĺˇĽĺˇ</span>
        </NuxtLink>
      </div>

      <!-- éĄľé˘ĺ¤´é¨ -->
      <header class="text-center mb-12">
        <h1 class="text-4xl md:text-5xl font-bold mb-4 bg-clip-text text-transparent bg-gradient-to-r from-orange-200 via-var(--color-bg-light, white) to-red-200">
          ĺˇĽĺˇĺé
        </h1>
        <p class="text-lg text-slate-400 max-w-2xl mx-auto">
          ç˛žéĺˇĽĺˇćĺďźä¸ćŹĄč´­äš°ďźĺ¨é¨ćĽć
        </p>
      </header>

      <!-- ć¨čĺé -->
      <div v-if="featuredCollections.length > 0" class="mb-12">
        <h2 class="text-2xl font-bold mb-6">ć¨čĺé</h2>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div
            v-for="collection in featuredCollections"
            :key="collection.id"
            class="bg-slate-800/30 backdrop-blur-md border border-var(--color-bg-light, white)/5 rounded-3xl overflow-hidden hover:bg-slate-800/50 transition-all hover:border-orange-500/30"
          >
            <div v-if="collection.coverImage" class="h-48 overflow-hidden">
              <img :src="collection.coverImage" :alt="collection.name" class="w-full h-full object-cover" />
            </div>
            <div class="p-6">
              <div class="flex items-start justify-between mb-4">
                <h3 class="text-2xl font-bold">{{ collection.name }}</h3>
                <div class="text-right">
                  <div class="text-2xl font-bold text-emerald-400">ÂĽ{{ collection.price }}</div>
                  <div v-if="collection.originalPrice" class="text-sm text-slate-500 line-through">ÂĽ{{ collection.originalPrice }}</div>
                </div>
              </div>
              <p class="text-slate-400 mb-4">{{ collection.description }}</p>
              <div class="flex items-center justify-between">
                <div class="text-sm text-slate-500">
                  ĺĺŤ {{ collection.toolCount }} ä¸ŞĺˇĽĺ?                </div>
                <button
                  @click="handlePurchaseCollection(collection)"
                  class="px-6 py-2 bg-gradient-to-r from-orange-600 to-red-600 hover:from-orange-500 hover:to-red-500 text-var(--color-bg-light, white) rounded-xl transition-all"
                >
                  çŤĺłč´­äš°
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- ććĺé?-->
      <div>
        <h2 class="text-2xl font-bold mb-6">ććĺé?/h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          <div
            v-for="collection in collections"
            :key="collection.id"
            class="bg-slate-800/30 backdrop-blur-md border border-var(--color-bg-light, white)/5 rounded-3xl overflow-hidden hover:bg-slate-800/50 transition-all"
          >
            <div class="p-6">
              <h3 class="text-xl font-bold mb-2">{{ collection.name }}</h3>
              <p class="text-slate-400 text-sm mb-4 line-clamp-2">{{ collection.description }}</p>
              <div class="flex items-center justify-between">
                <div>
                  <div class="text-lg font-bold text-emerald-400">ÂĽ{{ collection.price }}</div>
                  <div class="text-xs text-slate-500">{{ collection.toolCount }} ä¸ŞĺˇĽĺ?/div>
                </div>
                <button
                  @click="handlePurchaseCollection(collection)"
                  class="px-4 py-2 bg-gradient-to-r from-orange-600 to-red-600 hover:from-orange-500 hover:to-red-500 text-var(--color-bg-light, white) rounded-xl transition-all text-sm"
                >
                  č´­äš°
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const api = useApi()

interface Collection {
  id: number
  name: string
  slug: string
  description?: string
  coverImage?: string
  price: number
  originalPrice?: number
  toolCount: number
  purchaseCount: number
  isFeatured: boolean
}

const collections = ref<Collection[]>([])
const featuredCollections = computed(() => collections.value.filter(c => c.isFeatured))
const loading = ref(false)

// čˇĺĺéĺčĄ¨
const fetchCollections = async () => {
  loading.value = true
  try {
    const res = await api.get<Collection[]>('/Toolbox/collections')
    if (res && Array.isArray(res)) {
      collections.value = res
    }
  } catch (e) {
    console.error('čˇĺĺéĺčĄ¨ĺ¤ąč´Ľ', e)
    // ĺŚćAPIĺ¤ąč´Ľďźä˝żç¨çŠşć°çť
    collections.value = []
  } finally {
    loading.value = false
  }
}

// č´­äš°ĺé
const handlePurchaseCollection = async (collection: Collection) => {
  const visitorId = localStorage.getItem('visitor_id')
  if (!visitorId) {
    alert('čŻˇĺçťĺ˝')
    return
  }

  // TODO: ĺŽç°č´­äš°ĺéçAPI
  alert('č´­äš°ĺéĺč˝ĺźĺä¸­')
}

onMounted(() => {
  fetchCollections()
})

useHead({
  title: 'ĺˇĽĺˇĺé - ćşŞĺĺŹéŁ',
  meta: [
    { name: 'description', content: 'ç˛žéĺˇĽĺˇćĺďźä¸ćŹĄč´­äš°ďźĺ¨é¨ćĽć' }
  ]
})
</script>

<style scoped>
/* čżĺćéŽć ˇĺź */
.tools-back-button-container {
  margin-bottom: 1.5rem;
}

.tools-back-button {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: rgba(30, 41, 59, 0.5);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.5rem;
  color: rgb(203, 213, 225);
  text-decoration: none;
  transition: all 0.3s ease;
}

.tools-back-button:hover {
  background: rgba(30, 41, 59, 0.7);
  color: var(--color-bg-light, white);
}

.tools-back-button-icon {
  width: 1.25rem;
  height: 1.25rem;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>

