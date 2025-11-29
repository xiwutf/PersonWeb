<template>
  <div class="admin-page">
    <!-- 页面头部 -->
    <div class="page-header">
      <h1 class="page-title">会员管理</h1>
      <div class="header-actions">
        <button class="btn-secondary" @click="refresh">
          <i class="fas fa-sync-alt mr-2"></i>
          刷新
        </button>
      </div>
    </div>

    <!-- 提示卡片 -->
    <div class="membership-card">
      <h2 class="membership-title">会员功能规划中</h2>
      <p class="membership-desc">
        当前版本仅提供页面占位，会员套餐、权益配置、订阅统计等功能还在设计中。
      </p>
      <ul class="membership-list">
        <li>支持按「会员等级」管理访问权限与功能解锁</li>
        <li>支持查看会员订阅状态与续费时间</li>
        <li>支持和 API 计费、订单系统联动，统计收入与使用情况</li>
      </ul>
      <p class="membership-tip">
        如需优先实现某些会员相关功能，可以在「需求清单」中记录或直接在 TODO 中标记优先级。
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin',
  middleware: 'admin-auth'
})

const refresh = () => {
  // 预留刷新逻辑，后续接入会员列表 / 统计接口时使用
}
</script>

<style scoped>
.admin-page {
  padding: 2rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.page-title {
  font-size: 1.5rem;
  font-weight: 600;
}

.membership-card {
  padding: 1.75rem 2rem;
  border-radius: 0.75rem;
  background: rgba(15, 23, 42, 0.9);
  border: 1px solid rgba(148, 163, 184, 0.4);
  box-shadow: 0 18px 45px rgba(15, 23, 42, 0.6);
}

.membership-title {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 0.75rem;
}

.membership-desc {
  font-size: 0.95rem;
  color: #e5e7eb;
  margin-bottom: 0.75rem;
}

.membership-list {
  margin-left: 1.25rem;
  margin-bottom: 0.75rem;
  list-style-type: disc;
  color: #e5e7eb;
  font-size: 0.95rem;
}

.membership-tip {
  font-size: 0.9rem;
  color: #9ca3af;
}

.btn-secondary {
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  border: none;
  background: rgba(148, 163, 184, 0.2);
  color: #e5e7eb;
  font-size: 0.875rem;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  transition: background 0.2s ease, transform 0.1s ease;
}

.btn-secondary:hover {
  background: rgba(148, 163, 184, 0.35);
  transform: translateY(-1px);
}
</style>


