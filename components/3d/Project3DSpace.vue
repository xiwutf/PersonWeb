<template>
  <div ref="containerRef" class="relative w-full h-screen overflow-hidden bg-gradient-to-b from-slate-900 via-purple-900 to-slate-900">
    <canvas ref="canvasRef" class="w-full h-full" />
    
    <!-- 返回列表按钮 -->
    <button
      @click="handleBackToList"
      class="project-3d-back-button"
      title="返回列表视图"
    >
      <svg class="project-3d-back-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
      </svg>
      <span class="project-3d-back-text">返回列表</span>
    </button>
    
    <!-- 控制提示 -->
    <div class="project-3d-hint">
      <div class="project-3d-hint-content">
        <div class="project-3d-hint-item">🖱️ 拖动旋转视角</div>
        <div class="project-3d-hint-item">🖱️ 滚轮缩放</div>
        <div class="project-3d-hint-item">👆 点击卡片查看详情</div>
      </div>
    </div>
    
    <!-- 项目信息卡片 -->
    <div
      v-if="selectedProject"
      class="project-3d-info-card"
    >
      <button
        @click="selectedProject = null"
        class="project-3d-info-close"
        title="关闭"
      >
        ×
      </button>
      <h3 class="project-3d-info-title">{{ selectedProject.title }}</h3>
      <p class="project-3d-info-description">{{ selectedProject.description }}</p>
      <div class="project-3d-info-actions">
        <NuxtLink
          :to="`/projects/${selectedProject.id}`"
          class="project-3d-info-button project-3d-info-button-primary"
        >
          查看详情
        </NuxtLink>
        <a
          v-if="selectedProject.githubUrl"
          :href="selectedProject.githubUrl"
          target="_blank"
          class="project-3d-info-button project-3d-info-button-secondary"
        >
          GitHub
        </a>
      </div>
    </div>
    
    <!-- 悬停提示 -->
    <div v-if="hoveredProject" class="project-3d-hover-hint">
      <div class="project-3d-hover-hint-content">
        <div class="project-3d-hover-hint-title">{{ hoveredProject.title }}</div>
        <div class="project-3d-hover-hint-text">点击查看详情</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import * as THREE from 'three'
import type { Project } from '~/types/api'

const props = defineProps<{
  projects: Project[]
}>()

const emit = defineEmits<{
  'back-to-list': []
}>()

const containerRef = ref<HTMLDivElement | null>(null)
const canvasRef = ref<HTMLCanvasElement | null>(null)
const selectedProject = ref<Project | null>(null)
const hoveredProject = ref<Project | null>(null)

let scene: THREE.Scene | null = null
let camera: THREE.PerspectiveCamera | null = null
let renderer: THREE.WebGLRenderer | null = null
let controls: any = null
let projectCards: THREE.Mesh[] = []
let raycaster: THREE.Raycaster | null = null
let mouse: THREE.Vector2 | null = null

const initThree = () => {
  if (!canvasRef.value || !containerRef.value) return
  
  // 场景
  scene = new THREE.Scene()
  
  // 相机
  camera = new THREE.PerspectiveCamera(
    75,
    window.innerWidth / window.innerHeight,
    0.1,
    1000
  )
  camera.position.z = 50
  
  // 渲染器
  renderer = new THREE.WebGLRenderer({
    canvas: canvasRef.value,
    antialias: true,
    alpha: true
  })
  renderer.setSize(window.innerWidth, window.innerHeight)
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2))
  
  // 轨道控制器
  if (process.client) {
    import('three/examples/jsm/controls/OrbitControls.js').then((module) => {
      const OrbitControls = module.OrbitControls
      controls = new OrbitControls(camera, renderer.domElement)
      controls.enableDamping = true
      controls.dampingFactor = 0.05
      controls.minDistance = 20
      controls.maxDistance = 100
    }).catch(() => {
      // 如果导入失败，使用简单的鼠标控制
      console.warn('OrbitControls not available, using simple controls')
    })
  }
  
  // 光线
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.5)
  scene.add(ambientLight)
  
  const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8)
  directionalLight.position.set(10, 10, 10)
  scene.add(directionalLight)
  
  // 点光源
  const pointLight1 = new THREE.PointLight(0x3b82f6, 1, 100)
  pointLight1.position.set(20, 20, 20)
  scene.add(pointLight1)
  
  const pointLight2 = new THREE.PointLight(0x8b5cf6, 1, 100)
  pointLight2.position.set(-20, -20, 20)
  scene.add(pointLight2)
  
  // 射线检测
  raycaster = new THREE.Raycaster()
  mouse = new THREE.Vector2()
  
  // 创建项目卡片
  createProjectCards()
  
  // 动画循环
  animate()
}

const createProjectCards = () => {
  if (!scene) return
  
  const count = props.projects.length
  const radius = 30
  
  props.projects.forEach((project, index) => {
    // 计算位置（球形分布）
    const phi = Math.acos(-1 + (2 * index) / count)
    const theta = Math.sqrt(count * Math.PI) * phi
    
    const x = radius * Math.cos(theta) * Math.sin(phi)
    const y = radius * Math.sin(theta) * Math.sin(phi)
    const z = radius * Math.cos(phi)
    
    // 创建卡片几何体
    const geometry = new THREE.PlaneGeometry(8, 6)
    
    // 创建材质
    const material = new THREE.MeshStandardMaterial({
      color: 0x1e293b,
      metalness: 0.3,
      roughness: 0.4,
      side: THREE.DoubleSide
    })
    
    const card = new THREE.Mesh(geometry, material)
    card.position.set(x, y, z)
    card.lookAt(0, 0, 0)
    card.userData = { project }
    
    // 添加文字标签（简化版，直接使用 Canvas 纹理）
    createTextTexture(project.title).then((texture) => {
      if (!scene || !camera) return
      const textMaterial = new THREE.MeshStandardMaterial({
        map: texture,
        transparent: true,
        side: THREE.DoubleSide
      })
      const textMesh = new THREE.Mesh(geometry, textMaterial)
      textMesh.position.copy(card.position)
      textMesh.position.z += 0.01
      textMesh.lookAt(camera.position)
      scene.add(textMesh)
    }).catch(() => {
      // 如果创建纹理失败，跳过
    })
    
    scene.add(card)
    projectCards.push(card)
  })
}

const createTextTexture = (text: string): Promise<THREE.Texture> => {
  return new Promise((resolve) => {
    const canvas = document.createElement('canvas')
    canvas.width = 512
    canvas.height = 384
    const ctx = canvas.getContext('2d')
    
    if (!ctx) {
      resolve(new THREE.Texture())
      return
    }
    
    // 背景
    ctx.fillStyle = 'var(--color-border-default)'
    ctx.fillRect(0, 0, canvas.width, canvas.height)
    
    // 文字
    ctx.fillStyle = 'var(--color-bg-card)'
    ctx.font = 'bold 48px Arial'
    ctx.textAlign = 'center'
    ctx.textBaseline = 'middle'
    ctx.fillText(text, canvas.width / 2, canvas.height / 2)
    
    const texture = new THREE.CanvasTexture(canvas)
    texture.needsUpdate = true
    resolve(texture)
  })
}

const handleBackToList = () => {
  emit('back-to-list')
}

const onMouseMove = (event: MouseEvent) => {
  if (!raycaster || !camera || !mouse) return
  
  mouse.x = (event.clientX / window.innerWidth) * 2 - 1
  mouse.y = -(event.clientY / window.innerHeight) * 2 + 1
  
  raycaster.setFromCamera(mouse, camera)
  
  const intersects = raycaster.intersectObjects(projectCards)
  
  if (intersects.length > 0) {
    const hoveredCard = intersects[0].object as THREE.Mesh
    if (hoveredCard.userData.project) {
      hoveredProject.value = hoveredCard.userData.project
      // 添加悬停效果：放大卡片
      hoveredCard.scale.set(1.2, 1.2, 1.2)
      // 改变材质颜色
      if (Array.isArray(hoveredCard.material)) {
        hoveredCard.material.forEach((mat: any) => {
          if (mat instanceof THREE.MeshStandardMaterial) {
            mat.color.setHex(0x3b82f6)
            mat.emissive.setHex(0x1e40af)
            mat.emissiveIntensity = 0.3
          }
        })
      } else if (hoveredCard.material instanceof THREE.MeshStandardMaterial) {
        hoveredCard.material.color.setHex(0x3b82f6)
        hoveredCard.material.emissive.setHex(0x1e40af)
        hoveredCard.material.emissiveIntensity = 0.3
      }
      
      // 恢复其他卡片
      projectCards.forEach((card) => {
        if (card !== hoveredCard) {
          card.scale.set(1, 1, 1)
          if (Array.isArray(card.material)) {
            card.material.forEach((mat: any) => {
              if (mat instanceof THREE.MeshStandardMaterial) {
                mat.color.setHex(0x1e293b)
                mat.emissive.setHex(0x000000)
                mat.emissiveIntensity = 0
              }
            })
          } else if (card.material instanceof THREE.MeshStandardMaterial) {
            card.material.color.setHex(0x1e293b)
            card.material.emissive.setHex(0x000000)
            card.material.emissiveIntensity = 0
          }
        }
      })
    }
  } else {
    hoveredProject.value = null
    // 恢复所有卡片
    projectCards.forEach((card) => {
      card.scale.set(1, 1, 1)
      if (Array.isArray(card.material)) {
        card.material.forEach((mat: any) => {
          if (mat instanceof THREE.MeshStandardMaterial) {
            mat.color.setHex(0x1e293b)
            mat.emissive.setHex(0x000000)
            mat.emissiveIntensity = 0
          }
        })
      } else if (card.material instanceof THREE.MeshStandardMaterial) {
        card.material.color.setHex(0x1e293b)
        card.material.emissive.setHex(0x000000)
        card.material.emissiveIntensity = 0
      }
    })
  }
}

const onMouseClick = (event: MouseEvent) => {
  if (!raycaster || !camera || !mouse) return
  
  mouse.x = (event.clientX / window.innerWidth) * 2 - 1
  mouse.y = -(event.clientY / window.innerHeight) * 2 + 1
  
  raycaster.setFromCamera(mouse, camera)
  
  const intersects = raycaster.intersectObjects(projectCards)
  
  if (intersects.length > 0) {
    const clickedCard = intersects[0].object as THREE.Mesh
    if (clickedCard.userData.project) {
      selectedProject.value = clickedCard.userData.project
    }
  }
}

const animate = () => {
  if (!renderer || !scene || !camera) return
  
  requestAnimationFrame(animate)
  
  // 更新控制器
  if (controls) {
    controls.update()
  }
  
  // 旋转卡片
  projectCards.forEach((card, index) => {
    card.rotation.y += 0.001
    const scale = 1 + Math.sin(Date.now() * 0.001 + index) * 0.05
    card.scale.set(scale, scale, scale)
  })
  
  renderer.render(scene, camera)
}

const handleResize = () => {
  if (!camera || !renderer) return
  
  camera.aspect = window.innerWidth / window.innerHeight
  camera.updateProjectionMatrix()
  renderer.setSize(window.innerWidth, window.innerHeight)
}

onMounted(() => {
  if (process.client) {
    initThree()
    window.addEventListener('click', onMouseClick)
    window.addEventListener('mousemove', onMouseMove)
    window.addEventListener('resize', handleResize)
  }
})

onUnmounted(() => {
  window.removeEventListener('click', onMouseClick)
  window.removeEventListener('mousemove', onMouseMove)
  window.removeEventListener('resize', handleResize)
  
  if (renderer) {
    renderer.dispose()
  }
  
  projectCards.forEach((card) => {
    if (card.geometry) card.geometry.dispose()
    if (Array.isArray(card.material)) {
      card.material.forEach((mat) => mat.dispose())
    } else if (card.material) {
      card.material.dispose()
    }
  })
})
</script>

<style scoped>
/* 返回列表按钮 */
.project-3d-back-button {
  position: fixed;
  top: 1rem;
  right: 1rem;
  z-index: 100;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(12px);
  border: 2px solid var(--theme-primary);
  border-radius: 0.75rem;
  color: var(--color-border-default);
  font-weight: 600;
  font-size: 0.875rem;
  cursor: pointer;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
}

.project-3d-back-button:hover {
  background: rgba(255, 255, 255, 1);
  border-color: rgba(59, 130, 246, 0.6);
  transform: translateY(-2px);
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.4);
}

.project-3d-back-icon {
  width: 1.25rem;
  height: 1.25rem;
  flex-shrink: 0;
}

.project-3d-back-text {
  white-space: nowrap;
}

/* 控制提示 */
.project-3d-hint {
  position: fixed;
  top: 1rem;
  left: 1rem;
  z-index: 100;
  background: rgba(0, 0, 0, 0.75);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.75rem;
  padding: 1rem;
  z-index: 100;
}

.project-3d-hint-content {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.project-3d-hint-item {
  color: white;
  font-size: 0.875rem;
  font-weight: 500;
  white-space: nowrap;
}

/* 项目信息卡片 */
.project-3d-info-card {
  position: fixed;
  top: 50%;
  right: 2rem;
  transform: translateY(-50%);
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(12px);
  border-radius: 1rem;
  padding: 1.5rem;
  max-width: 24rem;
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.4);
  z-index: 100;
  border: 1px solid var(--shadow);
}

.project-3d-info-close {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  background: var(--shadow);
  border: none;
  color: var(--color-border-default);
  font-size: 1.5rem;
  line-height: 1;
  cursor: pointer;
  transition: all 0.2s ease;
}

.project-3d-info-close:hover {
  background: rgba(0, 0, 0, 0.2);
  transform: scale(1.1);
}

.project-3d-info-title {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
  color: var(--color-border-default);
  padding-right: 2rem;
}

.project-3d-info-description {
  color: var(--color-text-sec);
  margin-bottom: 1rem;
  line-height: 1.6;
}

.project-3d-info-actions {
  display: flex;
  gap: 0.75rem;
}

.project-3d-info-button {
  padding: 0.625rem 1.25rem;
  border-radius: 0.5rem;
  font-weight: 600;
  font-size: 0.875rem;
  transition: all 0.2s ease;
  text-decoration: none;
  display: inline-block;
}

.project-3d-info-button-primary {
  background: var(--color-primary);
  color: white;
}

.project-3d-info-button-primary:hover {
  background: var(--color-primary-hover);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.4);
}

.project-3d-info-button-secondary {
  background: var(--color-border-default);
  color: white;
}

.project-3d-info-button-secondary:hover {
  background: var(--color-text-main);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

/* 悬停提示 */
.project-3d-hover-hint {
  position: fixed;
  bottom: 2rem;
  left: 50%;
  transform: translateX(-50%);
  z-index: 100;
  pointer-events: none;
}

.project-3d-hover-hint-content {
  background: rgba(59, 130, 246, 0.95);
  backdrop-filter: blur(12px);
  border-radius: 0.75rem;
  padding: 0.75rem 1.25rem;
  box-shadow: 0 10px 25px rgba(59, 130, 246, 0.4);
  animation: project-3d-hint-fade-in 0.3s ease;
}

.project-3d-hover-hint-title {
  color: white;
  font-weight: 600;
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}

.project-3d-hover-hint-text {
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.75rem;
}

@keyframes project-3d-hint-fade-in {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* 暗色模式适配 */
:global(.dark) .project-3d-info-card {
  background: rgba(30, 41, 59, 0.95);
  border-color: rgba(255, 255, 255, 0.1);
}

:global(.dark) .project-3d-info-title {
  color: white;
}

:global(.dark) .project-3d-info-description {
  color: var(--color-text-muted);
}

:global(.dark) .project-3d-info-close {
  background: rgba(255, 255, 255, 0.1);
  color: white;
}

:global(.dark) .project-3d-info-close:hover {
  background: rgba(255, 255, 255, 0.2);
}
</style>

