<template>
  <div ref="containerRef" class="project-3d-scene-bg relative h-screen w-full overflow-hidden">
    <div class="project-3d-atmosphere" aria-hidden="true">
      <span class="project-3d-atmosphere-glow project-3d-atmosphere-glow--blue" />
      <span class="project-3d-atmosphere-glow project-3d-atmosphere-glow--violet" />
      <span class="project-3d-atmosphere-grid" />
      <span class="project-3d-atmosphere-vignette" />
    </div>

    <canvas ref="canvasRef" class="h-full w-full" />

    <div class="project-3d-scene-copy">
      <div class="project-3d-scene-kicker">Project Orbit</div>
      <h2 class="project-3d-scene-title">3D 项目展厅</h2>
      <p class="project-3d-scene-description">
        用轨道和层次感把项目悬浮展开，拖动就能感受到前后景深，不再只是平面轮播。
      </p>
    </div>

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

    <div class="project-3d-hint">
      <div class="project-3d-hint-content">
        <div class="project-3d-hint-item">拖动可以旋转视角</div>
        <div class="project-3d-hint-item">滚轮可以缩放景深</div>
        <div class="project-3d-hint-item">点击卡片查看项目详情</div>
      </div>
    </div>

    <div v-if="selectedProject" class="project-3d-info-card">
      <button
        @click="clearSelection"
        class="project-3d-info-close"
        title="关闭"
      >
        ×
      </button>

      <div class="project-3d-info-kicker">Selected Project</div>
      <h3 class="project-3d-info-title">{{ selectedProject.title }}</h3>
      <p class="project-3d-info-description">{{ selectedProject.description }}</p>

      <div
        v-if="normalizedTechStack(selectedProject).length"
        class="project-3d-info-tags"
      >
        <span
          v-for="tag in normalizedTechStack(selectedProject).slice(0, 4)"
          :key="tag"
          class="project-3d-info-tag"
        >
          {{ tag }}
        </span>
      </div>

      <div class="project-3d-info-actions">
        <NuxtLink
          :to="getProjectDetailLink(selectedProject)"
          class="project-3d-info-button project-3d-info-button-primary"
        >
          查看详情
        </NuxtLink>
        <a
          v-if="selectedProject.githubUrl"
          :href="selectedProject.githubUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="project-3d-info-button project-3d-info-button-secondary"
        >
          GitHub
        </a>
      </div>
    </div>

    <div v-if="hoveredProject" class="project-3d-hover-hint">
      <div class="project-3d-hover-hint-content">
        <div class="project-3d-hover-hint-title">{{ hoveredProject.title }}</div>
        <div class="project-3d-hover-hint-text">点击卡片查看详情</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
import * as THREE from 'three'
import type { Project } from '~/types/api'

const props = defineProps<{
  projects: Project[]
}>()

const emit = defineEmits<{
  'back-to-list': []
}>()

type OrbitControlsLike = {
  update: () => void
  dispose?: () => void
  enableDamping: boolean
  dampingFactor: number
  minDistance: number
  maxDistance: number
  target: THREE.Vector3
  autoRotate?: boolean
  autoRotateSpeed?: number
}

type ProjectCardMesh = THREE.Mesh<THREE.PlaneGeometry, THREE.MeshStandardMaterial> & {
  userData: {
    project: Project
    orbitRadius: number
    orbitAngle: number
    orbitSpeed: number
    verticalOffset: number
    floatOffset: number
    depthScale: number
    baseScale: number
  }
}

const containerRef = ref<HTMLDivElement | null>(null)
const canvasRef = ref<HTMLCanvasElement | null>(null)
const selectedProject = ref<Project | null>(null)
const hoveredProject = ref<Project | null>(null)

let scene: THREE.Scene | null = null
let camera: THREE.PerspectiveCamera | null = null
let renderer: THREE.WebGLRenderer | null = null
let controls: OrbitControlsLike | null = null
let raycaster: THREE.Raycaster | null = null
let mouse: THREE.Vector2 | null = null
let projectCards: ProjectCardMesh[] = []
let projectOrbitalGroup: THREE.Group | null = null
let ambientParticles: THREE.Points | null = null
let hoveredCard: ProjectCardMesh | null = null
let animationFrameId = 0
const clock = new THREE.Clock()

function getProjectDetailLink(project: Project): string {
  const id = project.id
  if (id == null || id === '') return '/projects'
  if (typeof id === 'number') return `/projects/${id}`
  const value = String(id)
  if (value.startsWith('/') || /\.(png|jpg|jpeg|gif|webp|svg)(\?|$)/i.test(value)) return '/projects'
  return `/projects/${id}`
}

const normalizedTechStack = (project: Project): string[] => {
  const techStack = project.techStack
  if (Array.isArray(techStack)) {
    return techStack.filter((item): item is string => typeof item === 'string' && Boolean(item.trim()))
  }
  if (typeof techStack === 'string') {
    return techStack
      .split(',')
      .map(item => item.trim())
      .filter(Boolean)
  }
  return []
}

const getViewportSize = () => {
  const rect = containerRef.value?.getBoundingClientRect()
  return {
    width: Math.max(rect?.width || window.innerWidth, 1),
    height: Math.max(rect?.height || window.innerHeight, 1)
  }
}

const updateMouseFromEvent = (event: MouseEvent) => {
  if (!renderer || !mouse) return false
  const rect = renderer.domElement.getBoundingClientRect()
  if (!rect.width || !rect.height) return false

  mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1
  mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1
  return true
}

const get3DToken = (name: string, fallback: string): string => {
  if (typeof document === 'undefined') return fallback
  const value = getComputedStyle(document.documentElement).getPropertyValue(name).trim()
  return value || fallback
}

const drawTitle = (ctx: CanvasRenderingContext2D, title: string, width: number, height: number) => {
  const displayTitle = title.length > 12 ? `${title.slice(0, 12)}…` : title
  const fontFamily = get3DToken('--font-family-base', 'system-ui, sans-serif')
  ctx.font = `bold 36px ${fontFamily}`
  ctx.textAlign = 'center'
  ctx.textBaseline = 'bottom'
  ctx.fillStyle = get3DToken('--color-3d-card-text', '#f8fafc')
  ctx.strokeStyle = get3DToken('--color-3d-card-text-stroke', 'rgba(0, 0, 0, 0.6)')
  ctx.lineWidth = 3
  ctx.strokeText(displayTitle, width / 2, height - 24)
  ctx.fillText(displayTitle, width / 2, height - 24)
}

const createTitleOnlyTexture = (title: string): THREE.Texture => {
  const canvas = document.createElement('canvas')
  canvas.width = 512
  canvas.height = 384

  const ctx = canvas.getContext('2d')
  if (!ctx) return new THREE.Texture()

  ctx.fillStyle = get3DToken('--color-3d-card-bg', '#1e293b')
  ctx.fillRect(0, 0, canvas.width, canvas.height)
  ctx.strokeStyle = get3DToken('--color-3d-card-border', 'rgba(148, 163, 184, 0.5)')
  ctx.lineWidth = 2
  ctx.strokeRect(4, 4, canvas.width - 8, canvas.height - 8)
  drawTitle(ctx, title, canvas.width, canvas.height)

  const texture = new THREE.CanvasTexture(canvas)
  texture.needsUpdate = true
  return texture
}

const loadImageTexture = (url: string): Promise<HTMLImageElement> => {
  return new Promise((resolve, reject) => {
    const img = new Image()
    img.crossOrigin = 'anonymous'
    img.onload = () => resolve(img)
    img.onerror = () => reject(new Error('Failed to load image'))
    img.src = url
  })
}

const addTitleOverlay = (img: HTMLImageElement, title: string) => {
  const canvas = document.createElement('canvas')
  canvas.width = 512
  canvas.height = 384

  const ctx = canvas.getContext('2d')
  if (!ctx) return canvas

  ctx.drawImage(img, 0, 0, canvas.width, canvas.height)
  const overlay = get3DToken('--color-3d-overlay', 'rgba(0, 0, 0, 0.85)')
  const gradient = ctx.createLinearGradient(0, canvas.height * 0.48, 0, canvas.height)
  gradient.addColorStop(0, 'rgba(0, 0, 0, 0)')
  gradient.addColorStop(1, overlay)
  ctx.fillStyle = gradient
  ctx.fillRect(0, 0, canvas.width, canvas.height)
  drawTitle(ctx, title, canvas.width, canvas.height)

  return canvas
}

const createCardTexture = async (project: Project) => {
  if (!project.coverUrl) {
    return createTitleOnlyTexture(project.title)
  }

  try {
    const image = await loadImageTexture(project.coverUrl)
    const canvas = addTitleOverlay(image, project.title)
    const texture = new THREE.CanvasTexture(canvas)
    texture.needsUpdate = true
    return texture
  } catch {
    return createTitleOnlyTexture(project.title)
  }
}

const createSceneScaffolding = () => {
  if (!scene) return

  const orbitGroup = new THREE.Group()
  const orbitConfigs = [
    { radius: 11, color: 0x38bdf8, rotationX: Math.PI / 2.4, opacity: 0.18 },
    { radius: 16, color: 0xa855f7, rotationX: Math.PI / 2.15, opacity: 0.14 },
    { radius: 21, color: 0x14b8a6, rotationX: Math.PI / 2.05, opacity: 0.12 }
  ]

  orbitConfigs.forEach((config) => {
    const ring = new THREE.Mesh(
      new THREE.TorusGeometry(config.radius, 0.06, 16, 180),
      new THREE.MeshBasicMaterial({
        color: config.color,
        transparent: true,
        opacity: config.opacity
      })
    )
    ring.rotation.x = config.rotationX
    ring.rotation.y = Math.PI / 10
    orbitGroup.add(ring)
  })

  const core = new THREE.Mesh(
    new THREE.SphereGeometry(2.8, 32, 32),
    new THREE.MeshBasicMaterial({
      color: 0x60a5fa,
      transparent: true,
      opacity: 0.09
    })
  )
  orbitGroup.add(core)
  scene.add(orbitGroup)

  const particleCount = 240
  const positions = new Float32Array(particleCount * 3)
  for (let i = 0; i < particleCount; i += 1) {
    const index = i * 3
    positions[index] = (Math.random() - 0.5) * 90
    positions[index + 1] = (Math.random() - 0.5) * 55
    positions[index + 2] = (Math.random() - 0.5) * 90
  }

  const geometry = new THREE.BufferGeometry()
  geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3))

  const material = new THREE.PointsMaterial({
    color: 0xe0f2fe,
    size: 0.14,
    transparent: true,
    opacity: 0.72,
    blending: THREE.AdditiveBlending,
    sizeAttenuation: true
  })

  ambientParticles = new THREE.Points(geometry, material)
  scene.add(ambientParticles)
}

const createProjectCards = async () => {
  if (!scene) return

  projectOrbitalGroup = new THREE.Group()
  scene.add(projectOrbitalGroup)

  const ringSettings = [
    { radius: 12, yBase: 4.2, speed: 0.16 },
    { radius: 17, yBase: -1.2, speed: -0.11 },
    { radius: 22, yBase: -5.5, speed: 0.08 }
  ]

  const cards = await Promise.all(
    props.projects.map(async (project, index) => {
      const texture = await createCardTexture(project)
      const ring = ringSettings[index % ringSettings.length]
      const angle = (index / Math.max(props.projects.length, 1)) * Math.PI * 2

      const card = new THREE.Mesh(
        new THREE.PlaneGeometry(10, 7.2),
        new THREE.MeshStandardMaterial({
          map: texture,
          transparent: true,
          side: THREE.DoubleSide,
          metalness: 0.08,
          roughness: 0.38,
          emissive: new THREE.Color(0x020617),
          emissiveIntensity: 0.04
        })
      ) as ProjectCardMesh

      card.userData = {
        project,
        orbitRadius: ring.radius,
        orbitAngle: angle,
        orbitSpeed: ring.speed,
        verticalOffset: ring.yBase + Math.sin(index * 1.5) * 1.2,
        floatOffset: index * 0.85,
        depthScale: 0.84 + (index % 3) * 0.06,
        baseScale: 1
      }

      return card
    })
  )

  cards.forEach((card) => {
    projectOrbitalGroup?.add(card)
    projectCards.push(card)
  })
}

const handleBackToList = () => {
  emit('back-to-list')
}

const clearSelection = () => {
  selectedProject.value = null
  if (controls) {
    controls.autoRotate = true
    controls.target.lerp(new THREE.Vector3(0, 0, 0), 0.3)
  }
}

const focusSelectedCard = (project: Project) => {
  if (!camera) return

  const matchedCard = projectCards.find(card => card.userData.project === project)
  if (!matchedCard) return

  const targetPosition = matchedCard.position.clone().normalize().multiplyScalar(28)
  targetPosition.y += 4
  camera.position.lerp(targetPosition, 0.16)
  controls?.target.lerp(matchedCard.position.clone(), 0.4)
}

const onMouseMove = (event: MouseEvent) => {
  if (!raycaster || !camera || !mouse) return
  if (!updateMouseFromEvent(event)) return

  raycaster.setFromCamera(mouse, camera)
  const intersects = raycaster.intersectObjects(projectCards)

  if (intersects.length > 0) {
    hoveredCard = intersects[0].object as ProjectCardMesh
    hoveredProject.value = hoveredCard.userData.project
    if (containerRef.value) {
      containerRef.value.style.cursor = 'pointer'
    }
    return
  }

  hoveredCard = null
  hoveredProject.value = null
  if (containerRef.value) {
    containerRef.value.style.cursor = 'grab'
  }
}

const onPointerLeave = () => {
  hoveredCard = null
  hoveredProject.value = null
  if (containerRef.value) {
    containerRef.value.style.cursor = 'grab'
  }
}

const onMouseClick = (event: MouseEvent) => {
  if (!raycaster || !camera || !mouse) return
  if (!updateMouseFromEvent(event)) return

  raycaster.setFromCamera(mouse, camera)
  const intersects = raycaster.intersectObjects(projectCards)

  if (intersects.length === 0) return

  const clickedCard = intersects[0].object as ProjectCardMesh
  selectedProject.value = clickedCard.userData.project
  focusSelectedCard(clickedCard.userData.project)

  if (controls) {
    controls.autoRotate = false
  }
}

const animate = () => {
  if (!renderer || !scene || !camera) return

  animationFrameId = requestAnimationFrame(animate)
  const elapsed = clock.getElapsedTime()

  controls?.update()

  if (projectOrbitalGroup) {
    projectOrbitalGroup.rotation.y = Math.sin(elapsed * 0.12) * 0.08
    projectOrbitalGroup.rotation.x = Math.sin(elapsed * 0.08) * 0.04
  }

  if (ambientParticles) {
    ambientParticles.rotation.y += 0.0006
    ambientParticles.rotation.x = Math.sin(elapsed * 0.05) * 0.06
  }

  projectCards.forEach((card) => {
    const {
      orbitRadius,
      orbitAngle,
      orbitSpeed,
      verticalOffset,
      floatOffset,
      depthScale,
      baseScale
    } = card.userData

    const angle = orbitAngle + elapsed * orbitSpeed
    card.position.x = Math.cos(angle) * orbitRadius
    card.position.z = Math.sin(angle) * orbitRadius * depthScale
    card.position.y = verticalOffset + Math.sin(elapsed * 1.35 + floatOffset) * 1.15
    card.lookAt(camera.position.x * 0.9, camera.position.y * 0.88, camera.position.z * 0.9)

    const isHovered = hoveredCard === card
    const isSelected = selectedProject.value === card.userData.project
    const breathingScale = baseScale + Math.sin(elapsed * 1.5 + floatOffset) * 0.03
    const targetScale = isSelected ? 1.16 : isHovered ? 1.1 : breathingScale
    const nextScale = THREE.MathUtils.lerp(card.scale.x, targetScale, 0.14)
    card.scale.setScalar(nextScale)

    card.material.emissive.set(isSelected ? 0xa855f7 : isHovered ? 0x38bdf8 : 0x020617)
    card.material.emissiveIntensity = THREE.MathUtils.lerp(
      card.material.emissiveIntensity,
      isSelected ? 0.32 : isHovered ? 0.2 : 0.04,
      0.14
    )
  })

  renderer.render(scene, camera)
}

const handleResize = () => {
  if (!camera || !renderer) return
  const { width, height } = getViewportSize()
  camera.aspect = width / height
  camera.updateProjectionMatrix()
  renderer.setSize(width, height)
}

const initThree = async () => {
  if (!canvasRef.value || !containerRef.value) return

  const { width, height } = getViewportSize()
  scene = new THREE.Scene()
  scene.fog = new THREE.FogExp2(0x060816, 0.018)

  camera = new THREE.PerspectiveCamera(55, width / height, 0.1, 1000)
  camera.position.set(0, 7, 36)

  renderer = new THREE.WebGLRenderer({
    canvas: canvasRef.value,
    antialias: true,
    alpha: true
  })
  renderer.setSize(width, height)
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2))
  renderer.outputColorSpace = THREE.SRGBColorSpace
  renderer.toneMapping = THREE.ACESFilmicToneMapping
  renderer.toneMappingExposure = 1.1

  if (process.client) {
    try {
      const module = await import('three/examples/jsm/controls/OrbitControls.js')
      controls = new module.OrbitControls(camera, renderer.domElement) as OrbitControlsLike
      controls.enableDamping = true
      controls.dampingFactor = 0.06
      controls.minDistance = 18
      controls.maxDistance = 64
      controls.autoRotate = true
      controls.autoRotateSpeed = 0.35
    } catch {
      console.warn('OrbitControls not available, using static camera only')
    }
  }

  scene.add(new THREE.AmbientLight(0xffffff, 1.25))

  const directionalLight = new THREE.DirectionalLight(0xffffff, 1.6)
  directionalLight.position.set(12, 16, 18)
  scene.add(directionalLight)

  const rimLight = new THREE.PointLight(0x38bdf8, 1.4, 120)
  rimLight.position.set(-22, 10, 16)
  scene.add(rimLight)

  const frontLight = new THREE.PointLight(0x3b82f6, 2.1, 120)
  frontLight.position.set(20, 18, 20)
  scene.add(frontLight)

  const backLight = new THREE.PointLight(0xa855f7, 2.4, 120)
  backLight.position.set(-20, -14, 14)
  scene.add(backLight)

  createSceneScaffolding()
  raycaster = new THREE.Raycaster()
  mouse = new THREE.Vector2()

  await createProjectCards()
  animate()
}

onMounted(async () => {
  if (!process.client) return

  await initThree()
  renderer?.domElement.addEventListener('click', onMouseClick)
  renderer?.domElement.addEventListener('mousemove', onMouseMove)
  renderer?.domElement.addEventListener('mouseleave', onPointerLeave)
  window.addEventListener('resize', handleResize)

  if (containerRef.value) {
    containerRef.value.style.cursor = 'grab'
  }
})

onUnmounted(() => {
  renderer?.domElement.removeEventListener('click', onMouseClick)
  renderer?.domElement.removeEventListener('mousemove', onMouseMove)
  renderer?.domElement.removeEventListener('mouseleave', onPointerLeave)
  window.removeEventListener('resize', handleResize)

  if (animationFrameId) {
    cancelAnimationFrame(animationFrameId)
  }

  controls?.dispose?.()
  renderer?.dispose()

  if (ambientParticles) {
    ambientParticles.geometry.dispose()
    if (ambientParticles.material instanceof THREE.Material) {
      ambientParticles.material.dispose()
    }
  }

  projectCards.forEach((card) => {
    card.geometry.dispose()
    if (card.material.map) {
      card.material.map.dispose()
    }
    card.material.dispose()
  })
})
</script>

<style scoped>
.project-3d-scene-bg {
  isolation: isolate;
}

.project-3d-atmosphere {
  position: absolute;
  inset: 0;
  z-index: 0;
  pointer-events: none;
}

.project-3d-atmosphere-glow {
  position: absolute;
  border-radius: 999px;
  filter: blur(100px);
  opacity: 0.5;
}

.project-3d-atmosphere-glow--blue {
  top: 12%;
  left: 14%;
  width: 22rem;
  height: 22rem;
  background: rgba(56, 189, 248, 0.22);
}

.project-3d-atmosphere-glow--violet {
  right: 12%;
  bottom: 10%;
  width: 26rem;
  height: 26rem;
  background: rgba(168, 85, 247, 0.24);
}

.project-3d-atmosphere-grid {
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px),
    linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px);
  background-size: 64px 64px;
  mask-image: radial-gradient(circle at center, rgba(0, 0, 0, 0.8), transparent 88%);
  opacity: 0.8;
}

.project-3d-atmosphere-vignette {
  position: absolute;
  inset: 0;
  background:
    radial-gradient(circle at center, transparent 32%, rgba(2, 6, 23, 0.22) 68%, rgba(2, 6, 23, 0.75) 100%),
    linear-gradient(180deg, rgba(2, 6, 23, 0.2), rgba(2, 6, 23, 0.5));
}

.project-3d-scene-copy {
  position: fixed;
  left: 2rem;
  bottom: 2rem;
  z-index: 100;
  max-width: 26rem;
  padding: 1.25rem 1.35rem;
  border: 1px solid rgba(96, 165, 250, 0.18);
  border-radius: 1.35rem;
  background: rgba(7, 11, 28, 0.72);
  backdrop-filter: blur(18px);
  box-shadow: 0 24px 60px rgba(2, 6, 23, 0.42);
}

.project-3d-scene-kicker {
  display: inline-flex;
  align-items: center;
  padding: 0.35rem 0.7rem;
  border-radius: 999px;
  background: rgba(59, 130, 246, 0.16);
  color: #93c5fd;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.16em;
  text-transform: uppercase;
}

.project-3d-scene-title {
  margin: 1rem 0 0.6rem;
  color: #f8fafc;
  font-size: clamp(1.9rem, 3vw, 2.8rem);
  font-weight: 800;
  line-height: 1.05;
}

.project-3d-scene-description {
  margin: 0;
  color: rgba(226, 232, 240, 0.78);
  font-size: 0.95rem;
  line-height: 1.75;
}

.project-3d-back-button {
  position: fixed;
  top: 6.5rem;
  left: 2rem;
  z-index: 100;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  border: 1px solid rgba(96, 165, 250, 0.22);
  border-radius: 999px;
  background: rgba(9, 14, 32, 0.72);
  backdrop-filter: blur(16px);
  color: #e2e8f0;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  box-shadow: 0 14px 34px rgba(2, 6, 23, 0.34);
  transition: all 0.3s ease;
}

.project-3d-back-button:hover {
  border-color: rgba(96, 165, 250, 0.42);
  background: rgba(15, 23, 42, 0.88);
  transform: translateY(-2px);
  box-shadow: 0 18px 40px rgba(15, 23, 42, 0.45);
}

.project-3d-back-icon {
  width: 1.25rem;
  height: 1.25rem;
  flex-shrink: 0;
}

.project-3d-back-text {
  white-space: nowrap;
}

.project-3d-hint {
  position: fixed;
  top: 6.5rem;
  left: 1rem;
  z-index: 100;
  transform: translateX(calc(100% + 1rem));
  padding: 1rem 1.05rem;
  border: 1px solid rgba(148, 163, 184, 0.16);
  border-radius: 1rem;
  background: rgba(9, 14, 32, 0.72);
  backdrop-filter: blur(16px);
  box-shadow: 0 18px 46px rgba(2, 6, 23, 0.32);
}

.project-3d-hint-content {
  display: flex;
  flex-direction: column;
  gap: 0.55rem;
}

.project-3d-hint-item {
  color: rgba(226, 232, 240, 0.82);
  font-size: 0.8125rem;
  font-weight: 500;
  white-space: nowrap;
}

.project-3d-info-card {
  position: fixed;
  top: 50%;
  right: 2rem;
  z-index: 100;
  max-width: 24rem;
  padding: 1.6rem;
  border: 1px solid rgba(96, 165, 250, 0.16);
  border-radius: 1.5rem;
  background: linear-gradient(180deg, rgba(15, 23, 42, 0.92), rgba(10, 15, 31, 0.94));
  backdrop-filter: blur(18px);
  box-shadow: 0 24px 80px rgba(2, 6, 23, 0.56);
  transform: translateY(-50%);
}

.project-3d-info-kicker {
  margin-bottom: 0.7rem;
  color: #93c5fd;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.16em;
  text-transform: uppercase;
}

.project-3d-info-close {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 2rem;
  height: 2rem;
  border: 1px solid rgba(148, 163, 184, 0.14);
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.08);
  color: #f8fafc;
  font-size: 1.5rem;
  line-height: 1;
  cursor: pointer;
  transition: all 0.2s ease;
}

.project-3d-info-close:hover {
  background: rgba(96, 165, 250, 0.16);
  transform: scale(1.1);
}

.project-3d-info-title {
  margin-bottom: 0.5rem;
  padding-right: 2rem;
  color: #f8fafc;
  font-size: 1.5rem;
  font-weight: 700;
}

.project-3d-info-description {
  margin-bottom: 1rem;
  color: rgba(226, 232, 240, 0.76);
  line-height: 1.6;
}

.project-3d-info-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.55rem;
  margin-bottom: 1.2rem;
}

.project-3d-info-tag {
  display: inline-flex;
  align-items: center;
  padding: 0.38rem 0.72rem;
  border: 1px solid rgba(148, 163, 184, 0.18);
  border-radius: 999px;
  background: rgba(51, 65, 85, 0.8);
  color: #e2e8f0;
  font-size: 0.75rem;
  font-weight: 600;
}

.project-3d-info-actions {
  display: flex;
  gap: 0.75rem;
}

.project-3d-info-button {
  display: inline-block;
  padding: 0.625rem 1.25rem;
  border: 1px solid transparent;
  border-radius: 0.75rem;
  color: #f8fafc;
  font-size: 0.875rem;
  font-weight: 600;
  text-decoration: none;
  transition: all 0.2s ease;
}

.project-3d-info-button-primary {
  background: linear-gradient(135deg, #38bdf8, #6366f1);
  color: #eff6ff;
}

.project-3d-info-button-primary:hover {
  background: linear-gradient(135deg, #22d3ee, #7c3aed);
  transform: translateY(-1px);
  box-shadow: 0 10px 20px rgba(59, 130, 246, 0.24);
}

.project-3d-info-button-secondary {
  border-color: rgba(148, 163, 184, 0.2);
  background: rgba(15, 23, 42, 0.72);
}

.project-3d-info-button-secondary:hover {
  background: rgba(30, 41, 59, 0.88);
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(15, 23, 42, 0.2);
}

.project-3d-hover-hint {
  position: fixed;
  bottom: 2.25rem;
  left: 50%;
  z-index: 100;
  pointer-events: none;
  transform: translateX(-50%);
}

.project-3d-hover-hint-content {
  padding: 0.8rem 1.2rem;
  border: 1px solid rgba(96, 165, 250, 0.2);
  border-radius: 1rem;
  background: rgba(8, 14, 30, 0.78);
  backdrop-filter: blur(16px);
  box-shadow: 0 18px 40px rgba(2, 6, 23, 0.34);
  animation: project-3d-hint-fade-in 0.3s ease;
}

.project-3d-hover-hint-title {
  margin-bottom: 0.25rem;
  color: #f8fafc;
  font-size: 0.875rem;
  font-weight: 600;
}

.project-3d-hover-hint-text {
  color: rgba(191, 219, 254, 0.9);
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

@media (max-width: 900px) {
  .project-3d-scene-copy {
    right: 1rem;
    left: 1rem;
    bottom: 1rem;
    max-width: none;
  }

  .project-3d-back-button {
    top: 5.8rem;
    left: 1rem;
  }

  .project-3d-hint {
    display: none;
  }

  .project-3d-info-card {
    top: auto;
    right: 1rem;
    bottom: 5.75rem;
    left: 1rem;
    max-width: none;
    transform: none;
  }

  .project-3d-info-actions {
    flex-wrap: wrap;
  }
}
</style>
