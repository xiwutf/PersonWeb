<template>
  <header class="header-container floating-nav">
    <div class="header-content-wrapper">
      <div class="header-main">
        <!-- Logo 闂傚倷绀侀幉锟犳偋閺囥垹绠规い鎰堕檮閸?-->
        <NuxtLink to="/" class="header-logo-link">
          <div 
            @click.stop="handleLogoClick"
            @mouseenter="handleAvatarHover"
            class="header-logo-avatar"
            title="点击头像可进入管理后台"
          >
            <img src="/images/avatar.jpg" alt="网站头像" />
          </div>
          <span 
            class="header-logo-text logo-text"
          >Ming Studio</span>
        </NuxtLink>

        <!-- 濠电姷顣介崜婵嬨€冮崨瀛樺剹闁稿本鍑归悞浠嬫煙濞堝灝鏋ら柛妤佺閵囧嫰寮介妸銉ユ瘓闂佹悶鍊戦崐婵嬪蓟閵堝绀堥棅顐幗閹癸綁鎮峰鍕凡缂佸鎳撻悾?-->
        <nav class="header-nav-desktop">
          <NuxtLink
            v-for="item in mainNavigationItems"
            :key="item.path"
            :to="item.path"
            class="header-nav-link"
            :class="isActiveRoute(item.path)
              ? 'header-nav-link-active' 
              : 'header-nav-link-inactive'"
            @click="handleNavClick(item.path, $event)"
          >
            <span class="header-nav-link-icon">{{ item.icon }}</span>
            <span class="header-nav-link-label">{{ item.title }}</span>
          </NuxtLink>
          
          <!-- 闂傚倷绀侀幖顐⒚洪敃鈧…鍥р枎閹惧秴娲畷绋课旀担鍛婎吙婵犵數濮撮敃銈夊疮椤愩倗鐭?-->
          <div class="relative">
            <button
              @click="toggleMoreMenu"
              class="header-more-menu-button"
              :class="isMoreMenuOpen || isMoreMenuActive
                ? 'header-more-menu-button-active'
                : 'header-more-menu-button-inactive'"
            >
              <span>更多</span>
              <svg class="header-more-menu-icon" :class="{ 'header-more-menu-icon-rotated': isMoreMenuOpen }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
              </svg>
            </button>
            
            <Transition name="dropdown">
              <div v-if="isMoreMenuOpen" class="header-dropdown-menu">
                <div class="header-dropdown-menu-content">
                  <NuxtLink
                    v-for="item in moreNavigationItems"
                    :key="item.path"
                    :to="item.path"
                    @click="closeMoreMenu"
                    class="header-dropdown-menu-item"
                    :class="route.path === item.path
                      ? 'header-dropdown-menu-item-active'
                      : 'header-dropdown-menu-item-inactive'"
                  >
                    <span class="header-dropdown-menu-item-icon">{{ item.icon }}</span>
                    <span>{{ item.title }}</span>
                  </NuxtLink>
                </div>
              </div>
            </Transition>
          </div>
          
          <!-- 闂傚倷鑳堕幊鎾诲触鐎ｎ剙鍨濋幖娣妼绾惧ジ鏌曟繛鐐珔缂佺姵婢橀…鍧楁嚋闂堟稑顫嶉柣?-->
          <NuxtLink
            to="/search"
            class="header-search-button"
            :class="{ 'header-search-button-active': route.path === '/search' }"
            title="搜索"
          >
            <svg class="header-search-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </NuxtLink>

          <!-- 婵犵數鍋為崹鍫曞箰閹间讲鈧箓鎮滄慨鎰ㄥ亾閸屾稓顩烽悗锝庝簽椤︻偊姊洪幖鐐插姌闁告柨鏈幆?-->
          <div class="header-theme-toggle-container">
            <ThemeToggle />
          </div>
        </nav>

        <!-- 缂傚倸鍊风粈渚€藝閹剁瓔鏁嬬憸搴ㄥ箞閵娾晛鐓涢柛鎰典簻瀹曘儱顪冮妶鍡欏缂佽鍊荤槐鐐哄川鐎涙鍘卞┑掳鍊撻悞锔剧矆閸愵喗鍊甸柛顭戝亞閻帡鏌?-->
        <button
          @click="toggleMobileMenu"
          class="header-mobile-menu-button"
          aria-label="打开移动端菜单"
        >
          <svg class="header-mobile-menu-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
          </svg>
        </button>
      </div>

      <!-- 缂傚倸鍊风粈渚€藝閹剁瓔鏁嬬憸搴ㄥ箞閵娾晛鐓涢柛鎰典簻瀹曘儱顪冮妶鍡欏缂佽鍊荤槐鐐哄川鐎涙鍘?-->
      <Transition name="slide-down">
        <div v-if="isMobileMenuOpen" class="header-mobile-menu">
          <nav class="header-mobile-menu-content">
            <NuxtLink
              v-for="item in navigationItems"
              :key="item.path"
              :to="item.path"
              @click="closeMobileMenu"
              class="header-mobile-menu-item"
              :class="route.path === item.path
                ? 'header-mobile-menu-item-active'
                : 'header-mobile-menu-item-inactive'"
            >
              <span class="header-dropdown-menu-item-icon">{{ item.icon }}</span>
              <span>{{ item.title }}</span>
            </NuxtLink>
            
            <NuxtLink
              to="/search"
              @click="closeMobileMenu"
              class="header-mobile-menu-item header-mobile-menu-item-inactive"
            >
              <span class="header-dropdown-menu-item-icon">🔎</span>
              <span>搜索</span>
            </NuxtLink>
          </nav>
        </div>
      </Transition>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
// 闂傚倷绀侀幖顐も偓姘煎弮瀹曟洖鐣烽崶顭戞綗闂佹寧绋戠€氀囧磻閹剧粯鍋ㄩ梻鍫熺☉娴犳﹢姊洪崨濠勬噧婵☆偅绋撳Σ鎰板箳濡も偓缁€瀣亜韫囨挻濯奸柛瀣ㄥ姂濮婃椽骞愭惔锝傛闂佹椿鍓欓妶鎼佸垂閹灛鐔兼嚒閵堝倹鏁?Nuxt 3 闂傚倷鑳堕崢褔銆冩惔銏㈩洸婵犲﹤瀚崣蹇涙煃閸濆嫬鏆婇柛瀣崌閹粙妫冨☉妯圭帛闂備礁鎲￠悷銉ノ涘Δ鍜冪稏闁靛繈鍊栭弲鏌ユ煕濞戝崬鐏犻柣鎾村灴閹鎲撮崟顒€浠╅梺绋垮婢瑰棛妲?
import ThemeToggle from '~/components/layout/ThemeToggle.vue'

// Nuxt 3 闂傚倷鑳堕崢褔銆冩惔銏㈩洸婵犲﹤瀚崣蹇涙煃閸濆嫬鏆婇柛瀣崌閹粙妫冨☉妯圭帛闂?
// @ts-ignore - Nuxt 3 auto-imports
const router = useRouter()
// @ts-ignore - Nuxt 3 auto-imports
const route = useRoute()

// 濠电姷鏁搁崑娑⑺囬銏犵鐎广儱顦粈鍫澝归悡搴ｆ憼闁哄拋鍓氱换娑㈠幢濞嗘劖鐦筧der 缂傚倸鍊搁崐椋庣矆娴ｈ　鍋撳闂寸盎闁宠閰ｆ慨鈧柣娆屽亾闁哥喎鎳橀弻鐔虹磼濡櫣鐟查梻?app.vue 婵犵數鍋為崹鍫曞箹閳哄懎鍌ㄥ┑鍌滎焾閻鐓崶銊︾；婵炲矈浜弻宥堫檨闁告挾鍠栭悰顕€骞囬弶璺紲濠电娀娼уú銊╁Χ娴煎瓨鈷戦柟绋挎捣閳藉鏌ｉ鈧妶鎼佸垂閹灛鐔兼嚒閵堝倹鏁垫繝鐢靛█濞佳兠洪妶澶堚偓鍌炲蓟閵夛妇鍘搁梺鍓插亝缁诲秴螣閳ь剟姊洪棃娑欘棞妞わ箓娼ц灋闁告劦鍠栭～鍛存煏閸繃鍣规鐐差儔濮婃椽宕崟顐患婵犫拃鍌滅煓濠碉繝娼ч～婵嬪箥娴ｉ晲澹曟繛杈剧到閸燁偊藟瀹ュ鐓曢柍杞扮劍閸ゅ洨鈧?

// 濠电姷顣藉Σ鍛村磻閳ь剟鏌涚€ｎ偅灏扮紒缁樼洴瀵爼骞嬮鐐插婵犵鈧啿绾ч柟顔煎€搁悾鐑藉Ψ閳哄倹娅嗛梺鍏煎墯閸ㄦ娊藝娴煎瓨鈷戦柛婵嗗椤ョ偞銇勯妸銉Ч缂佹梻鍠愮换婵嬪礃椤忎焦鐏冨┑鐘垫暩婵敻宕欒ぐ鎺戞辈?
const isDark = computed(() => {
  if (process.client) {
    const html = document.documentElement
    return html.classList.contains('dark') || 
           html.dataset.theme === 'dark' || 
           html.dataset.theme === 'hybrid-super-dark'
  }
  return false
})

// 缂傚倸鍊风粈渚€藝閹剁瓔鏁嬬憸搴ㄥ箞閵娾晛鐓涢柛鎰典簻瀹曘儱顪冮妶鍡欏缂佽鍊荤槐鐐哄川鐎涙鍘卞┑掳鍊撻悞锔剧矆閸績鏀芥い鏃囧Г閸婃劖顨ラ悙鈺佷壕?
const isMobileMenuOpen = ref(false)

// 闂傚倷绀侀幖顐⒚洪敃鈧…鍥р枎閹惧秴娲畷绋课旀担鍛婎吙婵犵數濮撮敃銈夊疮椤愩倗鐭嗛悗锝庡枟閻撶喐淇婇姘变虎闁绘挻鍔欓弻?
const isMoreMenuOpen = ref(false)

// 闂傚倸鍊搁崐鎼佸箠韫囨稑绀夋俊銈呮嫅缂嶆牗绻濋棃娑卞剰閻熸瑱绠撻獮鏍垝閸忓浜鹃柟鎯版瑜版瑩姊绘担渚劸缂佽鐗撳畷鏇熸綇椤愮姳姹?- Logo 闂傚倷鑳剁划顖炲礉濡ゅ懎绠犻柟鎹愵嚙閸氳銇勯弮鍌氫壕濠殿垰銈搁弻宥夊煛娴ｅ憡娈插┑?
let logoClickCount = 0
let logoClickTimer: NodeJS.Timeout | null = null
const SECRET_CLICKS = 5
const SECRET_TIMEOUT = 3000

const handleAvatarHover = () => {
  if (process.client && (window as any).handleAvatarHover) {
    (window as any).handleAvatarHover()
  }
}

const handleLogoClick = (e: MouseEvent) => {
  e.preventDefault()
  e.stopPropagation()
  
  logoClickCount++
  
  if (logoClickTimer) {
    clearTimeout(logoClickTimer)
  }
  
  if (logoClickCount >= SECRET_CLICKS) {
    router.push('/admin/login')
    logoClickCount = 0
    return
  }
  
  logoClickTimer = setTimeout(() => {
    logoClickCount = 0
  }, SECRET_TIMEOUT)
}

// 闂傚倸鍊烽悞锕傤敄濞嗘挸绐楁慨姗嗗劒妤﹁法鐤€婵浜弶绋款渻閵堝棗濮х紒鑼櫕缁﹨銇愰幒鎾嫼?
onMounted(() => {
  const handleKeyPress = (e: KeyboardEvent) => {
    if ((e.ctrlKey && e.shiftKey && e.key === 'A') || (e.ctrlKey && e.key === 'k')) {
      e.preventDefault()
      router.push('/admin/login')
    }
  }
  
  window.addEventListener('keydown', handleKeyPress)
  
  onUnmounted(() => {
    window.removeEventListener('keydown', handleKeyPress)
    if (logoClickTimer) {
      clearTimeout(logoClickTimer)
    }
  })
})

// 婵犵數鍋為崹鍫曞箰鐠囧弬锝夊箳濡炲皷鍋撻崨瀛樻櫆闂佹鍨版禍鎯归敐鍛殭妞ゃ儱绉归弻娑㈠煛娴ｇ懓娅ら梺?
const mainNavigationItems = computed(() => {
  // @ts-ignore - Nuxt 3 auto-imports
  const { isModuleEnabled } = useModuleSystem()
  const isSSR = process.server
  const shouldShowProjects = isSSR || isModuleEnabled('projects')
  const shouldShowBlog = isSSR || isModuleEnabled('blog')

  const items: Array<{ title: string; path: string; icon: string; moduleKey?: string }> = [
    {
      title: '首页',
      path: '/',
      icon: '🏠',
      moduleKey: 'core'
    }
  ]

  if (shouldShowProjects) {
    items.push({
      title: '项目案例',
      path: '/projects',
      icon: '💼',
      moduleKey: 'projects'
    })
  }

  items.push({
      title: '工具产品',
    path: '/tools',
    icon: '🧰',
    moduleKey: 'tools'
  })

  if (shouldShowBlog) {
    items.push({
      title: '内容文章',
      path: '/blog',
      icon: '📝',
      moduleKey: 'blog'
    })
  }

  items.push(
    {
      title: '关于我',
      path: '/about',
      icon: '👋',
      moduleKey: 'core'
    },
    {
      title: '联系合作',
      path: '/contact',
      icon: '🤝',
      moduleKey: 'core'
    }
  )

  return items
})

// 更多菜单项
const moreNavigationItems = computed(() => {
  // @ts-ignore - Nuxt 3 auto-imports
  const { isModuleEnabled } = useModuleSystem()
  const isSSR = process.server
  const shouldShowLife = isSSR || isModuleEnabled('life')
  const shouldShowEnglish = isSSR || isModuleEnabled('english')
  const shouldShowSkills = isSSR || isModuleEnabled('skills')
  const shouldShowDashboard = isSSR || isModuleEnabled('dashboard')
  const shouldShowGame = isSSR || isModuleEnabled('game')
  const shouldShowLab3D = isSSR || isModuleEnabled('lab-3d')

  const items: Array<{ title: string; path: string; icon: string; moduleKey?: string }> = [
    {
      title: 'AI / 智能体',
      path: '/ai',
      icon: '🤖',
      moduleKey: 'ai'
    }
  ]

  if (shouldShowLab3D) {
    items.push({
      title: 'AI 实验室',
      path: '/lab',
      icon: '🧪',
      moduleKey: 'lab-3d'
    })
  }

  if (shouldShowLife) {
    items.push({
      title: '生活',
      path: '/life',
      icon: '☕',
      moduleKey: 'life'
    })
  }

  if (shouldShowEnglish) {
    items.push({
      title: '英语',
      path: '/english',
      icon: '📘',
      moduleKey: 'english'
    })
  }

  if (shouldShowSkills) {
    items.push({
      title: '技能树',
      path: '/skills',
      icon: '🌳',
      moduleKey: 'skills'
    })
  }

  if (shouldShowDashboard) {
    items.push({
      title: '仪表盘',
      path: '/dashboard',
      icon: '📊',
      moduleKey: 'dashboard'
    })
  }

  items.push({
      title: '副业项目',
    path: '/side-projects',
    icon: '🚀',
    moduleKey: 'core'
  })

  if (shouldShowGame) {
    items.push({
      title: '小游戏',
      path: '/game',
      icon: '🎮',
      moduleKey: 'game'
    })
  }

  return items
})

// 所有导航项（用于移动端菜单）
const navigationItems = computed(() => {
  return [...mainNavigationItems.value, ...moreNavigationItems.value]
})

const isActiveRoute = (path: string) => {
  if (!route || !route.path) return false

  if (path === '/') {
    return route.path === '/' || route.path === ''
  }

  const prefixMatchPaths = [
    '/tools',
    '/projects',
    '/blog',
    '/ai',
    '/about',
    '/contact',
    '/lab',
    '/life',
    '/english',
    '/skills',
    '/dashboard',
    '/side-projects',
    '/game'
  ]
  if (prefixMatchPaths.includes(path)) {
    return route.path === path || route.path.startsWith(`${path}/`)
  }

  return route.path === path
}

// 婵犵數濮伴崹鐓庘枖濞戞埃鍋撳鐓庢珝妤犵偛鍟换婵嬪礋閵娿儰澹曟繛杈剧到閸燁偊藟瀹ュ鐓曢柍杞扮劍閸ゅ洭鏌熼鐣屾噰闁诡喚鍏樺鍫曞箰鎼达絾顫熼梻鍌欑劍鐎笛呯矙閹达附鍋嬮柟鎵閸庡秹鏌ｉ弮鍌氬妺閻庢碍宀搁弻锟犲炊閳轰絿锝夋煛閸℃绠婚柡?
const handleNavClick = (_path: string, _event: MouseEvent) => {}

// 濠电姷顣藉Σ鍛村磻閳ь剟鏌涚€ｎ偅宕岄柡?闂傚倷绀侀幖顐⒚洪敃鈧…鍥р枎閹?闂傚倷绀侀崥瀣偓绗涘懎鍨濋悘鐐垫櫕閺嗭箓鏌ｉ弮鍌氬付闁告瑥锕ラ妵鍕冀閵娧屾殹闂佺楠搁敃顏堝蓟濞戙垹鍐€鐟滃酣宕宠ぐ鎺撶厵妞ゆ牗姘ㄦ晶锝夋煏閸ャ劌濮嶆鐐达耿瀹曟ê顔忛鐣屸偓顐︽⒑閼姐倕小閻犫偓閿曞倸鍨傞柟鎯版閺?
const isMoreMenuActive = computed(() => {
  return moreNavigationItems.value.some(item => isActiveRoute(item.path))
})

// 闂傚倷鑳堕…鍫㈡崲閹寸偟绠惧┑鐘蹭迹濞戞埃鍋撻棃娑欐喐闁活厽鎹囬幃褰掑炊閵娿儳绁烽悶姘卞枔缁辨挻鎷呯憴鍕垫闂佹悶鍔嬬徊濠氬Φ閹邦剦鍚嬪璺猴功椤?
const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
}

// 闂傚倷绀侀幉锛勬暜閹烘嚚娲晝閳ь剟鎮鹃悜鑺ュ殤妞ゆ帒鍊婚悡瀣煟閻樺弶澶勭憸鏉垮暢椤ゅ嫮绱撻崒娆戝妽婵☆偄绻樺畷瑙勬媴閸愶缚姹楅悗骞垮劚椤︻垳娑甸埀?
const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

// 闂傚倷绀侀幉锛勬暜閹烘嚚娲晝閳ь剟鎮鹃悜钘夎摕闁靛鍎卞畵鍡涙⒑閸濆嫷妲归柛鈺佺墕铻ｉ悗锝庡枟閻撶娀鏌熺粙鎸庢崳闁活厼锕﹂埀?
const toggleMoreMenu = () => {
  isMoreMenuOpen.value = !isMoreMenuOpen.value
}

// 闂傚倷鑳堕…鍫㈡崲閹寸偟绠惧┑鐘蹭迹濞戙垹绠绘い鏃傚帶瀹撳棝姊洪崫鍕垫Ч闁糕晛鐗嗚灒閻庯綆鍠楅悡鐘绘煙缁嬫寧鎹ｉ柣顓烇功閳?
const closeMoreMenu = () => {
  isMoreMenuOpen.value = false
}

// 闂傚倷鑳剁划顖炲礉濡ゅ懎绠犻柟鎹愵嚙閸氳銇勯弮鍥棄缂佸墎鍋ら弻鈥愁吋鎼粹€崇闂佸憡鏌ｉ崕鐢稿蓟濞戞粎鐤€闁哄倸绨遍弸娆忊攽閳藉棗浜濋柟绋垮暱椤曪綁骞橀鑲╅獓闂佽鎯岄崢浠嬪煝婵傚憡鈷戦柛蹇曞帶椤曟粎绱掗鑹板闁?
onMounted(() => {
  const handleClickOutside = (e: MouseEvent) => {
    const target = e.target as HTMLElement
    // 濠电姷顣藉Σ鍛村磻閳ь剟鏌涚€ｎ偅宕岄柡宀嬬磿娴狅妇鎷犻幓鎺懶戦梻浣侯焾椤戞垹绱炴担鍝ユ殾婵炲棙鍨圭壕鍏间繆椤栨稐鎲鹃柨鏇炲€归悡鏇熸叏濡搫鈷旈柣锝堥哺缁绘繈鍩€椤掑嫷鏁傞柛娑卞灙閺嬫牠鏌ｆ惔顖滅У闁稿瀚伴、鎾斥枎閹惧鍘鹃梺鍦焾濞寸兘鎯屾繝鍕ㄥ亾鐟欏嫭绀堥柛鐘崇墵閻涱喚鈧綆鍠栭崘鈧悷婊冪Т椤曘儵骞樼紒妯煎幗闂佸搫绋侀崜姘辨兜閻愵兙浜滈柡鍐ｅ亾闁告梹鐟ラ悾鐑藉箮閽樺）鈺呮煏閸繃绁╅柛姘噺缁?
    const dropdownMenu = document.querySelector('.header-dropdown-menu')
    const moreButton = document.querySelector('.header-more-menu-button')
    if (dropdownMenu && moreButton) {
      if (!dropdownMenu.contains(target) && !moreButton.contains(target)) {
        isMoreMenuOpen.value = false
      }
    } else if (!target.closest('.relative')) {
      isMoreMenuOpen.value = false
    }
  }
  if (process.client) {
    // 闂佽娴烽崑锝夊磹閺嶎厼纾块柤娴嬫櫆瀹曞弶绻濇繝鍌氬即閹艰揪绲剧€氭岸鏌熺紒妯轰刊婵炶壈椴哥换娑氣偓娑欘焽閻﹥銇勯幋婵嗩仼闁宠閰ｆ慨鈧柕鍫濇閸擃噣姊洪悷鎵憼缂佽鍟村畷鏇㈠籍閸喓鍘遍梺鎸庣箓濞寸兘宕㈠鍛＜閺夊牄鍔庣粻鐐烘煛鐏炶姤顥㈢€规洖缍婇、娆撴偂鎼淬倗銈寸紓鍌氬€烽悞锕傚蓟閵娾晩鏁勯柛娑卞幘閺嗐倝鏌涢埄鍐噭闁哥姴妫濋弻銊╁即濡も偓娴滈箖鎮?
    setTimeout(() => {
      document.addEventListener('click', handleClickOutside)
    }, 100)
    onUnmounted(() => {
      document.removeEventListener('click', handleClickOutside)
    })
  }
})
</script>

<style scoped>
/* Header 闂備浇顕ф绋匡耿闁秴绠犻柟鐐灱閺嬪秹鏌熼悜姗嗘當闁告劏鍋撻柣搴ｆ嚀鐎氼厽绔熼崱娆愬厹闁?*/
.header-container {
  position: fixed !important;
  top: calc(var(--safe-area-top, 0px) + 1rem) !important;
  left: 50% !important;
  transform: translateX(-50%) !important;
  z-index: 10000 !important; /* 闂傚倷绀佸﹢杈╁垝椤栫偛绀夋繛鍡樻尰閸?z-index闂傚倷鐒︾€笛呯矙閹达附鍎斿┑鍌滎焾閻忚櫕淇婇婵嗗惞闁崇懓绉电换婵嬫濞戞瑯妫￠梻濠庡墻閸撶喖寮婚悢鍝勬瀳婵☆垵妗ㄦ竟鏇㈡⒒娴ｅ憡鎯堥柣妤€妫濊棟妞ゆ牜鍋涢崒銊╂煙閹规劕鐓愭い顐ｆ礋閺岋絽螣婢剁鎯堝銈嗘尭閹碱偊鍩?*/
  visibility: visible !important;
  opacity: 1 !important;
  display: block !important;
}

/* 闂傚倷鑳剁划顖炴偡椤栫偛绀堟繛鎴欏灪閸嬬喐绻涢幋娆忕仼缂佺姵鐗楅妵鍕箳瀹ュ棛銈伴梺璋庡倻绐旈柟顔筋殕閹峰懘宕橀…鎴烆棈闂備胶纭堕弲娑㈠箠濡警鍤曢柛濠勫枔閻も偓闂佸搫鍟崐鎼佀夋惔銊﹀€?- 婵犵數鍋犻幓顏嗙礊閳ь剚绻涙径瀣鐎?CSS 闂傚倷绀侀幉锟犳偡閿曞倹鏅濋柕蹇嬪€曢梻?*/
.floating-nav {
  background: var(--color-bg-elevated) !important;
  backdrop-filter: blur(20px) !important;
  -webkit-backdrop-filter: blur(20px) !important;
  border: 1px solid var(--color-border-default) !important;
  border-radius: 1rem !important;
  box-shadow: var(--shadow-md) !important;
  width: calc(100% - 2rem) !important;
  max-width: 1280px !important;
  transition: all 0.3s ease !important;
  color: var(--color-text-main) !important;
  visibility: visible !important;
  opacity: 1 !important;
  display: block !important;
}

/* 闂傚倷绀侀幖顐⑽涘▎鎾扁偓鍐╁緞鐎ｎ剛褰炬繝鐢靛Т閸熶即鍩㈤弬搴撴斀闁绘ê鍟块崵顒勬煛閸♀晛鐏犻柍钘夘樀楠炴﹢骞囨担绋垮婵犵數鍋涢悧濠囧垂瑜版帒鐒垫い鎴ｆ硶缁佸嘲顭块悷鎵ⅵ鐎规洘妞藉畷銊╁级閹存繂鏁?*/
:global(.dark) .floating-nav,
:global([data-theme="dark"]) .floating-nav,
:global([data-theme="hybrid-super-dark"]) .floating-nav {
  background: var(--color-bg-card) !important;
  backdrop-filter: blur(20px) !important;
  -webkit-backdrop-filter: blur(20px) !important;
  border-color: var(--color-border-default) !important;
  box-shadow: var(--shadow-lg) !important;
  color: var(--color-text-main) !important;
  visibility: visible !important;
  opacity: 1 !important;
  display: block !important;
}

/* 缂傚倷鑳堕搹搴ㄥ矗鎼淬劌绐楅柡鍥╁У瀹曞弶鎱ㄥΟ鎸庣【婵懓寮舵穱濠囧Χ閸涱収浠煎銈冨劚缁绘劙鍩ユ径鎰闁归鑳堕崥瀣渻閵堝繒绉┑顔哄€楃划娆愬緞鐏炵浜鹃柨婵嗙凹閹查箖鏌涢妶鍐ㄢ偓婵嬪蓟閵堝绀堥棅顐幘閸戝綊姊虹紒妯煎闁革綇绲介悾宄邦潨閳ь剙鐣烽妸鈺婃晬婵炴垶纰嶅▓鎼佹⒒娴ｅ憡璐℃い锝勭矙瀹曟垿骞樼紒妯煎幐闂佸壊鍋呯换宥呂ｈぐ鎺撶厸閻庯綆浜滈弳鐔兼煃瑜滈崜姘辩矙閹烘ぜ鈧啴宕卞▎鎴濆触婵犵數濮甸懝楣冩偪妤ｅ啯鐓涘璺猴功娴犮垽鎮规担鍦弨闁?*/
:global(.dark) .floating-nav,
:global([data-theme="dark"]) .floating-nav,
:global([data-theme="hybrid-super-dark"]) .floating-nav {
  color: var(--color-text-main) !important;
}

/* 闂佽楠哥紞濠傤焽閼姐倗纾芥慨妯挎硾閻?Logo 闂傚倷绀侀幖顐﹀磹缁嬫５娲Χ婢跺浠?*/
:global(.dark) .floating-nav .text-text-main,
:global([data-theme="dark"]) .floating-nav .text-text-main,
:global([data-theme="hybrid-super-dark"]) .floating-nav .text-text-main,
:global(.dark) .floating-nav a span.text-text-main,
:global([data-theme="dark"]) .floating-nav a span.text-text-main,
:global([data-theme="hybrid-super-dark"]) .floating-nav a span.text-text-main,
:global(.dark) .floating-nav .flex.items-center span,
:global([data-theme="dark"]) .floating-nav .flex.items-center span,
:global([data-theme="hybrid-super-dark"]) .floating-nav .flex.items-center span {
  color: var(--color-text-main, var(--color-bg-card)) !important;
  font-weight: 800 !important;
  opacity: 1 !important;
}

/* 闂備浇顕х花鑲╁緤缂佹鐝堕柛顐犲劚閺勩儵鏌涘☉妯兼憼闁绘搫缍侀弻锝夊箛闂堟稑顫╂繝銏ｆ硾缁夊綊寮婚敓鐘茬＜婵﹩鍘介幃娆撴⒑?- 婵犵數鍋犻幓顏嗙礊閳ь剚绻涙径瀣鐎?CSS 闂傚倷绀侀幉锟犳偡閿曞倹鏅濋柕蹇嬪€曢梻?*/
.floating-nav .text-text-muted {
  color: var(--color-text-muted, var(--color-text-sec)) !important;
}

/* 闂備浇顕х花鑲╁緤缂佹鐝堕柛顐犲劚閺勩儵鏌涘☉妯兼憼闁绘搫缍侀弻锝夊箛闂堟稑顫╂繝銏ｆ硾缁夊綊寮婚悢纰辨晪闁告侗鍘奸～鈺傜箾鐎电啸濠电偐鍋撻梺璇″枓閺呯娀骞婂鍛瀳婵☆垵妗ㄦ竟?*/
.floating-nav a:hover {
  color: var(--color-text-main, var(--color-text-main)) !important;
}

/* 闂傚倷鑳堕幊鎾诲触鐎ｎ剙鍨濋幖娣妼绾惧ジ鏌曟繛鐐珔闁绘劕锕弻锝夊箛闂堟稑顫梺鎼炲妼閵堟悂寮诲☉姗嗙叆闁告劦鐓堝▓顓㈡煟鎼淬劍娑ч柡鍫墴閺佸秹骞囬鈺冨枛瀹曨偊宕熼娑樹壕濠靛倸鎲￠悡娆愵殽閻愯尙浠㈤柣蹇婃櫆缁绘稓鈧稒锕懓璺ㄢ偓?*/
.floating-nav svg {
  color: var(--color-text-main, var(--color-text-main)) !important;
}

/* 闂傚倷绀侀幖顐⒚洪敃鈧…鍥р枎閹惧秴娲畷绋课旀担鍛婎吙婵犵數濮撮敃銈夊疮椤愩倗鐭嗛悗锝庡枟閻撴瑧绱掔€ｎ偒鍎ユ俊顖楀亾闂備胶鎳撻幉鈩冪箾婵犲偆鍤曟い鎺戝閻愬﹦鎲歌箛娑樼?*/
.floating-nav button {
  color: var(--color-text-main, var(--color-text-main)) !important;
}

/* 缂傚倸鍊风粈渚€藝閹剁瓔鏁嬬憸搴ㄥ箞閵娾晛鐓涢柛鎰典簻瀹曘儱顪冮妶鍡欏缂佽鍊荤槐鐐哄川鐎涙鍘卞┑掳鍊撻悞锔剧矆閸愵喗鍊甸柛顭戝亞閻帡鏌″畝瀣暤妞ゃ垺鐟╁畷妤呮嚃閳哄﹥鐤梻?*/
.floating-nav .md\:hidden svg {
  color: var(--color-text-main) !important;
}

/* 闂傚倸鍊搁崐鎼佹偋韫囨稑纾婚柣鎰暩缁憋箓鏌涘┑鍡椻枙婵炲樊浜濋崵鎺楁煏閸繃鍟楅柨鏇炲€归悡娆愩亜閹达絽鍔甸柛蹇撴湰缁绘盯宕奸悢椋庝患闂侀€炲苯鍘哥紒鑼帛缁旂喖宕奸妷銉︽К闂佸憡绋戦悺銊╂偂閿濆鐓熼柟浼存涧婢ь喖顭?- 婵犵數鍋犻幓顏嗙礊閳ь剚绻涙径瀣鐎?CSS 闂傚倷绀侀幉锟犳偡閿曞倹鏅濋柕蹇嬪€曢梻?*/
.floating-nav nav a:not(.header-nav-link-active) {
  background-color: var(--color-bg-elevated) !important;
  color: var(--color-text-muted) !important;
}

/* 濠电姷鏁告慨鐑姐€傛禒瀣；闁规儳顕粻楣冩煠閼圭増纭鹃柛姘辨嚀閳规垿顢欓懞銉モ偓鎰殽閻愨晛浜惧┑鐐舵彧缁插潡宕曢妶澶嬪仼闂侇剙绉甸崐闈浢归敐鍛殭妞ゃ儱绉归弻娑㈠煛娴ｇ懓娈楅梺鍝勭潤閸ャ劌鈧攱銇勯幒鍡椾壕婵?- 婵犵數鍋犻幓顏嗙礊閳ь剚绻涙径瀣鐎殿噮鍋婃俊鍫曞幢濞嗗本鐏冮梻浣告惈鐎氼剛鎹㈤幇鍏绠涘☉娆忎化?*/
.floating-nav nav a.header-nav-link-active {
  background-color: var(--color-primary-hover) !important;
  color: var(--color-primary) !important;
}

/* 闂佽瀛╅鏍窗閺嶎厼绠规い鎰剁畱閺勩儲淇婇妶鍛櫣缂佺姰鍎甸弻宥堫檨闁告挾鍠庨?span 闂傚倷绀侀幖顐﹀磹缁嬫５娲Χ婢跺浠遍悷婊冮叄楠炲繘宕ㄩ娑樼／闂侀潧顭梽鍕枔?CSS 闂傚倷绀侀幉锟犳偡閿曞倹鏅濋柕蹇嬪€曢梻?*/
.floating-nav span {
  color: var(--color-text-main) !important;
  opacity: 1 !important;
}

/* Logo 闂傚倷绀侀幖顐﹀磹缁嬫５娲Χ婢跺浠?- 婵犵數鍋犻幓顏嗙礊閳ь剚绻涙径瀣鐎?CSS 闂傚倷绀侀幉锟犳偡閿曞倹鏅濋柕蹇嬪€曢梻?*/
.floating-nav .logo-text {
  color: var(--color-text-main) !important;
  font-weight: 700 !important;
  opacity: 1 !important;
  text-shadow: none !important;
  -webkit-font-smoothing: antialiased !important;
  -moz-osx-font-smoothing: grayscale !important;
}

/* 缂傚倷鑳堕搹搴ㄥ矗鎼淬劌绐楅柡鍥╁У瀹曞弶鎱ㄥΟ鎸庣【闁绘劕锕弻锝夊箛闂堟稑顫梺?emoji 婵犵數鍋為崹璺侯潖婵犳艾绐楅柟閭﹀枤閸楁艾鈹戦悩瀹犲闁汇倝绠栭弻鈩冨緞婵犲嫪铏庨柣鐘辩劍閻擄繝骞?*/
.floating-nav .mr-1\.5 {
  color: var(--color-text-main) !important;
}

/* 闂傚倷绀侀幉锛勬崲閸屾粎鐭撻悗鍨摃婵娊鏌熺紒妯轰刊闁搞倖娲熼悡顐﹀炊閵夈儱濮㈢紓浣瑰姈椤ㄥ﹪骞冨Ο璺ㄧ杸闁规儳澧庨鍥⒑濞茶浜滄俊顐ｇ箞閹即顢氶埀顒€顕ｉ幘顔藉亜闁告挸寮惰ⅶ闂傚倷鑳堕…鍫㈡崲閸儱绀夌€光偓閸曞灚鏅?*/
@media (max-width: 640px) {
  .floating-nav {
    width: calc(100% - 1rem);
    top: calc(var(--safe-area-top, 0px) + 0.5rem);
    border-radius: 0.75rem;
  }
}

/* 婵犵數鍋為崹鍫曞箰閹间緡鏁勯柛鈩兠崹鏃€绻涘顔荤凹闁搞倖娲樼换婵囩節閸屾碍娈紓浣插亾閻庯綆鍠楅悡鏇㈡煛閸屾繍鍤欏┑鈥虫健閺?*/
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from {
  opacity: 0 !important;
  transform: translateY(-10px);
}

.dropdown-enter-to {
  opacity: 1 !important;
  transform: translateY(0);
}

.dropdown-leave-from {
  opacity: 1 !important;
  transform: translateY(0);
}

.dropdown-leave-to {
  opacity: 0 !important;
  transform: translateY(-10px);
}

/* 缂傚倸鍊风粈渚€藝閹剁瓔鏁嬬憸搴ㄥ箞閵娾晛鐓涢柛鎰典簻瀹曘儱顪冮妶鍡欏缂佽鍊荤槐鐐哄川鐎涙鍘卞┑掳鍊撻悞锔剧矆閳ь剚绻濆▓鍨灀闁哄懐濞€楠?*/
.slide-down-enter-active,
.slide-down-leave-active {
  transition: all 0.3s ease;
}

.slide-down-enter-from {
  opacity: 0;
  transform: translateY(-10px);
}

.slide-down-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

.header-content-wrapper {
  position: relative;
}

.floating-nav {
  background:
    linear-gradient(135deg, rgba(10, 15, 29, 0.84), rgba(19, 28, 48, 0.72)) !important;
  border: 1px solid rgba(255, 255, 255, 0.08) !important;
  box-shadow:
    0 28px 60px rgba(7, 12, 24, 0.24),
    inset 0 1px 0 rgba(255, 255, 255, 0.08) !important;
}

.floating-nav::before {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: inherit;
  background:
    radial-gradient(circle at left top, rgba(120, 119, 198, 0.18), transparent 34%),
    radial-gradient(circle at right top, rgba(56, 189, 248, 0.12), transparent 30%);
  pointer-events: none;
}

.floating-nav::after {
  content: '';
  position: absolute;
  inset: 1px;
  border-radius: calc(1rem - 1px);
  border: 1px solid rgba(255, 255, 255, 0.04);
  pointer-events: none;
}

.header-main {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1.5rem;
  min-height: 4.75rem;
  padding: 0.75rem 1.1rem;
}

.header-logo-link {
  display: inline-flex;
  align-items: center;
  gap: 0.9rem;
  min-width: 0;
}

.header-logo-avatar {
  position: relative;
  overflow: hidden;
  width: 2.9rem;
  height: 2.9rem;
  border-radius: 1rem;
  border: 1px solid rgba(255, 255, 255, 0.14);
  box-shadow:
    0 18px 32px rgba(15, 23, 42, 0.24),
    inset 0 1px 0 rgba(255, 255, 255, 0.12);
}

.header-logo-avatar::after {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.14), transparent 58%);
  pointer-events: none;
}

.header-logo-text {
  position: relative;
  display: inline-flex;
  flex-direction: column;
  gap: 0.2rem;
  font-size: 1.02rem;
  letter-spacing: 0.08em;
}

.header-logo-text::after {
  content: 'SYSTEMS · AI · TOOLS';
  font-size: 0.56rem;
  letter-spacing: 0.28em;
  color: rgba(226, 232, 240, 0.62);
  font-weight: 600;
}

.header-nav-desktop {
  position: relative;
  display: flex;
  align-items: center;
  gap: 0.65rem;
  padding: 0.45rem;
  border-radius: 999px;
  background: rgba(9, 14, 26, 0.38);
  border: 1px solid rgba(255, 255, 255, 0.06);
  box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.05);
}

.header-nav-link,
.header-more-menu-button,
.header-search-button {
  position: relative;
  border-radius: 999px;
  border: 1px solid transparent;
  transition:
    transform 0.25s ease,
    border-color 0.25s ease,
    background-color 0.25s ease,
    box-shadow 0.25s ease,
    color 0.25s ease;
}

.header-nav-link {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  min-height: 3rem;
  padding: 0.72rem 0.9rem;
}

.header-nav-link-inactive,
.header-more-menu-button-inactive,
.header-search-button {
  background: rgba(255, 255, 255, 0.02);
}

.header-nav-link-inactive:hover,
.header-more-menu-button-inactive:hover,
.header-search-button:hover {
  transform: translateY(-1px);
  border-color: rgba(148, 163, 184, 0.2);
  background: rgba(255, 255, 255, 0.06);
  box-shadow: 0 14px 24px rgba(8, 15, 29, 0.18);
}

.header-nav-link-active,
.header-more-menu-button-active,
.header-search-button-active {
  background: linear-gradient(135deg, rgba(56, 189, 248, 0.18), rgba(129, 140, 248, 0.2));
  border-color: rgba(125, 211, 252, 0.3);
  box-shadow:
    0 16px 28px rgba(8, 15, 29, 0.2),
    inset 0 1px 0 rgba(255, 255, 255, 0.12);
}

.header-nav-link-icon {
  flex: 0 0 auto;
  opacity: 0.84;
}

.header-nav-link-label,
.header-more-menu-button > span {
  display: inline-block;
  white-space: nowrap;
  word-break: keep-all;
  line-height: 1;
  font-size: 0.92rem;
  letter-spacing: 0.01em;
}

.header-search-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: auto;
  min-width: 2.9rem;
  padding-inline: 0.85rem;
}

.header-search-button::after {
  content: '/';
  font-size: 0.72rem;
  font-weight: 700;
  line-height: 1;
  color: rgba(226, 232, 240, 0.58);
}

.header-search-button:hover::after,
.header-search-button-active::after {
  color: rgba(255, 255, 255, 0.9);
}

.header-theme-toggle-container {
  margin-left: 0.15rem;
  padding-left: 0.3rem;
  border-left: 1px solid rgba(255, 255, 255, 0.08);
}

.header-more-menu-button {
  display: inline-flex;
  align-items: center;
  gap: 0.42rem;
  min-height: 3rem;
  padding: 0.72rem 0.95rem;
  white-space: nowrap;
}

.header-dropdown-menu {
  padding-top: 0.8rem;
}

.header-dropdown-menu-content,
.header-mobile-menu {
  background: rgba(10, 15, 29, 0.92);
  backdrop-filter: blur(24px);
  -webkit-backdrop-filter: blur(24px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  box-shadow: 0 24px 48px rgba(7, 12, 24, 0.28);
}

.header-mobile-menu {
  position: relative;
  z-index: 1;
  margin: 0 1rem 0.5rem;
  border-radius: 1.1rem;
  overflow: hidden;
}

.header-mobile-menu-content {
  padding: 0.75rem;
}

.header-mobile-menu-item {
  border-radius: 0.9rem;
}

.header-logo-text::after {
  content: 'SYSTEMS / AI / TOOLS';
}

@media (max-width: 960px) {
  .header-main {
    min-height: 4.25rem;
    padding: 0.65rem 0.85rem;
  }

  .header-logo-text::after {
    display: none;
  }
}

@media (min-width: 961px) and (max-width: 1360px) {
  .header-main {
    gap: 1rem;
  }

  .header-logo-link {
    gap: 0.75rem;
  }

  .header-logo-text {
    font-size: 0.95rem;
  }

  .header-logo-text::after {
    font-size: 0.5rem;
    letter-spacing: 0.22em;
  }

  .header-nav-desktop {
    gap: 0.45rem;
    padding: 0.35rem;
  }

  .header-nav-link,
  .header-more-menu-button {
    min-height: 2.85rem;
    padding: 0.68rem 0.78rem;
  }

  .header-nav-link-label,
  .header-more-menu-button > span {
    font-size: 0.86rem;
  }

  .header-search-button {
    min-width: 2.7rem;
    padding-inline: 0.72rem;
  }
}
</style>
