<template>
  <div class="container mx-auto px-4 py-8">
    <div class="flex flex-col lg:flex-row gap-8">
      <!-- 文章内容 -->
      <div class="w-full lg:w-3/4">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-8">
          <ContentDoc :path="`/blog/${route.params.id}`">
            <template #default="{ doc }">
              <div class="mb-6">
                <h1 class="text-3xl font-bold text-gray-800 dark:text-white mb-4">{{ doc.title }}</h1>
                <div class="flex items-center text-gray-500 dark:text-gray-400 text-sm">
                  <span class="mr-4">📅 {{ formatDate(doc.date) }}</span>
                  <span v-if="doc.category" class="mr-4">📂 {{ doc.category }}</span>
                  <span v-if="doc.tags" class="mr-4">🏷️ {{ doc.tags.join(', ') }}</span>
                </div>
              </div>

              <!-- 移动端 TOC (折叠) -->
              <div class="lg:hidden mb-6 bg-gray-50 dark:bg-gray-700/50 p-4 rounded-lg">
                <details>
                  <summary class="font-bold text-gray-700 dark:text-gray-300 cursor-pointer">目录</summary>
                  <ul class="mt-2 ml-4 list-disc text-sm text-gray-600 dark:text-gray-400">
                    <li v-for="link in doc.body.toc.links" :key="link.id">
                      <a :href="`#${link.id}`">{{ link.text }}</a>
                      <ul v-if="link.children">
                         <li v-for="child in link.children" :key="child.id">
                           <a :href="`#${child.id}`">{{ child.text }}</a>
                         </li>
                      </ul>
                    </li>
                  </ul>
                </details>
              </div>

              <ContentRenderer :value="doc" class="prose dark:prose-invert max-w-none" />
            </template>
            <template #not-found>
              <div class="text-center py-10">
                <h1 class="text-2xl font-bold text-gray-800 dark:text-white mb-4">文章未找到</h1>
                <p class="text-gray-600 dark:text-gray-400 mb-6">抱歉，您访问的文章不存在或已被删除。</p>
                <NuxtLink to="/blog" class="text-blue-600 hover:text-blue-800">返回博客列表</NuxtLink>
              </div>
            </template>
          </ContentDoc>
        </div>
      </div>

      <!-- 侧边栏 (桌面端 TOC) -->
      <div class="hidden lg:block w-1/4">
        <div class="sticky top-24">
          <ContentDoc :path="`/blog/${route.params.id}`">
            <template #default="{ doc }">
              <div v-if="doc.body.toc && doc.body.toc.links.length > 0" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
                <h3 class="font-bold text-gray-800 dark:text-white mb-4 text-lg">目录</h3>
                <nav>
                  <ul class="space-y-2 text-sm text-gray-600 dark:text-gray-400">
                    <li v-for="link in doc.body.toc.links" :key="link.id">
                      <a :href="`#${link.id}`" class="hover:text-blue-600 dark:hover:text-blue-400 block truncate transition-colors" :title="link.text">{{ link.text }}</a>
                      <ul v-if="link.children" class="ml-4 mt-1 space-y-1 border-l border-gray-200 dark:border-gray-700 pl-2">
                        <li v-for="child in link.children" :key="child.id">
                          <a :href="`#${child.id}`" class="hover:text-blue-600 dark:hover:text-blue-400 block truncate transition-colors" :title="child.text">{{ child.text }}</a>
                        </li>
                      </ul>
                    </li>
                  </ul>
                </nav>
              </div>
            </template>
            <template #not-found></template>
          </ContentDoc>
          
          <div class="mt-6 text-center">
             <NuxtLink to="/blog" class="text-blue-600 hover:text-blue-800 text-sm">
              &larr; 返回博客列表
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const route = useRoute()

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString()
}
</script>

<style>
/* 简单的平滑滚动 */
html {
  scroll-behavior: smooth;
}
</style>

