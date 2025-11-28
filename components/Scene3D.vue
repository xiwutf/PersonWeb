<template>
  <div ref="containerRef" class="w-full relative" :style="{ height: height }">
    <canvas ref="canvasRef" class="w-full h-full" />
    
    <!-- 交互提示 -->
    <div v-if="showHint" class="absolute bottom-4 left-1/2 transform -translate-x-1/2 bg-black/50 text-white px-4 py-2 rounded-lg text-sm backdrop-blur-sm z-10">
      点击 3D 对象跳转到不同页面
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick } from 'vue'
import * as THREE from 'three'

const props = withDefaults(defineProps<{
  type?: 'earth' | 'spaceship' | 'datasphere'
  showHint?: boolean
  height?: string
}>(), {
  type: 'earth',
  showHint: true,
  height: '400px'
})

const containerRef = ref<HTMLDivElement | null>(null)
const canvasRef = ref<HTMLCanvasElement | null>(null)

let scene: THREE.Scene | null = null
let camera: THREE.PerspectiveCamera | null = null
let renderer: THREE.WebGLRenderer | null = null
let animationId: number | null = null
let object: THREE.Object3D | null = null
let raycaster: THREE.Raycaster | null = null
let mouse: THREE.Vector2 | null = null
let resizeHandler: (() => void) | null = null
let mouseMoveHandler: ((e: MouseEvent) => void) | null = null
let clickHandler: (() => void) | null = null

const router = useRouter()

const initScene = () => {
  if (!canvasRef.value || !containerRef.value) return

  try {
    // 创建场景
    scene = new THREE.Scene()
    scene.background = new THREE.Color(0x0a0a0a)

  // 创建相机
  const width = containerRef.value.clientWidth
  const height = containerRef.value.clientHeight
  camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 1000)
  camera.position.z = 5

  // 创建渲染器 - 启用阴影和更好的渲染质量
  renderer = new THREE.WebGLRenderer({ 
    canvas: canvasRef.value, 
    antialias: true,
    alpha: true,
    powerPreference: "high-performance"
  })
  renderer.setSize(width, height)
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2)) // 限制像素比以提升性能
  renderer.shadowMap.enabled = true
  renderer.shadowMap.type = THREE.PCFSoftShadowMap

  // 创建对象
  createObject()

  // 添加多光源系统 - 更炫酷的光照
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.4)
  scene.add(ambientLight)

  // 主光源
  const directionalLight = new THREE.DirectionalLight(0xffffff, 1.2)
  directionalLight.position.set(5, 5, 5)
  directionalLight.castShadow = true
  scene.add(directionalLight)

  // 补光 - 从另一侧
  const fillLight = new THREE.DirectionalLight(0x4488ff, 0.5)
  fillLight.position.set(-5, 0, -5)
  scene.add(fillLight)

  // 点光源 - 增加氛围
  const pointLight = new THREE.PointLight(0x00ffff, 0.8, 10)
  pointLight.position.set(0, 0, 5)
  scene.add(pointLight)

  // 射线检测
  raycaster = new THREE.Raycaster()
  mouse = new THREE.Vector2()

  // 鼠标事件
  const handleMouseMove = (event: MouseEvent) => {
    if (!mouse || !containerRef.value) return
    const rect = containerRef.value.getBoundingClientRect()
    mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1
    mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1
  }

  const handleClick = () => {
    if (!raycaster || !mouse || !camera || !object) return
    
    raycaster.setFromCamera(mouse, camera)
    const intersects = raycaster.intersectObject(object, true)
    
    if (intersects.length > 0) {
      // 根据类型跳转到不同页面
      const type = props.type || 'earth'
      const routes: Record<string, string> = {
        earth: '/blog',
        spaceship: '/projects',
        datasphere: '/dashboard'
      }
      router.push(routes[type] || '/')
    }
  }

  // 窗口大小调整
  const handleResize = () => {
    if (!containerRef.value || !camera || !renderer) return
    const width = containerRef.value.clientWidth
    const height = containerRef.value.clientHeight
    camera.aspect = width / height
    camera.updateProjectionMatrix()
    renderer.setSize(width, height)
  }

  // 保存事件处理函数引用，以便在 onUnmounted 中移除
  resizeHandler = handleResize
  mouseMoveHandler = handleMouseMove
  clickHandler = handleClick

  // 添加事件监听
  window.addEventListener('resize', handleResize)
  if (canvasRef.value) {
    canvasRef.value.addEventListener('mousemove', handleMouseMove)
    canvasRef.value.addEventListener('click', handleClick)
  }

  // 鼠标悬停效果
  let isHovered = false
  const originalScale = { x: 1, y: 1, z: 1 }
  
  const handleMouseEnter = () => {
    isHovered = true
  }
  
  const handleMouseLeave = () => {
    isHovered = false
  }
  
  if (canvasRef.value) {
    canvasRef.value.addEventListener('mouseenter', handleMouseEnter)
    canvasRef.value.addEventListener('mouseleave', handleMouseLeave)
  }

  // 动画循环 - 更流畅的动画
  let time = 0
  const animate = () => {
    if (!scene || !camera || !renderer) return

    time += 0.016 // 约 60fps

    if (object) {
      // 基础旋转
      object.rotation.y += 0.008
      object.rotation.x = Math.sin(time * 0.5) * 0.1
      
      // 鼠标悬停时放大
      if (isHovered) {
        const scale = 1 + Math.sin(time * 10) * 0.05 + 0.1
        object.scale.set(scale, scale, scale)
      } else {
        // 呼吸效果
        const breath = 1 + Math.sin(time * 2) * 0.03
        object.scale.set(breath, breath, breath)
      }
      
      // 根据类型添加特殊动画
      const type = props.type || 'earth'
      if (type === 'earth' && object.children) {
        // 地球轨道环旋转
        object.children.forEach((child: THREE.Object3D) => {
          if (child.type === 'Mesh' && (child as THREE.Mesh).geometry?.type === 'TorusGeometry') {
            child.rotation.z += 0.01
          }
        })
      } else if (type === 'datasphere' && object.children) {
        // 数据星球粒子闪烁
        object.children.forEach((child: THREE.Object3D, index: number) => {
          if (child.type === 'Mesh' && (child as THREE.Mesh).geometry?.type === 'SphereGeometry' && (child as THREE.Mesh).geometry.parameters?.radius === 0.05) {
            const opacity = 0.3 + Math.sin(time * 5 + index) * 0.7
            if ((child as THREE.Mesh).material instanceof THREE.MeshBasicMaterial) {
              (child as THREE.Mesh).material.opacity = opacity
            }
          }
        })
      }
    }

    renderer.render(scene, camera)
    animationId = requestAnimationFrame(animate)
  }

  animate()
  } catch (error) {
    console.error('Failed to initialize 3D scene:', error)
  }
}

const createObject = () => {
  if (!scene) return

  const type = props.type || 'earth'

  switch (type) {
    case 'earth':
      // 创建炫酷的地球 - 多层效果
      const earthGroup = new THREE.Group()
      
      // 外层光晕
      const outerGeometry = new THREE.SphereGeometry(2.2, 32, 32)
      const outerMaterial = new THREE.MeshBasicMaterial({
        color: 0x3b82f6,
        transparent: true,
        opacity: 0.2,
        side: THREE.DoubleSide
      })
      const outerSphere = new THREE.Mesh(outerGeometry, outerMaterial)
      earthGroup.add(outerSphere)
      
      // 主球体 - 带渐变效果
      const earthGeometry = new THREE.SphereGeometry(2, 64, 64)
      const earthMaterial = new THREE.MeshPhongMaterial({
        color: 0x3b82f6,
        emissive: 0x1e40af,
        emissiveIntensity: 0.5,
        shininess: 100,
        specular: 0xffffff
      })
      const earth = new THREE.Mesh(earthGeometry, earthMaterial)
      earthGroup.add(earth)
      
      // 内部发光球
      const innerGeometry = new THREE.SphereGeometry(1.5, 32, 32)
      const innerMaterial = new THREE.MeshBasicMaterial({
        color: 0x60a5fa,
        transparent: true,
        opacity: 0.3
      })
      const innerSphere = new THREE.Mesh(innerGeometry, innerMaterial)
      earthGroup.add(innerSphere)
      
      // 添加轨道环
      const ringGeometry = new THREE.TorusGeometry(2.5, 0.05, 16, 100)
      const ringMaterial = new THREE.MeshBasicMaterial({
        color: 0x60a5fa,
        transparent: true,
        opacity: 0.6
      })
      const ring = new THREE.Mesh(ringGeometry, ringMaterial)
      ring.rotation.x = Math.PI / 2
      earthGroup.add(ring)
      
      object = earthGroup
      break

    case 'spaceship':
      // 创建炫酷的飞船
      const shipGroup = new THREE.Group()
      
      // 主体 - 流线型
      const bodyGeometry = new THREE.ConeGeometry(0.4, 1.8, 8)
      const bodyMaterial = new THREE.MeshPhongMaterial({
        color: 0x888888,
        emissive: 0x333333,
        shininess: 100
      })
      const body = new THREE.Mesh(bodyGeometry, bodyMaterial)
      body.rotation.x = Math.PI
      shipGroup.add(body)
      
      // 机翼 - 带发光效果
      const wingGeometry = new THREE.BoxGeometry(2.2, 0.15, 0.6)
      const wingMaterial = new THREE.MeshPhongMaterial({
        color: 0x666666,
        emissive: 0x444444,
        shininess: 80
      })
      const wing1 = new THREE.Mesh(wingGeometry, wingMaterial)
      wing1.position.set(0, -0.4, 0)
      shipGroup.add(wing1)
      
      // 推进器发光
      const thrusterGeometry = new THREE.CylinderGeometry(0.15, 0.2, 0.3, 8)
      const thrusterMaterial = new THREE.MeshBasicMaterial({
        color: 0xff4444,
        emissive: 0xff0000,
        transparent: true,
        opacity: 0.8
      })
      const thruster = new THREE.Mesh(thrusterGeometry, thrusterMaterial)
      thruster.position.set(0, -0.9, 0)
      shipGroup.add(thruster)
      
      // 添加能量环
      const energyRingGeometry = new THREE.TorusGeometry(0.5, 0.03, 16, 50)
      const energyRingMaterial = new THREE.MeshBasicMaterial({
        color: 0x00ffff,
        emissive: 0x00ffff,
        transparent: true,
        opacity: 0.7
      })
      const energyRing = new THREE.Mesh(energyRingGeometry, energyRingMaterial)
      energyRing.rotation.x = Math.PI / 2
      energyRing.position.y = 0.2
      shipGroup.add(energyRing)
      
      object = shipGroup
      break

    case 'datasphere':
      // 创建炫酷的数据星球 - 多层线框
      const dataGroup = new THREE.Group()
      
      // 外层线框球
      const outerWireGeometry = new THREE.IcosahedronGeometry(2.2, 1)
      const outerWireMaterial = new THREE.MeshBasicMaterial({
        color: 0x00ff88,
        wireframe: true,
        transparent: true,
        opacity: 0.4
      })
      const outerWire = new THREE.Mesh(outerWireGeometry, outerWireMaterial)
      dataGroup.add(outerWire)
      
      // 中层线框球
      const midWireGeometry = new THREE.IcosahedronGeometry(2, 1)
      const midWireMaterial = new THREE.MeshBasicMaterial({
        color: 0x00ffaa,
        wireframe: true,
        emissive: 0x004422,
        emissiveIntensity: 0.5
      })
      const midWire = new THREE.Mesh(midWireGeometry, midWireMaterial)
      dataGroup.add(midWire)
      
      // 内层发光球
      const innerDataGeometry = new THREE.IcosahedronGeometry(1.5, 0)
      const innerDataMaterial = new THREE.MeshBasicMaterial({
        color: 0x00ff88,
        transparent: true,
        opacity: 0.3,
        emissive: 0x00ff88,
        emissiveIntensity: 0.3
      })
      const innerData = new THREE.Mesh(innerDataGeometry, innerDataMaterial)
      dataGroup.add(innerData)
      
      // 添加数据点粒子效果
      for (let i = 0; i < 20; i++) {
        const pointGeometry = new THREE.SphereGeometry(0.05, 8, 8)
        const pointMaterial = new THREE.MeshBasicMaterial({
          color: 0x00ff88,
          emissive: 0x00ff88
        })
        const point = new THREE.Mesh(pointGeometry, pointMaterial)
        
        // 随机分布在球面上
        const radius = 2.1
        const theta = Math.random() * Math.PI * 2
        const phi = Math.acos(Math.random() * 2 - 1)
        point.position.set(
          radius * Math.sin(phi) * Math.cos(theta),
          radius * Math.sin(phi) * Math.sin(theta),
          radius * Math.cos(phi)
        )
        dataGroup.add(point)
      }
      
      object = dataGroup
      break
  }

  if (object) {
    scene.add(object)
  }
}

onMounted(() => {
  nextTick(() => {
    if (typeof window !== 'undefined' && containerRef.value && canvasRef.value) {
      initScene()
    }
  })
})

onUnmounted(() => {
  if (resizeHandler) {
    window.removeEventListener('resize', resizeHandler)
  }
  if (mouseMoveHandler && canvasRef.value) {
    canvasRef.value.removeEventListener('mousemove', mouseMoveHandler)
  }
  if (clickHandler && canvasRef.value) {
    canvasRef.value.removeEventListener('click', clickHandler)
  }
  if (animationId) {
    cancelAnimationFrame(animationId)
  }
  if (renderer) {
    renderer.dispose()
  }
  // 清理场景
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

