<template>
  <div class="min-h-screen bg-slate-50 overflow-x-hidden selection:bg-blue-500/30">
    <!-- 添加到主屏幕提示 -->
    <AddToHomeScreen />

    <!-- 全局微纹理（极低透明度，仅增加质感） -->
    <div
      class="fixed inset-0 pointer-events-none opacity-[0.01] z-50 mix-blend-overlay"
      style="background-image: url(&quot;data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noiseFilter'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noiseFilter)'/%3E%3C/svg%3E&quot;);"
    ></div>

    <!-- Hero Banner -->
    <section class="relative min-h-[100vh] flex items-center justify-center overflow-hidden bg-gradient-to-br from-gray-950 via-slate-950 to-zinc-950 py-16 sm:py-20 md:py-24">
      <!-- 专业网格背景 -->
      <div class="absolute inset-0 overflow-hidden pointer-events-none z-0">
        <!-- 清晰的网格线 -->
        <div class="absolute inset-0 opacity-[0.03]">
          <svg class="w-full h-full" xmlns="http://www.w3.org/2000/svg">
            <defs>
              <pattern id="grid" width="40" height="40" patternUnits="userSpaceOnUse">
                <path d="M 40 0 L 0 0 0 40" fill="none" stroke="white" stroke-width="1"/>
              </pattern>
            </defs>
            <rect width="100%" height="100%" fill="url(#grid)" />
          </svg>
        </div>
        
        <!-- 极微妙的角落光晕（几乎不可见，仅增加深度感） -->
        <div class="absolute top-0 right-0 w-[600px] h-[600px] bg-blue-500/2 rounded-full blur-[120px]"></div>
        <div class="absolute bottom-0 left-0 w-[600px] h-[600px] bg-slate-500/2 rounded-full blur-[120px]"></div>
      </div>

      <!-- 主内容 -->
      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12 w-full z-10">
        <div class="grid lg:grid-cols-2 gap-16 lg:gap-20 xl:gap-24 items-center">
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
              class="text-4xl sm:text-5xl md:text-6xl lg:text-7xl xl:text-8xl font-bold text-white tracking-tight leading-[1.1] mb-6"
            >
              你好，我是
              <span
                class="block mt-3 text-transparent bg-clip-text bg-gradient-to-r from-blue-400 via-purple-400 to-cyan-300 animate-gradient-x"
              >
                溪午听风
              </span>
            </h1>

            <!-- 角色轮播 -->
            <div
              class="h-10 sm:h-12 md:h-14 text-xl sm:text-2xl lg:text-3xl text-slate-300 font-semibold flex items-center justify-center lg:justify-start overflow-hidden mb-6"
            >
              <span class="mr-3 sm:mr-4 text-slate-400">我是一名</span>
              <div class="relative h-full w-64 sm:w-72 text-left">
                <transition-group name="slide-up" tag="div" class="absolute top-0 left-0 w-full">
                  <span
                    v-if="currentRoleIndex === 0"
                    key="0"
                    class="block font-bold text-blue-300"
                  >
                    全栈开发者
                  </span>
                  <span
                    v-if="currentRoleIndex === 1"
                    key="1"
                    class="block font-bold text-purple-300"
                  >
                    AI 应用探索者
                  </span>
                  <span
                    v-if="currentRoleIndex === 2"
                    key="2"
                    class="block font-bold text-cyan-300"
                  >
                    Revit 插件实践者
                  </span>
                  <span
                    v-if="currentRoleIndex === 3"
                    key="3"
                    class="block font-bold text-orange-200"
                  >
                    终身学习者
                  </span>
                </transition-group>
              </div>
            </div>

            <!-- 简介文案 -->
            <p
              class="text-base sm:text-lg md:text-xl text-slate-300 max-w-2xl mx-auto lg:mx-0 leading-relaxed mb-8"
            >
              专注构建高效、优雅的数字体验。
              无论是企业级业务系统、AI 驱动应用，还是有趣的小工具，我都致力于把复杂问题做得简单好用。
            </p>

            <!-- 个人标签 -->
            <div
              class="flex flex-wrap justify-center lg:justify-start gap-3 mb-8 text-sm sm:text-base text-slate-300"
            >
              <span class="px-4 py-2 rounded-full bg-white/5 border border-white/10 font-medium">前后端一体化</span>
              <span class="px-4 py-2 rounded-full bg-white/5 border border-white/10 font-medium">AI 应用落地</span>
              <span class="px-4 py-2 rounded-full bg-white/5 border border-white/10 font-medium">自动化效率提升</span>
            </div>

            <!-- 按钮区 -->
            <div
              class="flex flex-wrap justify-center lg:justify-start gap-4 sm:gap-5"
            >
              <NuxtLink to="/projects" class="relative inline-flex group touch-target">
                <div
                  class="absolute transition-all duration-700 opacity-70 -inset-px bg-gradient-to-r from-[#44BCFF] via-[#FF44EC] to-[#FF675E] rounded-xl blur-lg group-hover:opacity-100 group-hover:-inset-1 group-hover:duration-200 animate-tilt"
                ></div>
                <button
                  class="relative inline-flex items-center justify-center px-8 sm:px-10 md:px-12 py-4 sm:py-4.5 md:py-5 text-base sm:text-lg md:text-xl font-bold text-white transition-all duration-200 bg-slate-950/80 rounded-xl focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-900/80 min-h-[48px]"
                >
                  浏览项目
                  <i
                    class="fas fa-arrow-right ml-3 group-hover:translate-x-1 transition-transform"
                  ></i>
                </button>
              </NuxtLink>

              <NuxtLink
                to="/about"
                class="inline-flex items-center justify-center px-8 sm:px-10 md:px-12 py-4 sm:py-4.5 md:py-5 text-base sm:text-lg md:text-xl font-bold text-slate-200 transition-all duration-200 bg-white/5 border-2 border-slate-600/70 rounded-xl hover:bg-white/10 hover:border-slate-400/80 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-900/80 min-h-[48px] touch-target"
              >
                关于我
              </NuxtLink>
            </div>
          </div>

          <!-- 右侧视觉区 -->
          <div class="relative hidden lg:block" data-aos="fade-left">
            <div class="relative w-full max-w-sm mx-auto aspect-square">
              <!-- 柔和渐变背景块 - 使用更中性的颜色 -->
              <div
                class="absolute inset-0 bg-gradient-to-tr from-slate-700/20 via-blue-900/20 to-slate-600/20 rounded-[2.1rem] rotate-6 opacity-40 blur-3xl"
              ></div>

              <!-- 主卡片 - 使用更现代的毛玻璃效果 -->
              <div class="w-full h-full rounded-[2rem] overflow-hidden border border-white/20 backdrop-blur-2xl bg-gradient-to-br from-white/10 via-white/5 to-transparent shadow-2xl shadow-indigo-500/20">
                <div class="w-full h-full relative">
                  <!-- 粒子头像 - 降低可见度 -->
                  <div class="w-full h-full opacity-40 hover:opacity-60 transition-opacity duration-300">
                    <ParticleAvatar
                      :image-url="'/images/avatar.jpg'"
                      :text="'溪午听风'"
                      :subtitle="'Full-stack & AI Developer'"
                      :particle-count="1200"
                      :interactive="true"
                      class="w-full h-full"
                    />
                  </div>

                  <!-- 底部信息条 - 重新设计为更融合的样式 -->
                  <div
                    class="absolute bottom-5 left-5 right-5 p-5 backdrop-blur-2xl rounded-2xl border border-white/20 bg-gradient-to-br from-white/15 via-white/10 to-white/5 shadow-2xl"
                  >
                    <div class="flex items-center justify-between gap-3 mb-3">
                      <div class="flex-1">
                        <h3 class="font-bold text-white text-lg mb-1 drop-shadow-lg">溪午听风</h3>
                        <p class="text-xs text-slate-200/90 font-medium">
                          全栈开发 · AI 应用 · Revit 插件
                        </p>
                      </div>
                      <div
                        class="w-12 h-12 bg-gradient-to-br from-slate-600/60 to-blue-800/60 rounded-xl flex items-center justify-center text-white text-lg border border-white/30 shadow-lg backdrop-blur-sm"
                      >
                        <i class="fas fa-code"></i>
                      </div>
                    </div>
                    <div class="mt-4 pt-3 border-t border-white/20">
                      <div class="flex flex-wrap gap-2 text-xs">
                        <span
                          class="px-3 py-1.5 rounded-lg bg-blue-600/30 backdrop-blur-sm border border-blue-400/40 text-blue-100 font-medium shadow-lg"
                        >
                          Vue · Nuxt
                        </span>
                        <span
                          class="px-3 py-1.5 rounded-lg bg-slate-600/30 backdrop-blur-sm border border-slate-400/40 text-slate-100 font-medium shadow-lg"
                        >
                          .NET · Node.js
                        </span>
                        <span
                          class="px-3 py-1.5 rounded-lg bg-cyan-600/30 backdrop-blur-sm border border-cyan-400/40 text-cyan-100 font-medium shadow-lg"
                        >
                          AI 工具链
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- 悬浮卡片：项目进度 - 重新设计 -->
              <div
                class="absolute -right-6 top-12 p-4 backdrop-blur-2xl rounded-2xl border border-white/20 bg-gradient-to-br from-emerald-500/20 via-emerald-500/10 to-transparent shadow-2xl shadow-emerald-500/20 animate-float-delayed w-44 z-20"
              >
                <div class="flex items-center gap-2 mb-3">
                  <div
                    class="w-8 h-8 bg-emerald-500/40 backdrop-blur-sm rounded-lg flex items-center justify-center text-emerald-100 text-sm border border-emerald-400/50 shadow-lg"
                  >
                    <i class="fas fa-check"></i>
                  </div>
                  <span class="font-bold text-sm text-white drop-shadow-lg">个人站点已上线</span>
                </div>
                <div class="w-full bg-white/10 backdrop-blur-sm rounded-full h-2 overflow-hidden border border-white/20">
                  <div
                    class="bg-gradient-to-r from-emerald-400 to-emerald-500 h-2 rounded-full w-full shadow-[0_0_15px_rgba(74,222,128,0.8)]"
                  ></div>
                </div>
                <p class="mt-2 text-[11px] text-slate-200/80 font-medium">持续迭代功能与体验</p>
              </div>

              <!-- 悬浮卡片：AI 驱动 - 重新设计 -->
              <div
                class="absolute -left-6 bottom-24 p-4 backdrop-blur-2xl rounded-2xl border border-white/20 bg-gradient-to-br from-purple-500/20 via-purple-500/10 to-transparent shadow-2xl shadow-purple-500/20 animate-float w-44 z-20"
              >
                <div class="flex items-center gap-3">
                  <div
                    class="w-8 h-8 bg-purple-500/40 backdrop-blur-sm rounded-lg flex items-center justify-center text-purple-100 text-sm border border-purple-400/50 shadow-lg"
                  >
                    <i class="fas fa-sparkles"></i>
                  </div>
                  <div>
                    <div class="font-bold text-sm text-white drop-shadow-lg mb-0.5">AI 驱动体验</div>
                    <div class="text-[11px] text-slate-200/80 font-medium">智能助手 · 智能工具</div>
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

    <!-- Bento Grid 内容展示 -->
    <section id="content" class="py-32 bg-slate-50 relative">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 xl:px-12">
        <div class="text-center mb-20" data-aos="fade-up">
          <h2 class="text-4xl lg:text-5xl xl:text-6xl font-bold text-slate-900 mb-4">探索我的世界</h2>
          <p class="text-xl lg:text-2xl text-slate-600">这里有代码、有思考，也有生活</p>
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

          <!-- AI 实验室 -->
          <div class="md:col-span-1 lg:col-span-1 row-span-1" data-aos="fade-up" data-aos-delay="200">
            <TiltCard class="h-full">
              <div
                class="h-full bg-slate-950 text-white rounded-3xl shadow-md p-6 relative overflow-hidden border border-slate-800 hover:border-cyan-400/70 hover:shadow-cyan-500/30 transition-all flex flex-col"
              >
                <div class="absolute -top-10 -right-10 w-32 h-32 bg-cyan-500/20 rounded-full blur-2xl"></div>
                <div class="absolute bottom-0 left-0 w-24 h-24 bg-blue-500/15 rounded-full blur-xl -ml-8 -mb-8"></div>

                <div class="relative z-10 flex flex-col h-full">
                  <div
                    class="w-12 h-12 bg-white/10 rounded-2xl flex items-center justify-center text-cyan-300 text-xl mb-4 border border-white/10"
                  >
                    <i class="fas fa-flask"></i>
                  </div>
                  <h3 class="text-lg sm:text-xl font-bold mb-2">AI 实验室</h3>
                  <p class="text-sm text-slate-300 mb-4 flex-grow">
                    3D 场景、AI 小实验与互动体验的集合地。
                  </p>
                  <NuxtLink
                    to="/lab"
                    class="inline-flex items-center text-xs sm:text-sm font-medium text-cyan-300 hover:text-cyan-100 mt-auto group"
                  >
                    进入实验室
                    <i class="fas fa-arrow-right ml-1 text-xs group-hover:translate-x-1 transition-transform"></i>
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

// 这是一个风格组件，不需要 definePageMeta 和 useHead
</script>

<style scoped>
/* 动画关键帧 */

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
