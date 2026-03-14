<template>
  <div class="payment-success">
    <div class="container">
      <div class="success-icon">
        <i class="fas fa-check-circle"></i>
      </div>
      <h1>支付成功！</h1>
      <p>感谢您的购买，您的许可证已生成。</p>

      <div class="order-info" v-if="orderData">
        <h3>订单信息</h3>
        <p><strong>订单编号：</strong>{{ orderData.orderNumber }}</p>
        <p><strong>支付金额：</strong>¥{{ orderData.price.toFixed(2) }}</p>
        <p><strong>购买模块：</strong>{{ orderData.moduleKey }} v{{ orderData.version }}</p>
        <p><strong>许可证密钥：</strong>{{ orderData.licenseKey }}</p>
      </div>

      <div class="actions">
        <button @click="copyLicense" class="btn btn-secondary">
          <i class="fas fa-copy"></i> 复制许可证
        </button>
        <button @click="downloadLicense" class="btn btn-primary">
          <i class="fas fa-download"></i> 下载许可证文件
        </button>
        <button @click="goToMyLicenses" class="btn btn-outline">
          查看我的许可证
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const orderData = ref(null)

onMounted(async () => {
  // 从路由参数中获取订单ID
  const orderId = route.query.orderId

  if (orderId) {
    try {
      // 这里应该调用API获取订单详情
      // const response = await fetch(`/api/payment/orders/${orderId}`)
      // orderData.value = await response.json()

      // 模拟数据
      orderData.value = {
        id: orderId,
        orderNumber: `ORDER_${Date.now()}`,
        price: 99.00,
        moduleKey: 'ai-assistant',
        version: '1.0.0',
        licenseKey: 'MOD-1x2y3z-abc123-def456'
      }
    } catch (error) {
      console.error('Failed to fetch order data:', error)
    }
  }
})

const copyLicense = async () => {
  if (orderData.value?.licenseKey) {
    try {
      await navigator.clipboard.writeText(orderData.value.licenseKey)
      alert('许可证已复制到剪贴板')
    } catch (error) {
      console.error('Failed to copy license:', error)
    }
  }
}

const downloadLicense = () => {
  if (orderData.value?.licenseKey) {
    const content = `LICENSE KEY: ${orderData.value.licenseKey}\nMODULE: ${orderData.value.moduleKey}\nVERSION: ${orderData.value.version}\nDATE: ${new Date().toLocaleString()}`

    const blob = new Blob([content], { type: 'text/plain' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `license-${orderData.value.moduleKey}.txt`
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
    URL.revokeObjectURL(url)
  }
}

const goToMyLicenses = () => {
  // 跳转到我的许可证页面
  router.push('/my-licenses')
}
</script>

<style scoped>
.payment-success {
  padding: 2rem 0;
}

.container {
  max-width: 600px;
  margin: 0 auto;
  text-align: center;
  padding: 2rem;
  background: var(--color-bg-light, white);
  border-radius: 8px;
  box-shadow: 0 2px 10px var(--shadow);
}

.success-icon {
  font-size: 4rem;
  color: var(--color-success);
  margin-bottom: 1rem;
}

h1 {
  color: var(--color-text-dark);
  margin-bottom: 1rem;
}

p {
  color: var(--color-text-muted);
  margin-bottom: 2rem;
}

.order-info {
  background: var(--color-bg-elevated);
  padding: 1.5rem;
  border-radius: 4px;
  margin-bottom: 2rem;
  text-align: left;
}

.order-info h3 {
  margin-top: 0;
  color: var(--color-text-dark);
}

.order-info p {
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
  background: var(--color-success);
  color: var(--color-bg-light, white);
}

.btn-primary:hover {
  background: var(--color-success-hover);
}

.btn-secondary {
  background: var(--color-primary);
  color: var(--color-bg-light, white);
}

.btn-secondary:hover {
  background: var(--color-primary-hover);
}

.btn-outline {
  background: transparent;
  border: 2px solid var(--color-primary);
  color: var(--color-primary);
}

.btn-outline:hover {
  background: var(--color-primary);
  color: var(--color-bg-light, white);
}

i {
  font-size: 1.2rem;
}
</style>