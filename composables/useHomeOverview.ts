import type { HomeOverview } from '~/types/home'

const FALLBACK: HomeOverview = {
  stats: { projects: 20, articles: 50, tools: 8 },
  featuredProjects: [],
  featuredArticle: null,
  latestArticles: [],
  nowBuilding: [],
  journey: [
    {
      id: 0,
      year: 2022,
      title: '探索开始',
      description: '接触编程与自动化，开启数字世界的探索之旅。',
      icon: 'icon-terminal',
      color: 'blue',
      isNow: false,
    },
    {
      id: 1,
      year: 2024,
      title: '产品探索',
      description: '从想法到原型，开始构建自己的产品。',
      icon: 'icon-cube',
      color: 'purple',
      isNow: false,
    },
    {
      id: 2,
      year: new Date().getFullYear(),
      title: '系统构建',
      description: '专注产品化与系统化，打造可复用的解决方案。',
      icon: 'icon-layers',
      color: 'blue',
      isNow: true,
    },
  ],
}

export const useHomeOverview = () => {
  const { data, pending, error } = useLazyAsyncData(
    'home-overview',
    () => $fetch<HomeOverview>('/api/home/overview'),
    { server: false }
  )

  const overview = computed<HomeOverview>(() => data.value ?? FALLBACK)
  const loading = computed(() => pending.value)

  return { overview, loading, error }
}
