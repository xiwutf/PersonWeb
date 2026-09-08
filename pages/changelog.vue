<script setup lang="ts">
import { nestLegacyWorkPath } from '~/utils/work-paths'
const route = useRoute()
await navigateTo(nestLegacyWorkPath(route.fullPath), { redirectCode: 301, external: false })
</script>
<template>
  <div />
</template>
