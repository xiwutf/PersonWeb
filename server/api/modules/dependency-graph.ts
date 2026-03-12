import { ModuleDependencyGraph } from '~/types/module'

// 模拟模块数据
const mockModules = [
  {
    moduleKey: 'ai-assistant',
    dependencies: []
  },
  {
    moduleKey: 'blog',
    dependencies: []
  },
  {
    moduleKey: 'comments',
    dependencies: ['blog']
  },
  {
    moduleKey: 'analytics',
    dependencies: ['blog', 'ai-assistant']
  },
  {
    moduleKey: 'seo-tools',
    dependencies: ['blog', 'analytics']
  },
  {
    moduleKey: 'newsletter',
    dependencies: ['blog', 'comments']
  }
]

export default defineEventHandler(async (event) => {
  try {
    // 构建依赖关系图
    const dependencyGraph: ModuleDependencyGraph = {}

    // 初始化每个模块的依赖关系
    mockModules.forEach(module => {
      dependencyGraph[module.moduleKey] = {
        dependsOn: [],
        dependents: []
      }
    })

    // 填充依赖关系
    mockModules.forEach(module => {
      module.dependencies.forEach(dep => {
        if (dependencyGraph[dep]) {
          dependencyGraph[dep].dependents.push(module.moduleKey)
        }
        if (dependencyGraph[module.moduleKey]) {
          dependencyGraph[module.moduleKey].dependsOn.push(dep)
        }
      })
    })

    // 检测循环依赖
    const hasCycles = detectCycles(dependencyGraph)

    // 获取所有可能的依赖路径
    const allPaths = findAllDependencyPaths(dependencyGraph)

    return {
      success: true,
      data: {
        graph: dependencyGraph,
        hasCycles,
        cycles: hasCycles ? findCycles(dependencyGraph) : [],
        allPaths
      }
    }
  } catch (error) {
    throw createError({
      statusCode: 500,
      statusMessage: '获取依赖关系图失败',
      data: { error: error.message }
    })
  }
})

// 检测是否有循环依赖
function detectCycles(graph: ModuleDependencyGraph): boolean {
  const visited = new Set<string>()
  const recursionStack = new Set<string>()

  function hasCycle(node: string): boolean {
    visited.add(node)
    recursionStack.add(node)

    const neighbors = graph[node].dependsOn
    for (const neighbor of neighbors) {
      if (!visited.has(neighbor)) {
        if (hasCycle(neighbor)) {
          return true
        }
      } else if (recursionStack.has(neighbor)) {
        return true
      }
    }

    recursionStack.delete(node)
    return false
  }

  return Object.keys(graph).some(hasCycle)
}

// 查找所有循环依赖
function findCycles(graph: ModuleDependencyGraph): string[][] {
  const cycles: string[][] = []
  const visited = new Set<string>()
  const path: string[] = []

  function dfs(node: string, start: string) {
    visited.add(node)
    path.push(node)

    for (const neighbor of graph[node].dependsOn) {
      if (neighbor === start && path.length > 1) {
        cycles.push([...path, start])
      } else if (!visited.has(neighbor)) {
        dfs(neighbor, start)
      }
    }

    path.pop()
    visited.delete(node)
  }

  Object.keys(graph).forEach(node => {
    if (!visited.has(node)) {
      dfs(node, node)
    }
  })

  return cycles
}

// 查找所有依赖路径
function findAllDependencyPaths(graph: ModuleDependencyGraph): Record<string, string[][]> {
  const paths: Record<string, string[][]> = {}

  Object.keys(graph).forEach(target => {
    paths[target] = []
    const visited = new Set<string>()
    const currentPath: string[] = []

    function dfs(node: string) {
      visited.add(node)
      currentPath.push(node)

      if (node === target && currentPath.length > 1) {
        paths[target].push([...currentPath])
      } else {
        graph[node].dependents.forEach(dependent => {
          if (!visited.has(dependent)) {
            dfs(dependent)
          }
        })
      }

      currentPath.pop()
      visited.delete(node)
    }

    // 找到所有能够到达target的模块
    Object.keys(graph).forEach(source => {
      if (source !== target && !paths[target].some(p => p[0] === source)) {
        visited.clear()
        currentPath.length = 0
        if (canReach(graph, source, target, new Set())) {
          dfs(source)
        }
      }
    })
  })

  return paths
}

// 检查是否能从源节点到达目标节点
function canReach(
  graph: ModuleDependencyGraph,
  source: string,
  target: string,
  visited: Set<string>
): boolean {
  if (source === target) return true
  if (visited.has(source)) return false

  visited.add(source)

  for (const dependent of graph[source].dependents) {
    if (canReach(graph, dependent, target, visited)) {
      return true
    }
  }

  return false
}