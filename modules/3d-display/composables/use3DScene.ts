/**
 * 3D Scene Composable
 * 3D场景管理功能的核心逻辑
 */

import * as THREE from 'three'
import type { SceneObject, LightConfig, CameraConfig } from '~/types/3d'

// 场景配置
interface SceneConfig {
  enableAutoRotate: boolean
  autoRotateSpeed: number
  enableOrbitControls: boolean
  enableZoom: boolean
  maxZoom: number
  enableFog: boolean
  lightIntensity: number
  enableShadows: boolean
  enableStats: boolean
}

// 3D场景对象
interface SceneObject {
  id: string
  name: string
  type: 'mesh' | 'model' | 'light' | 'helper'
  mesh?: THREE.Mesh
  model?: THREE.Object3D
  light?: THREE.Light
  visible: boolean
  position?: [number, number, number]
  rotation?: [number, number, number]
  scale?: [number, number, number]
  material?: any
}

// 灯光配置
interface LightConfig {
  type: 'ambient' | 'directional' | 'point' | 'spot'
  color?: string
  intensity?: number
  position?: [number, number, number]
  target?: [number, number, number]
}

// 相机配置
interface CameraConfig {
  type: 'perspective' | 'orthographic'
  position: [number, number, number]
  target: [number, number, number]
  fov?: number
  aspect?: number
  near?: number
  far?: number
}

/**
 * 3D场景管理Composable
 */
export const use3DScene = () => {
  const scene = ref<THREE.Scene | null>(null)
  const camera = ref<THREE.PerspectiveCamera | THREE.OrthographicCamera | null>(null)
  const renderer = ref<THREE.WebGLRenderer | null>(null)
  const controls = ref<any>(null)
  const stats = ref<any>(null)

  const config = ref<SceneConfig>(getDefaultConfig())
  const objects = ref<SceneObject[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const { isModuleEnabled } = useModuleSystem()

  // 获取默认配置
  function getDefaultConfig(): SceneConfig {
    return {
      enableAutoRotate: true,
      autoRotateSpeed: 1,
      enableOrbitControls: true,
      enableZoom: true,
      maxZoom: 2,
      enableFog: true,
      lightIntensity: 1,
      enableShadows: true,
      enableStats: false
    }
  }

  // 初始化场景
  async function initScene(canvas: HTMLCanvasElement, options?: {
    width?: number
    height?: number
  }) {
    try {
      isLoading.value = true
      error.value = null

      // 创建场景
      scene.value = new THREE.Scene()

      // 创建相机
      camera.value = new THREE.PerspectiveCamera(
        75,
        (options?.width || window.innerWidth) / (options?.height || window.innerHeight),
        0.1,
        1000
      )
      camera.value.position.set(0, 5, 10)
      camera.value.lookAt(0, 0, 0)

      // 创建渲染器
      renderer.value = new THREE.WebGLRenderer({
        canvas,
        antialias: true,
        alpha: true
      })
      renderer.value.setSize(options?.width || window.innerWidth, options?.height || window.innerHeight)
      renderer.value.shadowMap.enabled = config.value.enableShadows

      // 添加灯光
      setupLights()

      // 添加雾效
      if (config.value.enableFog) {
        scene.value.fog = new THREE.Fog(0x000000, 10, 100)
      }

      // 添加轨道控制
      if (config.value.enableOrbitControls) {
        const OrbitControls = (await import('three/examples/jsm/controls/OrbitControls')).OrbitControls
        controls.value = new OrbitControls(camera.value, renderer.value.domElement)
        controls.value.enableDamping = true
        controls.value.dampingFactor = 0.05
        controls.value.enableZoom = config.value.enableZoom
        controls.value.maxDistance = 20 * config.value.maxZoom
        controls.value.minDistance = 5
      }

      // 添加性能统计
      if (config.value.enableStats) {
        const Stats = (await import('three/examples/jsm/libs/stats.module')).Stats
        stats.value = Stats()
        stats.value.showPanel(0)
        document.body.appendChild(stats.value.dom)
      }

      // 开始渲染循环
      animate()

      // 监听配置变化
      watch(config, handleConfigChange, { deep: true })

    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to initialize 3D scene'
      console.error('3D scene initialization failed:', e)
    } finally {
      isLoading.value = false
    }
  }

  // 设置灯光
  function setupLights() {
    if (!scene.value) return

    // 环境光
    const ambientLight = new THREE.AmbientLight(0x404040, config.value.lightIntensity * 0.4)
    scene.value.add(ambientLight)

    // 方向光
    const directionalLight = new THREE.DirectionalLight(0xffffff, config.value.lightIntensity)
    directionalLight.position.set(5, 5, 5)
    directionalLight.castShadow = config.value.enableShadows
    directionalLight.shadow.camera.near = 0.1
    directionalLight.shadow.camera.far = 50
    directionalLight.shadow.camera.left = -10
    directionalLight.shadow.camera.right = 10
    directionalLight.shadow.camera.top = 10
    directionalLight.shadow.camera.bottom = -10
    scene.value.add(directionalLight)

    // 添加灯光辅助
    if (config.value.enableShadows) {
      const helper = new THREE.DirectionalLightHelper(directionalLight, 1)
      scene.value.add(helper)
    }
  }

  // 添加对象
  async function addObject(obj: SceneObject) {
    if (!scene.value) return

    try {
      switch (obj.type) {
        case 'mesh':
          const geometry = new THREE.BoxGeometry(1, 1, 1)
          const material = new THREE.MeshPhongMaterial({
            color: 0x00ff00,
            transparent: true,
            opacity: 0.8
          })
          const mesh = new THREE.Mesh(geometry, material)
          mesh.castShadow = config.value.enableShadows
          mesh.receiveShadow = config.value.enableShadows
          scene.value.add(mesh)
          obj.mesh = mesh
          break

        case 'model':
          // 加载3D模型
          const loader = new THREE.GLTFLoader()
          const model = await loader.loadAsync(obj.model as string)
          scene.value.add(model.scene)
          obj.model = model.scene
          break

        case 'light':
          // 添加灯光
          const light = new THREE.PointLight(0xffffff, 1, 100)
          light.position.set(...(obj.position || [0, 5, 0]))
          scene.value.add(light)
          obj.light = light
          break
      }

      obj.visible = true
      objects.value.push(obj)

    } catch (e) {
      console.error('Failed to add object:', e)
    }
  }

  // 移除对象
  function removeObject(id: string) {
    const index = objects.value.findIndex(obj => obj.id === id)
    if (index === -1) return

    const obj = objects.value[index]
    if (scene.value && obj.mesh) {
      scene.value.remove(obj.mesh)
    }
    if (scene.value && obj.model) {
      scene.value.remove(obj.model)
    }
    if (scene.value && obj.light) {
      scene.value.remove(obj.light)
    }

    objects.value.splice(index, 1)
  }

  // 更新对象
  function updateObject(id: string, updates: Partial<SceneObject>) {
    const obj = objects.value.find(o => o.id === id)
    if (!obj || !scene.value) return

    // 更新位置
    if (updates.position && obj.mesh) {
      obj.mesh.position.set(...updates.position)
    }

    // 更新旋转
    if (updates.rotation && obj.mesh) {
      obj.mesh.rotation.set(...updates.rotation)
    }

    // 更新缩放
    if (updates.scale && obj.mesh) {
      obj.mesh.scale.set(...updates.scale)
    }

    // 更新可见性
    if (updates.visible !== undefined && obj.mesh) {
      obj.visible = updates.visible
      obj.mesh.visible = updates.visible
    }
  }

  // 处理配置变化
  function handleConfigChange(newConfig: SceneConfig) {
    if (!scene.value || !renderer.value || !camera.value) return

    // 更新渲染器设置
    renderer.value.shadowMap.enabled = newConfig.enableShadows

    // 更新相机控制
    if (controls.value) {
      controls.value.enableZoom = newConfig.enableZoom
      controls.value.maxDistance = 20 * newConfig.maxZoom
    }

    // 更新雾效
    if (newConfig.enableFog) {
      scene.value.fog = new THREE.Fog(0x000000, 10, 100)
    } else {
      scene.value.fog = null
    }

    // 更新灯光
    scene.value.traverse(child => {
      if (child instanceof THREE.Light) {
        child.intensity = newConfig.lightIntensity
      }
    })
  }

  // 动画循环
  function animate() {
    requestAnimationFrame(animate)

    if (controls.value) {
      controls.value.update()
    }

    if (stats.value) {
      stats.value.update()
    }

    if (scene.value && camera.value && renderer.value) {
      renderer.value.render(scene.value, camera.value)
    }
  }

  // 调整画布大小
  function resize(width: number, height: number) {
    if (!camera.value || !renderer.value) return

    const aspect = width / height
    if (camera.value instanceof THREE.PerspectiveCamera) {
      camera.value.aspect = aspect
      camera.value.updateProjectionMatrix()
    }

    renderer.value.setSize(width, height)
  }

  // 销毁场景
  function dispose() {
    if (renderer.value) {
      renderer.value.dispose()
    }
    if (controls.value) {
      controls.value.dispose()
    }
    if (stats.value) {
      stats.value.dom.remove()
    }
    objects.value = []
    scene.value = null
    camera.value = null
    renderer.value = null
  }

  // 获取场景统计信息
  function getSceneStats() {
    return {
      objectCount: objects.value.length,
      triangleCount: calculateTriangleCount(),
      drawCalls: renderer.value?.info?.render?.calls || 0,
      memoryUsage: renderer.value?.info?.memory?.geometries || 0
    }
  }

  // 计算三角形数量
  function calculateTriangleCount(): number {
    let count = 0
    scene.value?.traverse(child => {
      if (child instanceof THREE.Mesh) {
        if (child.geometry) {
          const geometry = child.geometry
          if (geometry.index) {
            count += geometry.index.count / 3
          } else if (geometry.attributes.position) {
            count += geometry.attributes.position.count / 3
          }
        }
      }
    })
    return Math.floor(count)
  }

  // 加载3D模型
  async function loadModel(url: string): Promise<THREE.Group> {
    const loader = new THREE.GLTFLoader()
    const gltf = await loader.loadAsync(url)
    return gltf.scene
  }

  // 导出场景配置
  function exportSceneConfig() {
    return {
      objects: objects.value.map(obj => ({
        id: obj.id,
        name: obj.name,
        type: obj.type,
        position: obj.position,
        rotation: obj.rotation,
        scale: obj.scale,
        visible: obj.visible
      })),
      config: config.value
    }
  }

  return {
    // 状态
    scene: readonly(scene),
    camera: readonly(camera),
    renderer: readonly(renderer),
    controls: readonly(controls),
    stats: readonly(stats),
    config,
    objects,
    isLoading,
    error,

    // 方法
    initScene,
    addObject,
    removeObject,
    updateObject,
    resize,
    dispose,
    getSceneStats,
    loadModel,
    exportSceneConfig,

    // 工具
    calculateTriangleCount
  }
}

/**
 * 3D场景工具函数
 */
export const use3DTools = () => {
  /**
   * 创建基础几何体
   */
  function createGeometry(type: 'box' | 'sphere' | 'cylinder' | 'cone', options: {
    width?: number
    height?: number
    depth?: number
    radius?: number
    segments?: number
  } = {}): THREE.BufferGeometry {
    switch (type) {
      case 'box':
        return new THREE.BoxGeometry(options.width || 1, options.height || 1, options.depth || 1)
      case 'sphere':
        return new THREE.SphereGeometry(options.radius || 1, options.segments || 32, options.segments || 16)
      case 'cylinder':
        return new THREE.CylinderGeometry(
          options.radius || 1,
          options.radius || 1,
          options.height || 1,
          options.segments || 32
        )
      case 'cone':
        return new THREE.ConeGeometry(
          options.radius || 1,
          options.height || 1,
          options.segments || 32
        )
      default:
        return new THREE.BoxGeometry(1, 1, 1)
    }
  }

  /**
   * 创建材质
   */
  function createMaterial(options: {
    color?: string
    metalness?: number
    roughness?: number
    transparent?: boolean
    opacity?: number
  } = {}): THREE.MeshStandardMaterial {
    return new THREE.MeshStandardMaterial({
      color: options.color || '#00ff00',
      metalness: options.metalness || 0.5,
      roughness: options.roughness || 0.5,
      transparent: options.transparent || false,
      opacity: options.opacity || 1
    })
  }

  /**
   * 创建灯光
   */
  function createLight(type: 'ambient' | 'directional' | 'point' | 'spot', options: {
    color?: string
    intensity?: number
    position?: [number, number, number]
  } = {}): THREE.Light {
    const color = options.color || '#ffffff'
    const intensity = options.intensity || 1

    switch (type) {
      case 'ambient':
        return new THREE.AmbientLight(color, intensity)
      case 'directional':
        const light = new THREE.DirectionalLight(color, intensity)
        if (options.position) {
          light.position.set(...options.position)
        }
        return light
      case 'point':
        const pointLight = new THREE.PointLight(color, intensity)
        if (options.position) {
          pointLight.position.set(...options.position)
        }
        return pointLight
      case 'spot':
        const spotLight = new THREE.SpotLight(color, intensity)
        if (options.position) {
          spotLight.position.set(...options.position)
        }
        return spotLight
      default:
        return new THREE.AmbientLight(color, intensity)
    }
  }

  return {
    createGeometry,
    createMaterial,
    createLight
  }
}