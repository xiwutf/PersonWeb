<template>
  <div class="min-h-screen bg-slate-50 overflow-x-hidden selection:bg-blue-500/30">
    <!-- 添加到主屏幕提示 -->
    <AddToHomeScreen />

    <!-- 全局噪点纹理 -->
    <div
      class="fixed inset-0 pointer-events-none opacity-[0.03] z-50 mix-blend-overlay"
      style="background-image: url(&quot;data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E&quot;);"
    ></div>

    <!-- Hero Banner -->
    <section class="relative min-h-[100vh] flex items-center justify-center overflow-hidden bg-[#020617]">
      <!-- 流体背景（弱化一点，做氛围） -->
      <FluidBackground :intensity="0.2" color="#3b82f6" :interactive="true" />

      <!-- 沉浸式3D背景（移动端禁用由组件内部控制） -->
      <div class="absolute inset-0 z-0">
        <Immersive3DScene :show-hint="false" :scroll-enabled="true" />
      </div>

      <!-- 背景光斑 -->
      <div class="absolute inset-0 overflow-hidden pointer-events-none z-[1]">
        <div class="absolute -top-32 -left-32 w-[45%] h-[45%] bg-blue-600/25 rounded-full mix-blend-screen blur-[110px] animate-blob"></div>
        <div class="absolute -top-10 right-[-15%] w-[40%] h-[40%] bg-purple-600/25 rounded-full mix-blend-screen blur-[110px] animate-blob animation-delay-2000"></div>
        <div class="absolute bottom-[-20%] left-[15%] w-[40%] h-[40%] bg-cyan-500/25 rounded-full mix-blend-screen blur-[110px] animate-blob animation-delay-4000"></div>

        <!-- 网格背景 -->
        <div class="absolute inset-0 bg-[url('/images/grid.svg')] bg-center [mask-image:linear-gradient(180deg,white,rgba(255,255,255,0))] opacity-20"></div>
      </div>

      <!-- 主内容 -->
      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 w-full z-10">
        <div class="grid lg:grid-cols-2 gap-12 items-center">
          <!-- 左侧文案区 -->
          <div class="text-center lg:text-left space-y-8" data-aos="fade-right">
            <!-- 顶部小标签 -->
            <div
              class="inline-flex items-center px-4 py-2 bg-white/5 backdrop-blur-md rounded-full border border-white/10 mb-2 animate-float hover:bg-white/10 transition-colors cursor-default"
            >
              <span class="flex h-2 w-2 relative mr-3">
                <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75"></span>
                <span class="relative inline-flex rounded-full h-2 w-2 bg-emerald-500"></span>
              </span>
              <span class="text-blue-100 font-medium text-xs sm:text-sm">欢迎来到我的数字花园</span>
            </div>

            <!-- 主标题 -->
            <h1
              class="text-3xl sm:text-4xl md:text-5xl lg:text-6xl xl:text-7xl font-bold text-white tracking-tight leading-tight"
            >
              你好，我是
              <span
                class="block mt-2 text-transparent bg-clip-text bg-gradient-to-r from-blue-400 via-purple-400 to-cyan-300 animate-gradient-x"
              >
                溪午听风
              </span>
            </h1>

            <!-- 角色轮播 -->
            <div
              class="h-9 sm:h-10 md:h-12 text-lg sm:text-xl lg:text-2xl text-slate-400 font-medium flex items-center justify-center lg:justify-start overflow-hidden"
            >
              <span class="mr-2 sm:mr-3 text-slate-400">我是一名</span>
              <div class="relative h-full w-56 sm:w-64 text-left">
                <transition-group name="slide-up" tag="div" class="absolute top-0 left-0 w-full">
                  <span
                    v-if="currentRoleIndex === 0"
                    key="0"
                    class="block font-semibold text-blue-300"
                  >
                    全栈开发者
                  </span>
                  <span
                    v-if="currentRoleIndex === 1"
                    key="1"
                    class="block font-semibold text-purple-300"
                  >
                    AI 应用探索者
                  </span>
                  <span
                    v-if="currentRoleIndex === 2"
                    key="2"
                    class="block font-semibold text-cyan-300"
                  >
                    Revit 插件实践者
                  </span>
                  <span
                    v-if="currentRoleIndex === 3"
                    key="3"
                    class="block font-semibold text-orange-200"
                  >
                    终身学习者
                  </span>
                </transition-group>
              </div>
            </div>

            <!-- 简介文案 -->
            <p
              class="text-sm sm:text-base md:text-lg text-slate-300 max-w-xl mx-auto lg:mx-0 leading-relaxed"
            >
              专注构建高效、优雅的数字体验。
              无论是企业级业务系统、AI 驱动应用，还是有趣的小工具，我都致力于把复杂问题做得简单好用。
            </p>

            <!-- 个人标签 -->
            <div
              class="flex flex-wrap justify-center lg:justify-start gap-2 pt-1 text-xs sm:text-sm text-slate-300"
            >
              <span class="px-3 py-1 rounded-full bg-white/5 border border-white/10">前后端一体化</span>
              <span class="px-3 py-1 rounded-full bg-white/5 border border-white/10">AI 应用落地</span>
              <span class="px-3 py-1 rounded-full bg-white/5 border border-white/10">自动化效率提升</span>
            </div>

            <!-- 按钮区 -->
            <div
              class="flex flex-wrap justify-center lg:justify-start gap-3 sm:gap-4 pt-4"
            >
              <NuxtLink to="/projects" class="relative inline-flex group touch-target">
                <div
                  class="absolute transition-all duration-700 opacity-70 -inset-px bg-gradient-to-r from-[#44BCFF] via-[#FF44EC] to-[#FF675E] rounded-xl blur-lg group-hover:opacity-100 group-hover:-inset-1 group-hover:duration-200 animate-tilt"
                ></div>
                <button
                  class="relative inline-flex items-center justify-center px-6 sm:px-7 md:px-8 py-3 sm:py-3.5 md:py-4 text-sm sm:text-base md:text-lg font-semibold text-white transition-all duration-200 bg-slate-950/80 rounded-xl focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-900/80 min-h-[44px]"
                >
                  浏览项目
                  <i
                    class="fas fa-arrow-right ml-2 group-hover:translate-x-1 transition-transform"
                  ></i>
                </button>
              </NuxtLink>

              <NuxtLink
                to="/about"
                class="inline-flex items-center justify-center px-6 sm:px-7 md:px-8 py-3 sm:py-3.5 md:py-4 text-sm sm:text-base md:text-lg font-semibold text-slate-200 transition-all duration-200 bg-white/5 border border-slate-600/70 rounded-xl hover:bg-white/10 hover:border-slate-400/80 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-900/80 min-h-[44px] touch-target"
              >
                关于我
              </NuxtLink>
            </div>
          </div>

          <!-- 右侧视觉区 -->
          <div class="relative hidden lg:block" data-aos="fade-left">
            <div class="relative w-full max-w-sm mx-auto aspect-square">
              <!-- 柔和渐变背景块 -->
              <div
                class="absolute inset-0 bg-gradient-to-tr from-blue-500/60 via-purple-500/60 to-cyan-400/60 rounded-[2.1rem] rotate-6 opacity-40 blur-2xl"
              ></div>

              <GlassCard class="w-full h-full rounded-[2rem] overflow-hidden border border-white/15">
                <div class="w-full h-full relative bg-slate-900/40">
                  <!-- 粒子头像 -->
                  <ParticleAvatar
                    :image-url="'/images/avatar.jpg'"
                    :text="'溪午听风'"
                    :subtitle="'Full-stack & AI Developer'"
                    :particle-count="2000"
                    :interactive="true"
                    class="w-full h-full"
                  />

                  <!-- 底部信息条 -->
                  <div
                    class="absolute bottom-5 left-5 right-5 p-4 bg-slate-900/80 backdrop-blur-xl rounded-xl border border-white/15 shadow-2xl"
                  >
                    <div class="flex items-center justify-between gap-3">
                      <div class="flex-1">
                        <h3 class="font-semibold text-white text-base mb-0.5">溪午听风</h3>
                        <p class="text-xs text-slate-300">
                          全栈开发 · AI 应用 · Revit 插件
                        </p>
                      </div>
                      <div
                        class="w-11 h-11 bg-gradient-to-br from-blue-500/40 to-purple-500/40 rounded-full flex items-center justify-center text-blue-100 text-lg border border-white/30 shadow-lg"
                      >
                        <i class="fas fa-code"></i>
                      </div>
                    </div>
                    <div class="mt-3 pt-2 border-t border-white/10">
                      <div class="flex flex-wrap gap-1.5 text-[11px] text-slate-100">
                        <span
                          class="px-2 py-0.5 rounded-full bg-blue-500/25 border border-blue-300/40"
                        >
                          Vue · Nuxt
                        </span>
                        <span
                          class="px-2 py-0.5 rounded-full bg-emerald-500/20 border border-emerald-300/40"
                        >
                          .NET · Node.js
                        </span>
                        <span
                          class="px-2 py-0.5 rounded-full bg-fuchsia-500/20 border border-fuchsia-300/40"
                        >
                          AI 工具链
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </GlassCard>

              <!-- 悬浮卡片：项目进度 -->
              <div
                class="absolute -right-6 top-12 p-3 bg-slate-900/85 backdrop-blur-xl rounded-2xl shadow-xl border border-white/12 animate-float-delayed w-40 z-20"
              >
                <div class="flex items-center gap-2 mb-2">
                  <div
                    class="w-7 h-7 bg-emerald-500/15 rounded-lg flex items-center justify-center text-emerald-300 text-xs border border-emerald-400/40"
                  >
                    <i class="fas fa-check"></i>
                  </div>
                  <span class="font-semibold text-xs text-slate-100">个人站点已上线</span>
                </div>
                <div class="w-full bg-slate-800 rounded-full h-1.5 overflow-hidden">
                  <div
                    class="bg-emerald-400 h-1.5 rounded-full w-full shadow-[0_0_12px_rgba(74,222,128,0.6)]"
                  ></div>
                </div>
                <p class="mt-1.5 text-[10px] text-slate-400">持续迭代功能与体验</p>
              </div>

              <!-- 悬浮卡片：AI 驱动 -->
              <div
                class="absolute -left-6 bottom-24 p-3 bg-slate-900/85 backdrop-blur-xl rounded-2xl shadow-xl border border-white/12 animate-float w-40 z-20"
              >
                <div class="flex items-center gap-2">
                  <div
                    class="w-7 h-7 bg-purple-500/15 rounded-lg flex items-center justify-center text-purple-300 text-xs border border-purple-400/40"
                  >
                    <i class="fas fa-sparkles"></i>
                  </div>
                  <div>
                    <div class="font-semibold text-xs text-slate-100">AI 驱动体验</div>
                    <div class="text-[10px] text-slate-400">智能助手 · 智能工具</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 滚动提示 -->
        <div
          class="absolute bottom-10 left-1/2 -translate-x-1/2 cursor-pointer group"
          @click="scrollToContent"
        >
          <div class="flex flex-col items-center gap-2">
            <span
              class="text-[10px] tracking-[0.2em] text-slate-500 uppercase group-hover:text-blue-400 transition-colors"
            >
              Scroll
            </span>
            <div
              class="w-5 h-8 border-2 border-slate-600 rounded-full flex justify-center p-1 group-hover:border-blue-400 transition-colors"
            >
              <div
                class="w-1 h-2 bg-slate-400 rounded-full animate-scroll-down group-hover:bg-blue-400"
              ></div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 成长轨迹时间线 -->
    <Timeline />

    <!-- 3D 互动场景 -->
    <section
      class="py-16 bg-gradient-to-b from-[#020617] via-slate-900 to-slate-950 relative overflow-hidden"
    >
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-12" data-aos="fade-up">
          <h2 class="text-3xl lg:text-4xl font-bold text-white mb-3">互动探索</h2>
          <p class="text-base sm:text-lg text-slate-400">
            点击不同 3D 对象，进入博客、项目与数据世界
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <!-- 博客 -->
          <div
            class="bg-slate-900/70 backdrop-blur-xl rounded-2xl p-6 border border-slate-700/80 hover:border-blue-500/60 transition-all"
          >
            <h3 class="text-white font-semibold mb-4 text-center text-sm sm:text-base">
              🌍 博客文章
            </h3>
            <Scene3D type="earth" :show-hint="false" height="300px" />
            <p class="text-slate-400 text-xs sm:text-sm text-center mt-4">
              点击地球，查看技术与思考
            </p>
          </div>

          <!-- 项目 -->
          <div
            class="bg-slate-900/70 backdrop-blur-xl rounded-2xl p-6 border border-slate-700/80 hover:border-purple-500/60 transition-all"
          >
            <h3 class="text-white font-semibold mb-4 text-center text-sm sm:text-base">
              🚀 项目作品
            </h3>
            <Scene3D type="spaceship" :show-hint="false" height="300px" />
            <p class="text-slate-400 text-xs sm:text-sm text-center mt-4">
              点击飞船，查看完整项目集
            </p>
          </div>

          <!-- 数据仪表盘 -->
          <div
            class="bg-slate-900/70 backdrop-blur-xl rounded-2xl p-6 border border-slate-700/80 hover:border-emerald-500/60 transition-all"
          >
            <h3 class="text-white font-semibold mb-4 text-center text-sm sm:text-base">
              💎 数据仪表盘
            </h3>
            <Scene3D type="datasphere" :show-hint="false" height="300px" />
            <p class="text-slate-400 text-xs sm:text-sm text-center mt-4">
              点击数据星球，查看站点数据
            </p>
          </div>
        </div>
      </div>
    </section>

    <!-- Bento Grid 内容展示 -->
    <section id="content" class="py-24 bg-slate-50 relative">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16" data-aos="fade-up">
          <h2 class="text-3xl lg:text-4xl font-bold text-slate-900 mb-3">探索我的世界</h2>
          <p class="text-lg text-slate-600">这里有代码、有思考，也有生活</p>
        </div>

        <div
          class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 auto-rows-[minmax(200px,auto)]"
        >
          <!-- 最新博客 -->
          <div class="md:col-span-2 lg:col-span-2 row-span-2" data-aos="fade-up">
            <TiltCard class="h-full">
              <div
                class="group relative h-full overflow-hidden rounded-3xl shadow-md bg-white p-8 flex flex-col justify-between border border-slate-200 hover:border-blue-200 hover:shadow-xl transition-all duration-300"
              >
                <div
                  class="absolute inset-0 bg-gradient-to-br from-blue-50 via-sky-50 to-indigo-50 opacity-0 group-hover:opacity-100 transition-opacity duration-500"
                ></div>

                <div class="relative z-10">
                  <div
                    class="inline-flex items-center px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-xs sm:text-sm mb-4 font-medium"
                  >
                    <i class="fas fa-pen-fancy mr-2"></i> 最新博客
                  </div>
                  <h3
                    class="text-2xl sm:text-3xl font-bold text-slate-900 mb-2 group-hover:text-blue-700 transition-colors"
                  >
                    技术探索与分享
                  </h3>
                  <p class="text-slate-600 text-sm sm:text-base">
                    记录学习过程中的洞察，与解决问题的完整思路。
                  </p>
                </div>

                <div class="mt-8 space-y-3 relative z-10">
                  <div v-if="latestPosts && latestPosts.length > 0">
                    <NuxtLink
                      v-for="post in latestPosts"
                      :key="post.id"
                      :to="`/blog/${post.slug || post.id}`"
                      class="block p-4 bg-white rounded-xl shadow-sm border border-slate-100 hover:shadow-md hover:border-blue-200 transition-all group/item mb-2"
                    >
                      <div class="flex justify-between items-start gap-2">
                        <div
                          class="font-semibold text-slate-800 text-sm sm:text-base group-hover/item:text-blue-600 transition-colors truncate pr-4"
                        >
                          {{ post.title }}
                        </div>
                        <i
                          class="fas fa-arrow-right text-slate-300 group-hover/item:text-blue-500 transform group-hover/item:translate-x-1 transition-all"
                        ></i>
                      </div>
                      <div class="text-xs text-slate-500 mt-1">
                        {{ formatDate(post.publishTime || post.createdAt) }} ·
                        {{ post.categoryName || '未分类' }}
                      </div>
                    </NuxtLink>
                  </div>
                  <div v-else class="text-center text-slate-500 py-4 text-sm">
                    暂无最新文章，敬请期待更新。
                  </div>
                </div>
              </div>
            </TiltCard>
          </div>

          <!-- 工具箱 -->
          <div class="md:col-span-1 lg:col-span-1 row-span-1" data-aos="fade-up" data-aos-delay="80">
            <TiltCard class="h-full">
              <div
                class="h-full bg-white rounded-3xl shadow-md p-6 border border-slate-200 group hover:border-blue-200 hover:shadow-xl transition-all duration-300 flex flex-col relative overflow-hidden"
              >
                <div
                  class="absolute -right-6 -top-6 w-20 h-20 bg-blue-50 rounded-full group-hover:scale-150 transition-transform duration-500"
                ></div>

                <div class="relative z-10 flex flex-col h-full">
                  <div
                    class="w-12 h-12 bg-blue-100 rounded-2xl flex items-center justify-center text-blue-600 text-xl mb-4 group-hover:rotate-6 transition-transform shadow-sm"
                  >
                    <i class="fas fa-tools"></i>
                  </div>
                  <h3 class="text-lg sm:text-xl font-bold text-slate-800 mb-2">效率工具</h3>
                  <p class="text-sm text-slate-500 mb-4 flex-grow">
                    Revit 插件与自动化脚本，让日常工作更顺滑高效。
                  </p>
                  <NuxtLink
                    to="/tools"
                    class="text-blue-600 font-medium text-xs sm:text-sm hover:underline flex items-center mt-auto group/link"
                  >
                    查看工具库
                    <i
                      class="fas fa-arrow-right ml-1 text-xs group-hover/link:translate-x-1 transition-transform"
                    ></i>
                  </NuxtLink>
                </div>
              </div>
            </TiltCard>
          </div>

          <!-- 项目展示 -->
          <div class="md:col-span-1 lg:col-span-1 row-span-1" data-aos="fade-up" data-aos-delay="160">
            <TiltCard class="h-full">
              <div
                class="h-full bg-slate-900 rounded-3xl shadow-md p-6 text-white overflow-hidden relative group flex flex-col border border-slate-800 hover:border-purple-500/60 hover:shadow-xl transition-all"
              >
                <div
                  class="absolute top-0 right-0 w-28 h-28 bg-purple-500/25 rounded-full blur-2xl -mr-10 -mt-10 group-hover:bg-purple-500/35 transition-colors"
                ></div>
                <div
                  class="absolute bottom-0 left-0 w-24 h-24 bg-blue-500/20 rounded-full blur-xl -ml-10 -mb-10 group-hover:bg-blue-500/30 transition-colors"
                ></div>

                <div class="relative z-10 flex flex-col h-full">
                  <div
                    class="w-12 h-12 bg-white/10 rounded-2xl flex items-center justify-center text-purple-200 text-xl mb-4 backdrop-blur-sm border border-white/10 group-hover:scale-110 transition-transform"
                  >
                    <i class="fas fa-project-diagram"></i>
                  </div>
                  <h3 class="text-lg sm:text-xl font-bold mb-2">精选项目</h3>
                  <p class="text-sm text-slate-300 mb-4 flex-grow">
                    实战项目与开源尝试，持续拓展技术边界。
                  </p>
                  <NuxtLink
                    to="/projects"
                    class="text-purple-200 font-medium text-xs sm:text-sm hover:text-purple-100 flex items-center mt-auto group/link"
                  >
                    浏览作品集
                    <i
                      class="fas fa-arrow-right ml-1 text-xs group-hover/link:translate-x-1 transition-transform"
                    ></i>
                  </NuxtLink>
                </div>
              </div>
            </TiltCard>
          </div>

          <!-- 生活随笔 -->
          <div class="md:col-span-2 lg:col-span-2 row-span-1" data-aos="fade-up" data-aos-delay="240">
            <TiltCard class="h-full">
              <div
                class="h-full bg-gradient-to-r from-orange-50 via-amber-50 to-rose-50 rounded-3xl shadow-md p-6 border border-orange-100 flex flex-col md:flex-row items-center gap-6 group hover:shadow-xl transition-all duration-300"
              >
                <div class="flex-1">
                  <div
                    class="w-12 h-12 bg-orange-100 rounded-2xl flex items-center justify-center text-orange-600 text-xl mb-4 shadow-sm group-hover:scale-110 transition-transform"
                  >
                    <i class="fas fa-coffee"></i>
                  </div>
                  <h3 class="text-lg sm:text-xl font-bold text-slate-800 mb-2">生活随笔</h3>
                  <p class="text-sm text-slate-600 mb-4">
                    记录代码之外的风景，用文字和图片保存一些温度。
                  </p>
                  <NuxtLink
                    to="/life"
                    class="inline-flex items-center px-5 py-2.5 bg-orange-500 text-white rounded-xl text-sm font-medium hover:bg-orange-600 transition-all shadow-lg shadow-orange-500/30 hover:shadow-orange-500/40 hover:-translate-y-0.5"
                  >
                    进入生活专栏
                  </NuxtLink>
                </div>
                <div class="w-full md:w-1/2 space-y-3">
                  <div
                    class="p-3 bg-white rounded-xl shadow-sm border border-orange-100/70 flex items-center gap-3 hover:shadow-md transition-shadow cursor-pointer"
                  >
                    <div class="w-12 h-12 bg-gray-100 rounded-lg flex-shrink-0 overflow-hidden">
                      <img
                        src="/images/blog/thermal-circulation.png"
                        class="w-full h-full object-cover transform hover:scale-110 transition-transform duration-500"
                        alt="cover"
                      />
                    </div>
                    <div>
                      <div
                        class="text-[10px] text-orange-500 font-bold uppercase tracking-wider mb-0.5"
                      >
                        最近更新
                      </div>
                      <div
                        class="font-medium text-slate-700 text-sm line-clamp-1 group-hover:text-orange-600 transition-colors"
                      >
                        地理科普：一文读懂热力环流
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </TiltCard>
          </div>

          <!-- 关于我 / 联系方式 -->
          <div class="md:col-span-2 lg:col-span-2 row-span-1" data-aos="fade-up" data-aos-delay="320">
            <TiltCard class="h-full">
              <div
                class="h-full bg-white rounded-3xl shadow-md p-8 border border-slate-200 flex flex-col justify-center items-center text-center gap-4 group hover:border-blue-200 hover:shadow-xl transition-all relative overflow-hidden"
              >
                <div
                  class="absolute inset-0 bg-slate-50 opacity-0 group-hover:opacity-100 transition-opacity duration-500 pointer-events-none"
                ></div>

                <div class="relative z-10">
                  <h3 class="text-2xl font-bold text-slate-800 mb-2">想要进一步了解我？</h3>
                  <p class="text-slate-600 text-sm sm:text-base max-w-md mx-auto">
                    欢迎一起聊聊技术、产品、AI 应用落地，或者单纯交个朋友。
                  </p>
                </div>
                <div class="flex gap-4 mt-2 relative z-10">
                  <a
                    href="https://github.com"
                    target="_blank"
                    class="w-10 h-10 bg-slate-100 rounded-full flex items-center justify-center text-slate-700 hover:bg-slate-900 hover:text-white transition-all duration-300 hover:-translate-y-1"
                  >
                    <i class="fab fa-github"></i>
                  </a>
                  <a
                    href="mailto:contact@example.com"
                    class="w-10 h-10 bg-blue-50 rounded-full flex items-center justify-center text-blue-600 hover:bg-blue-600 hover:text-white transition-all duration-300 hover:-translate-y-1"
                  >
                    <i class="fas fa-envelope"></i>
                  </a>
                  <NuxtLink
                    to="/about"
                    class="px-6 py-2 bg-slate-900 text-white rounded-full text-sm font-medium hover:bg-slate-800 transition-all shadow-lg shadow-slate-900/20 hover:shadow-slate-900/40 hover:-translate-y-1"
                  >
                    查看详细介绍
                  </NuxtLink>
                </div>
              </div>
            </TiltCard>
          </div>
        </div>
      </div>
    </section>

    <!-- 时间胶囊墙展示区域 -->
    <section class="py-24 bg-gradient-to-b from-slate-50 to-white relative">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16" data-aos="fade-up">
          <h2 class="text-3xl lg:text-4xl font-bold text-slate-900 mb-3">时间胶囊墙</h2>
          <p class="text-lg text-slate-600">访客留下的足迹与祝福</p>
        </div>

        <TimeCapsuleDisplay />
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
// 逻辑部分保持原有结构，只是补充了类型声明
import { ref, onMounted, onUnmounted } from 'vue'

const api = useApi()
const latestPosts = ref<any[]>([])

// 获取最新文章
const fetchLatestPosts = async () => {
  try {
    const res = await api.get<any>('/Articles', {
      params: {
        page: 1,
        pageSize: 2
      }
    })
    latestPosts.value = res.list
  } catch (e) {
    console.error('Failed to fetch latest posts', e)
  }
}

// 日期格式化
const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('zh-CN')
}

// 轮播文字逻辑
const currentRoleIndex = ref(0)
let roleInterval: number | null = null

const startRoleRotation = () => {
  roleInterval = window.setInterval(() => {
    currentRoleIndex.value = (currentRoleIndex.value + 1) % 4
  }, 3000) // 每 3 秒切换一次
}

// 滚动到内容区域
const scrollToContent = () => {
  const contentSection = document.getElementById('content')
  if (contentSection) {
    contentSection.scrollIntoView({ behavior: 'smooth' })
  }
}

// 简单滚动动画观察器（模拟 AOS）
let observer: IntersectionObserver | null = null

onMounted(() => {
  fetchLatestPosts()
  startRoleRotation()

  observer = new IntersectionObserver(
    entries => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('aos-animate')
        }
      })
    },
    { threshold: 0.1 }
  )

  document.querySelectorAll<HTMLElement>('[data-aos]').forEach(el => {
    observer?.observe(el)
  })
})

onUnmounted(() => {
  if (roleInterval) {
    clearInterval(roleInterval)
  }
  if (observer) {
    observer.disconnect()
  }
})

definePageMeta({
  layout: 'home'
})

useHead({
  title: '溪午听风 - 数字花园',
  meta: [{ name: 'description', content: '溪午听风的个人网站，分享技术、生活与思考。' }]
})
</script>

<style scoped>
/* 动画关键帧 */
@keyframes blob {
  0% {
    transform: translate(0px, 0px) scale(1);
  }
  33% {
    transform: translate(30px, -50px) scale(1.1);
  }
  66% {
    transform: translate(-20px, 20px) scale(0.9);
  }
  100% {
    transform: translate(0px, 0px) scale(1);
  }
}

@keyframes float {
  0%,
  100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-10px);
  }
}

@keyframes scroll-down {
  0% {
    transform: translateY(0);
    opacity: 1;
  }
  100% {
    transform: translateY(6px);
    opacity: 0;
  }
}

@keyframes gradient-x {
  0% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
  100% {
    background-position: 0% 50%;
  }
}

@keyframes tilt {
  0%,
  50%,
  100% {
    transform: rotate(0deg);
  }
  25% {
    transform: rotate(0.5deg);
  }
  75% {
    transform: rotate(-0.5deg);
  }
}

/* 工具类动画 */
.animate-blob {
  animation: blob 7s infinite;
}

.animation-delay-2000 {
  animation-delay: 2s;
}

.animation-delay-4000 {
  animation-delay: 4s;
}

.animate-float {
  animation: float 3s ease-in-out infinite;
}

.animate-float-delayed {
  animation: float 3s ease-in-out 1.5s infinite;
}

.animate-scroll-down {
  animation: scroll-down 1.5s infinite;
}

.animate-gradient-x {
  background-size: 200% 200%;
  animation: gradient-x 4s ease infinite;
}

.animate-tilt {
  animation: tilt 10s infinite linear;
}

/* 文字轮播动画 */
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.5s ease;
}

.slide-up-enter-from {
  opacity: 0;
  transform: translateY(20px);
}

.slide-up-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}

/* AOS 初始状态 */
[data-aos] {
  opacity: 0;
  transform: translateY(20px);
  transition:
    opacity 0.6s ease-out,
    transform 0.6s ease-out;
}

[data-aos].aos-animate {
  opacity: 1;
  transform: translateY(0);
}

[data-aos='fade-right'] {
  transform: translateX(-20px);
}

[data-aos='fade-left'] {
  transform: translateX(20px);
}

[data-aos].aos-animate[data-aos='fade-right'],
[data-aos].aos-animate[data-aos='fade-left'] {
  transform: translateX(0);
}
</style>
