<template>
  <div class="library-access" :class="{ 'library-access-authorized': authenticated }">
    <ClientOnly>
      <NSpin v-if="loading" description="正在确认阅读权限…" />
      <NCard v-else-if="!authenticated" title="个人阅读中心">
        <NSpace vertical>
          <p>输入阅读访问密码，授权这台设备。</p>
          <NAlert v-if="error" type="error" role="alert">{{ error }}</NAlert>
          <form @submit.prevent="submit">
            <NSpace vertical>
              <label for="reading-password">阅读访问密码</label>
              <NInput :input-props="{ id: 'reading-password', autocomplete: 'current-password' }" v-model:value="password" type="password"
                show-password-on="click" :maxlength="1024" autocomplete="current-password" />
              <NButton type="primary" attr-type="submit" :disabled="!password">进入阅读中心</NButton>
              <NButton v-if="error" @click="check">重新检查连接</NButton>
            </NSpace>
          </form>
        </NSpace>
      </NCard>
      <div v-else class="library-authorized">
        <NAlert v-if="error" type="error">{{ error }}</NAlert>
        <slot />
      </div>
      <template #fallback><p role="status">正在加载阅读中心…</p></template>
    </ClientOnly>
  </div>
</template>

<script setup lang="ts">
import { NAlert, NButton, NCard, NInput, NSpace, NSpin } from 'naive-ui'
const { authenticated, loading, error, check, login } = useLibraryAccess()
const password = ref('')
let timer: ReturnType<typeof setInterval> | undefined
onMounted(() => {
  void check()
  timer = setInterval(() => { if (authenticated.value) void check() }, 12 * 60 * 60 * 1000)
})
onBeforeUnmount(() => { if (timer) clearInterval(timer) })
async function submit() {
  const value = password.value
  password.value = ''
  await login(value)
}
</script>
