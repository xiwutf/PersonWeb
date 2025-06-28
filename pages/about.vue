<template>
  <div class="container mx-auto px-4 py-8">
    <div class="max-w-4xl mx-auto">
      <!-- 页面标题 -->
      <div class="text-center mb-12">
        <h1 class="text-4xl font-bold text-gray-800 mb-4">👤 关于我</h1>
        <p class="text-xl text-gray-600">了解更多关于溪午听风的信息</p>
      </div>

      <!-- 个人介绍卡片 -->
      <div class="bg-white rounded-xl shadow-lg overflow-hidden mb-12">
        <div class="md:flex">
          <div class="md:w-1/3 bg-gradient-to-br from-blue-500 to-purple-600 p-8 text-white text-center">
            <div class="w-32 h-32 bg-white rounded-full mx-auto mb-4 overflow-hidden border-2 border-blue-200">
              <img src="/images/avatar.jpg" alt="溪午听风头像" class="w-full h-full object-cover" />
            </div>
            <h2 class="text-2xl font-bold mb-2">溪午听风</h2>
            <p class="text-blue-100">全栈开发者 & Revit插件专家</p>
          </div>
          <div class="md:w-2/3 p-8">
            <h3 class="text-2xl font-semibold text-gray-800 mb-4">个人简介</h3>
            <p class="text-gray-600 mb-4">
              你好！我是溪午听风，一名热爱技术的全栈开发者。专注于Web开发、Revit插件开发和移动端应用开发。
              拥有丰富的项目经验，致力于用技术解决实际问题，提升工作效率。
            </p>
            <p class="text-gray-600 mb-6">
              除了技术开发，我还热衷于分享知识和经验，通过博客记录技术心得、项目总结和生活感悟。
              同时对理财投资也有一定的研究和实践经验。
            </p>
            
            <!-- 技能标签 -->
            <div class="mb-6">
              <h4 class="text-lg font-semibold text-gray-800 mb-3">技能专长</h4>
              <div class="flex flex-wrap gap-2">
                <span class="px-3 py-1 bg-blue-100 text-blue-600 rounded-full text-sm">Vue.js</span>
                <span class="px-3 py-1 bg-green-100 text-green-600 rounded-full text-sm">Nuxt.js</span>
                <span class="px-3 py-1 bg-yellow-100 text-yellow-600 rounded-full text-sm">JavaScript</span>
                <span class="px-3 py-1 bg-purple-100 text-purple-600 rounded-full text-sm">Node.js</span>
                <span class="px-3 py-1 bg-red-100 text-red-600 rounded-full text-sm">Revit API</span>
                <span class="px-3 py-1 bg-indigo-100 text-indigo-600 rounded-full text-sm">C#</span>
                <span class="px-3 py-1 bg-pink-100 text-pink-600 rounded-full text-sm">微信小程序</span>
                <span class="px-3 py-1 bg-teal-100 text-teal-600 rounded-full text-sm">MongoDB</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 联系方式 -->
      <div class="grid md:grid-cols-2 gap-8 mb-12">
        <!-- 联系信息 -->
        <div class="bg-white rounded-lg shadow-md p-6">
          <h3 class="text-xl font-semibold text-gray-800 mb-6 border-b-2 border-blue-500 pb-2">
            📬 联系方式
          </h3>
          <div class="space-y-4">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
                <span class="text-blue-600">📧</span>
              </div>
              <div>
                <p class="text-sm text-gray-500">邮箱</p>
                <p class="text-gray-800">255106739@qq.com</p>
              </div>
            </div>
            
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 bg-green-100 rounded-lg flex items-center justify-center">
                <span class="text-green-600">💬</span>
              </div>
              <div>
                <p class="text-sm text-gray-500">微信</p>
                <p class="text-gray-800">LinXi-5152</p>
              </div>
            </div>
            
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 bg-gray-100 rounded-lg flex items-center justify-center">
                <span class="text-gray-600">🐙</span>
              </div>
              <div>
                <p class="text-sm text-gray-500">GitHub</p>
                <a href="#" class="text-blue-600 hover:text-blue-800">github.com/Lijing327</a>
              </div>
            </div>
            
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
                <span class="text-blue-600">💼</span>
              </div>
              <div>
                <p class="text-sm text-gray-500">QQ</p>
                <a href="#" class="text-blue-600 hover:text-blue-800">255106739</a>
              </div>
            </div>
          </div>
        </div>

        <!-- 二维码区域 -->
        <div class="bg-white rounded-lg shadow-md p-6">
          <h3 class="text-xl font-semibold text-gray-800 mb-6 border-b-2 border-green-500 pb-2">
            📱 扫码关注
          </h3>
          <div class="flex justify-center">
            <!-- 微信二维码 -->
            <div class="text-center">
              <div class="w-40 h-40 bg-white rounded-lg mx-auto mb-3 overflow-hidden border border-gray-200 shadow-sm">
                <img src="/images/wechat-qr.png" alt="微信二维码" class="w-full h-full object-cover" />
              </div>
              <div class="flex items-center justify-center gap-2 mb-2">
                <span class="text-green-500">💬</span>
                <p class="text-sm font-medium text-gray-700">微信扫码加好友</p>
              </div>
              <p class="text-xs text-gray-500">LinXi-5152</p>
            </div>
          </div>
          
          <!-- 公众号敬请期待 -->
          <div class="mt-6 pt-4 border-t border-gray-100">
            <div class="text-center">
              <div class="w-16 h-16 bg-gray-100 rounded-lg mx-auto mb-2 flex items-center justify-center">
                <span class="text-gray-400 text-2xl">📱</span>
              </div>
              <p class="text-sm text-gray-400">公众号敬请期待</p>
              <p class="text-xs text-gray-300 mt-1">技术分享与经验交流</p>
            </div>
          </div>
        </div>
      </div>

      <!-- 项目成就和技术经验 -->
      <div class="grid md:grid-cols-2 gap-8 mb-12">
        <div class="bg-white rounded-lg shadow-md p-6">
          <h3 class="text-xl font-semibold text-gray-800 mb-6 border-b-2 border-purple-500 pb-2">
            🚀 项目成就
          </h3>
          <div class="space-y-4">
            <div class="border-l-4 border-blue-500 pl-4">
              <h4 class="font-semibold text-gray-800">恋爱魔方小程序</h4>
              <p class="text-sm text-gray-600">个人项目 | 2024年</p>
              <p class="text-sm text-gray-500 mt-1">累计用户5000+，日活跃用户800+，好评率98%的情侣互动小程序</p>
            </div>
            <div class="border-l-4 border-green-500 pl-4">
              <h4 class="font-semibold text-gray-800">Revit插件工具集</h4>
              <p class="text-sm text-gray-600">商业产品 | 2023-2024年</p>
              <p class="text-sm text-gray-500 mt-1">开发多款Revit插件，累计销售1000+份，帮助建筑师提升工作效率</p>
            </div>
            <div class="border-l-4 border-orange-500 pl-4">
              <h4 class="font-semibold text-gray-800">个人技术博客</h4>
              <p class="text-sm text-gray-600">知识分享 | 2023年至今</p>
              <p class="text-sm text-gray-500 mt-1">分享技术心得、项目经验和理财知识，持续输出高质量内容</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-md p-6">
          <h3 class="text-xl font-semibold text-gray-800 mb-6 border-b-2 border-orange-500 pb-2">
            💡 技术经验
          </h3>
          <div class="space-y-4">
            <div class="border-l-4 border-blue-500 pl-4">
              <h4 class="font-semibold text-gray-800">全栈开发</h4>
              <p class="text-sm text-gray-600">3年+ 经验</p>
              <p class="text-sm text-gray-500 mt-1">熟练掌握Vue.js、Nuxt.js、Node.js等现代Web开发技术栈</p>
            </div>
            <div class="border-l-4 border-green-500 pl-4">
              <h4 class="font-semibold text-gray-800">移动端开发</h4>
              <p class="text-sm text-gray-600">2年+ 经验</p>
              <p class="text-sm text-gray-500 mt-1">微信小程序开发，具备完整的产品设计和开发能力</p>
            </div>
            <div class="border-l-4 border-purple-500 pl-4">
              <h4 class="font-semibold text-gray-800">插件开发</h4>
              <p class="text-sm text-gray-600">2年+ 经验</p>
              <p class="text-sm text-gray-500 mt-1">Revit API开发，C#编程，专业软件二次开发</p>
            </div>
          </div>
        </div>
      </div>

      <!-- 兴趣爱好 -->
      <div class="bg-white rounded-lg shadow-md p-6 mb-12">
        <h3 class="text-xl font-semibold text-gray-800 mb-6 border-b-2 border-pink-500 pb-2">
          🎯 兴趣爱好
        </h3>
        <div class="grid md:grid-cols-4 gap-4">
          <div class="text-center p-4 rounded-lg bg-blue-50 hover:bg-blue-100 transition-colors">
            <span class="text-2xl mb-2 block">💻</span>
            <p class="text-sm text-gray-700">编程开发</p>
          </div>
          <div class="text-center p-4 rounded-lg bg-green-50 hover:bg-green-100 transition-colors">
            <span class="text-2xl mb-2 block">📚</span>
            <p class="text-sm text-gray-700">技术学习</p>
          </div>
          <div class="text-center p-4 rounded-lg bg-purple-50 hover:bg-purple-100 transition-colors">
            <span class="text-2xl mb-2 block">💰</span>
            <p class="text-sm text-gray-700">投资理财</p>
          </div>
          <div class="text-center p-4 rounded-lg bg-orange-50 hover:bg-orange-100 transition-colors">
            <span class="text-2xl mb-2 block">✈️</span>
            <p class="text-sm text-gray-700">旅行摄影</p>
          </div>
        </div>
      </div>

      <!-- 返回首页链接 -->
      <div class="text-center">
        <NuxtLink to="/" class="inline-flex items-center text-blue-600 hover:text-blue-800 font-medium">
          ← 返回首页
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container {
  min-height: 100vh;
  background: linear-gradient(135deg, #a8edea 0%, #fed6e3 100%);
}
</style> 