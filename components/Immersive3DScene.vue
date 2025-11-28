<template>
  <div ref="containerRef" class="absolute inset-0 w-full h-full overflow-hidden">
    <canvas ref="canvasRef" class="w-full h-full" />
    
    <!-- 交互提示 -->
    <div v-if="showHint" class="absolute bottom-8 left-1/2 transform -translate-x-1/2 bg-black/50 text-white px-6 py-3 rounded-full text-sm backdrop-blur-md z-20 border border-white/20 pointer-events-none">
      <span class="mr-2">🖱️</span>
      滚动页面或拖动鼠标探索 3D 世界
    </div>

    <!-- 导航标签 -->
    <div class="absolute top-8 left-8 z-20 space-y-3 pointer-events-auto">
      <button
        v-for="(item, index) in navigationItems"
        :key="index"
        @click="navigateTo(item.path)"
        class="block px-4 py-2 bg-black/50 backdrop-blur-md text-white rounded-lg hover:bg-black/70 transition-all border border-white/20 hover:border-blue-400/50 hover:scale-105 cursor-pointer"
        :class="{ 'bg-blue-500/50 border-blue-400': currentPath === item.path }"
      >
        <span class="mr-2">{{ item.icon }}</span>
        {{ item.label }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick } from 'vue'
import * as THREE from 'three'

const props = withDefaults(defineProps<{
  showHint?: boolean
  scrollEnabled?: boolean
}>(), {
  showHint: true,
  scrollEnabled: true
})

const containerRef = ref<HTMLDivElement | null>(null)
const canvasRef = ref<HTMLCanvasElement | null>(null)

let scene: THREE.Scene | null = null
let camera: THREE.PerspectiveCamera | null = null
let renderer: THREE.WebGLRenderer | null = null
let controls: any = null
let animationId: number | null = null

// 3D 对象
let objects: THREE.Object3D[] = []
let raycaster: THREE.Raycaster | null = null
let mouse: THREE.Vector2 | null = null

const router = useRouter()
const route = useRoute()
const currentPath = computed(() => route.path)

const navigationItems = [
  { icon: '🌍', label: '博客', path: '/blog' },
  { icon: '🚀', label: '项目', path: '/projects' },
  { icon: '📚', label: '知识库', path: '/knowledge' },
  { icon: '💻', label: '工具', path: '/tools' },
  { icon: '👤', label: '关于', path: '/about' }
]

const initScene = () => {
  if (!canvasRef.value || !containerRef.value) return

  try {
    // 创建场景
    scene = new THREE.Scene()
    scene.background = new THREE.Color(0x0a0a0f)
    scene.fog = new THREE.Fog(0x0a0a0f, 10, 50)

    // 创建相机
    const width = containerRef.value.clientWidth
    const height = containerRef.value.clientHeight
    camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 1000)
    camera.position.set(0, 0, 15)

    // 创建渲染器
    renderer = new THREE.WebGLRenderer({
      canvas: canvasRef.value,
      antialias: true,
      alpha: true,
      powerPreference: 'high-performance'
    })
    renderer.setSize(width, height)
    renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2))
    renderer.shadowMap.enabled = true
    renderer.shadowMap.type = THREE.PCFSoftShadowMap

    // 添加光源
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.5)
    scene.add(ambientLight)

    const directionalLight = new THREE.DirectionalLight(0xffffff, 1)
    directionalLight.position.set(5, 5, 5)
    directionalLight.castShadow = true
    scene.add(directionalLight)

    const pointLight1 = new THREE.PointLight(0x3b82f6, 1, 20)
    pointLight1.position.set(-5, 5, 5)
    scene.add(pointLight1)

    const pointLight2 = new THREE.PointLight(0x8b5cf6, 1, 20)
    pointLight2.position.set(5, -5, 5)
    scene.add(pointLight2)

    // 创建3D对象（代表不同模块）
    createObjects()

    // 射线检测
    raycaster = new THREE.Raycaster()
    mouse = new THREE.Vector2()

    // 鼠标事件
    if (canvasRef.value) {
      canvasRef.value.addEventListener('mousemove', handleMouseMove)
      canvasRef.value.addEventListener('click', handleClick)
    }

    // 滚动事件
    if (props.scrollEnabled) {
      window.addEventListener('scroll', handleScroll, { passive: true })
    }

    // 窗口大小调整
    window.addEventListener('resize', handleResize)

    // 开始动画
    animate()
  } catch (error) {
    console.error('Failed to initialize 3D scene:', error)
  }
}

const createObjects = () => {
  if (!scene) return

  // 创建多个3D对象，代表不同模块
  const positions = [
    { x: -8, y: 0, z: 0, color: 0x3b82f6, path: '/blog', icon: '🌍' },
    { x: -4, y: 3, z: -2, color: 0x8b5cf6, path: '/projects', icon: '🚀' },
    { x: 0, y: 0, z: 0, color: 0x10b981, path: '/', icon: '💎' },
    { x: 4, y: -3, z: -2, color: 0xf59e0b, path: '/knowledge', icon: '📚' },
    { x: 8, y: 0, z: 0, color: 0xec4899, path: '/tools', icon: '💻' }
  ]

  positions.forEach((pos, index) => {
    const group = new THREE.Group()
    group.userData = { path: pos.path, icon: pos.icon }

    // 主球体
    const geometry = new THREE.IcosahedronGeometry(1, 1)
    const material = new THREE.MeshPhongMaterial({
      color: pos.color,
      emissive: pos.color,
      emissiveIntensity: 0.3,
      shininess: 100,
      transparent: true,
      opacity: 0.9
    })
    const mesh = new THREE.Mesh(geometry, material)
    mesh.castShadow = true
    mesh.receiveShadow = true
    group.add(mesh)

    // 外层光晕
    const glowGeometry = new THREE.IcosahedronGeometry(1.3, 1)
    const glowMaterial = new THREE.MeshBasicMaterial({
      color: pos.color,
      transparent: true,
      opacity: 0.2,
      side: THREE.BackSide
    })
    const glow = new THREE.Mesh(glowGeometry, glowMaterial)
    group.add(glow)

    // 轨道环
    const ringGeometry = new THREE.TorusGeometry(1.5, 0.05, 16, 100)
    const ringMaterial = new THREE.MeshBasicMaterial({
      color: pos.color,
      transparent: true,
      opacity: 0.6
    })
    const ring = new THREE.Mesh(ringGeometry, ringMaterial)
    ring.rotation.x = Math.PI / 2
    group.add(ring)

    group.position.set(pos.x, pos.y, pos.z)
    objects.push(group)
    scene.add(group)
  })

  // 添加粒子系统
  createParticleSystem()
}

const createParticleSystem = () => {
  if (!scene) return

  const particleCount = 500
  const geometry = new THREE.BufferGeometry()
  const positions = new Float32Array(particleCount * 3)
  const colors = new Float32Array(particleCount * 3)

  for (let i = 0; i < particleCount; i++) {
    const i3 = i * 3
    positions[i3] = (Math.random() - 0.5) * 50
    positions[i3 + 1] = (Math.random() - 0.5) * 50
    positions[i3 + 2] = (Math.random() - 0.5) * 50

    const color = new THREE.Color()
    color.setHSL(Math.random(), 0.7, 0.6)
    colors[i3] = color.r
    colors[i3 + 1] = color.g
    colors[i3 + 2] = color.b
  }

  geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3))
  geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3))

  const material = new THREE.PointsMaterial({
    size: 0.1,
    vertexColors: true,
    transparent: true,
    opacity: 0.6
  })

  const particles = new THREE.Points(geometry, material)
  scene.add(particles)
}

let time = 0
const animate = () => {
  if (!scene || !camera || !renderer) return

  time += 0.016

  // 旋转所有对象
  objects.forEach((obj, index) => {
    obj.rotation.y += 0.01
    obj.rotation.x = Math.sin(time * 0.5 + index) * 0.1

    // 呼吸效果
    const scale = 1 + Math.sin(time * 2 + index) * 0.05
    obj.scale.set(scale, scale, scale)

    // 轨道环旋转
    obj.children.forEach((child) => {
      if (child.type === 'Mesh' && (child as THREE.Mesh).geometry?.type === 'TorusGeometry') {
        child.rotation.z += 0.02
      }
    })
  })

  // 相机轻微摆动
  if (camera) {
    camera.position.x = Math.sin(time * 0.3) * 2
    camera.position.y = Math.cos(time * 0.2) * 1
    camera.lookAt(0, 0, 0)
  }

  renderer.render(scene, camera)
  animationId = requestAnimationFrame(animate)
}

const handleMouseMove = (e: MouseEvent) => {
  if (!containerRef.value || !mouse || !camera) return

  const rect = containerRef.value.getBoundingClientRect()
  mouse.x = ((e.clientX - rect.left) / rect.width) * 2 - 1
  mouse.y = -((e.clientY - rect.top) / rect.height) * 2 + 1

  // 相机跟随鼠标
  if (camera) {
    camera.position.x += (mouse.x * 2 - camera.position.x) * 0.05
    camera.position.y += (mouse.y * 2 - camera.position.y) * 0.05
  }
}

const handleClick = () => {
  if (!raycaster || !mouse || !camera) return

  raycaster.setFromCamera(mouse, camera)
  const intersects = raycaster.intersectObjects(objects, true)

  if (intersects.length > 0) {
    const object = intersects[0].object.parent
    if (object && object.userData.path) {
      router.push(object.userData.path)
    }
  }
}

const handleScroll = () => {
  if (!camera) return

  const scrollY = window.scrollY
  const maxScroll = document.documentElement.scrollHeight - window.innerHeight
  const scrollProgress = scrollY / maxScroll

  // 相机根据滚动位置移动
  camera.position.z = 15 - scrollProgress * 10
  camera.position.y = scrollProgress * 5
}

const handleResize = () => {
  if (!containerRef.value || !camera || !renderer) return

  const width = containerRef.value.clientWidth
  const height = containerRef.value.clientHeight

  camera.aspect = width / height
  camera.updateProjectionMatrix()
  renderer.setSize(width, height)
}

const navigateTo = (path: string) => {
  router.push(path)
}

onMounted(() => {
  nextTick(() => {
    if (typeof window !== 'undefined' && containerRef.value && canvasRef.value) {
      initScene()
    }
  })
})

onUnmounted(() => {
  if (canvasRef.value) {
    canvasRef.value.removeEventListener('mousemove', handleMouseMove)
    canvasRef.value.removeEventListener('click', handleClick)
  }
  window.removeEventListener('scroll', handleScroll)
  window.removeEventListener('resize', handleResize)
  
  if (animationId) {
    cancelAnimationFrame(animationId)
  }
  
  if (renderer) {
    renderer.dispose()
  }
  
  if (scene) {
    scene.clear()
  }
})
</script>

<style scoped>
canvas {
  display: block;
  width: 100%;
  height: 100%;
}
</style>

