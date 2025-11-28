<template>
  <div ref="containerRef" class="relative w-full h-screen overflow-hidden bg-gradient-to-b from-slate-900 via-purple-900 to-slate-900">
    <canvas ref="canvasRef" class="w-full h-full" />
    
    <!-- 控制提示 -->
    <div class="absolute top-4 left-4 bg-black/50 text-white px-4 py-2 rounded-lg backdrop-blur-md text-sm z-10">
      <div>🖱️ 拖动旋转 | 🖱️ 滚轮缩放 | 点击卡片查看详情</div>
    </div>
    
    <!-- 项目信息卡片 -->
    <div
      v-if="selectedProject"
      class="absolute top-4 right-4 bg-white/90 dark:bg-gray-800/90 backdrop-blur-md rounded-xl p-6 max-w-sm shadow-2xl z-10"
    >
      <button
        @click="selectedProject = null"
        class="absolute top-2 right-2 w-6 h-6 flex items-center justify-center rounded-full bg-gray-200 hover:bg-gray-300 transition"
      >
        ×
      </button>
      <h3 class="text-xl font-bold mb-2">{{ selectedProject.title }}</h3>
      <p class="text-gray-600 dark:text-gray-400 mb-4">{{ selectedProject.description }}</p>
      <div class="flex gap-2">
        <NuxtLink
          :to="`/projects/${selectedProject.id}`"
          class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
        >
          查看详情
        </NuxtLink>
        <a
          v-if="selectedProject.githubUrl"
          :href="selectedProject.githubUrl"
          target="_blank"
          class="px-4 py-2 bg-gray-800 text-white rounded-lg hover:bg-gray-900 transition"
        >
          GitHub
        </a>
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

const containerRef = ref<HTMLDivElement | null>(null)
const canvasRef = ref<HTMLCanvasElement | null>(null)
const selectedProject = ref<Project | null>(null)

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
    ctx.fillStyle = '#1e293b'
    ctx.fillRect(0, 0, canvas.width, canvas.height)
    
    // 文字
    ctx.fillStyle = '#ffffff'
    ctx.font = 'bold 48px Arial'
    ctx.textAlign = 'center'
    ctx.textBaseline = 'middle'
    ctx.fillText(text, canvas.width / 2, canvas.height / 2)
    
    const texture = new THREE.CanvasTexture(canvas)
    texture.needsUpdate = true
    resolve(texture)
  })
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
    window.addEventListener('resize', handleResize)
  }
})

onUnmounted(() => {
  window.removeEventListener('click', onMouseClick)
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

