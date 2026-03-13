<template>
  <div class="payment-cancel">
    <div class="container">
      <div class="cancel-icon">
        <i class="fas fa-times-circle"></i>
      </div>
      <h1>支付已取消</h1>
      <p>您已取消本次支付，订单尚未完成。</p>

      <div class="reasons">
        <h3>可能的原因：</h3>
        <ul>
          <li>您不想继续购买此模块</li>
          <li>支付过程中出现错误</li>
          <li>需要使用其他支付方式</li>
        </ul>
      </div>

      <div class="actions">
        <button @click="retryPayment" class="btn btn-primary">
          <i class="fas fa-redo"></i> 重新支付
        </button>
        <button @click="goToModules" class="btn btn-outline">
          浏览其他模块
        </button>
        <button @click="contactSupport" class="btn btn-secondary">
          <i class="fas fa-headset"></i> 联系客服
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()

const retryPayment = () => {
  // 获取订单ID并重新跳转到支付页面
  const orderId = route.query.orderId
  if (orderId) {
    router.push(`/payment?orderId=${orderId}`)
  } else {
    router.push('/modules')
  }
}

const goToModules = () => {
  router.push('/modules')
}

const contactSupport = () => {
  // 打开客服联系方式
  window.open('mailto:support@example.com?subject=支付咨询', '_blank')
}
</script>

<style scoped>
.payment-cancel {
  padding: 2rem 0;
}

.container {
  max-width: 600px;
  margin: 0 auto;
  text-align: center;
  padding: 2rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px var(--shadow);
}

.cancel-icon {
  font-size: 4rem;
  color: #f44336;
  margin-bottom: 1rem;
}

h1 {
  color: #333;
  margin-bottom: 1rem;
}

p {
  color: #666;
  margin-bottom: 2rem;
}

.reasons {
  background: #fff3cd;
  padding: 1.5rem;
  border-radius: 4px;
  margin-bottom: 2rem;
  text-align: left;
}

.reasons h3 {
  margin-top: 0;
  color: #856404;
}

.reasons ul {
  color: #856404;
  padding-left: 20px;
}

.reasons li {
  margin-bottom: 0.5rem;
}

.actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-primary {
  background: #2196f3;
  color: white;
}

.btn-primary:hover {
  background: #1976d2;
}

.btn-secondary {
  background: #ff9800;
  color: white;
}

.btn-secondary:hover {
  background: #f57c00;
}

.btn-outline {
  background: transparent;
  border: 2px solid #2196f3;
  color: #2196f3;
}

.btn-outline:hover {
  background: #2196f3;
  color: white;
}

i {
  font-size: 1.2rem;
}
</style>