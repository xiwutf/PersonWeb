/**
 * 依赖关系图功能单元测试
 */

import { describe, it, expect, beforeEach } from 'vitest'

// Types
interface ModuleDependencyGraph {
  [key: string]: {
    dependsOn: string[]
    dependents: string[]
  }
}

describe('Dependency Graph', () => {
  let mockModules: any[]

  beforeEach(() => {
    mockModules = [
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
  })

  describe('Building Dependency Graph', () => {
    it('should build dependency graph from modules', () => {
      const graph: ModuleDependencyGraph = {}

      mockModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      expect(Object.keys(graph)).toHaveLength(6)
      expect(graph['ai-assistant']).toBeDefined()
      expect(graph['blog']).toBeDefined()
    })

    it('should fill dependency relationships', () => {
      const graph: ModuleDependencyGraph = {}

      mockModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      mockModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
          if (graph[module.moduleKey]) {
            graph[module.moduleKey].dependsOn.push(dep)
          }
        })
      })

      expect(graph['comments'].dependsOn).toContain('blog')
      expect(graph['analytics'].dependsOn).toContain('blog')
      expect(graph['analytics'].dependsOn).toContain('ai-assistant')
    })

    it('should track dependents correctly', () => {
      const graph: ModuleDependencyGraph = {}

      mockModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      mockModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
        })
      })

      expect(graph['blog'].dependents).toContain('comments')
      expect(graph['blog'].dependents).toContain('analytics')
      expect(graph['blog'].dependents).toContain('seo-tools')
    })
  })

  describe('Cycle Detection', () => {
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

    it('should detect no cycles in valid dependency graph', () => {
      const graph: ModuleDependencyGraph = {}

      mockModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      mockModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
          if (graph[module.moduleKey]) {
            graph[module.moduleKey].dependsOn.push(dep)
          }
        })
      })

      expect(detectCycles(graph)).toBe(false)
    })

    it('should detect cycles in invalid dependency graph', () => {
      const cyclicModules = [
        { moduleKey: 'a', dependencies: ['b'] },
        { moduleKey: 'b', dependencies: ['c'] },
        { moduleKey: 'c', dependencies: ['a'] }
      ]

      const graph: ModuleDependencyGraph = {}

      cyclicModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      cyclicModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
          if (graph[module.moduleKey]) {
            graph[module.moduleKey].dependsOn.push(dep)
          }
        })
      })

      expect(detectCycles(graph)).toBe(true)
    })

    it('should detect self-reference as cycle', () => {
      const graph: ModuleDependencyGraph = {
        'self-refer': {
          dependsOn: ['self-refer'],
          dependents: []
        }
      }

      expect(detectCycles(graph)).toBe(true)
    })
  })

  describe('Finding Cycles', () => {
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

    it('should find no cycles in valid graph', () => {
      const graph: ModuleDependencyGraph = {}

      mockModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      mockModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
          if (graph[module.moduleKey]) {
            graph[module.moduleKey].dependsOn.push(dep)
          }
        })
      })

      expect(findCycles(graph)).toHaveLength(0)
    })

    it('should find cycle in cyclic graph', () => {
      const cyclicModules = [
        { moduleKey: 'a', dependencies: ['b'] },
        { moduleKey: 'b', dependencies: ['c'] },
        { moduleKey: 'c', dependencies: ['a'] }
      ]

      const graph: ModuleDependencyGraph = {}

      cyclicModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      cyclicModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
          if (graph[module.moduleKey]) {
            graph[module.moduleKey].dependsOn.push(dep)
          }
        })
      })

      const cycles = findCycles(graph)

      expect(cycles.length).toBeGreaterThan(0)
      expect(cycles[0]).toContain('a')
      expect(cycles[0]).toContain('b')
      expect(cycles[0]).toContain('c')
    })
  })

  describe('Reachability Check', () => {
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

    it('should check reachability between modules', () => {
      const graph: ModuleDependencyGraph = {}

      mockModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      mockModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
          if (graph[module.moduleKey]) {
            graph[module.moduleKey].dependsOn.push(dep)
          }
        })
      })

      expect(canReach(graph, 'blog', 'comments', new Set())).toBe(true)
      expect(canReach(graph, 'blog', 'analytics', new Set())).toBe(true)
      expect(canReach(graph, 'ai-assistant', 'analytics', new Set())).toBe(true)
    })

    it('should not reach unrelated modules', () => {
      const graph: ModuleDependencyGraph = {}

      mockModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      mockModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
          if (graph[module.moduleKey]) {
            graph[module.moduleKey].dependsOn.push(dep)
          }
        })
      })

      expect(canReach(graph, 'comments', 'ai-assistant', new Set())).toBe(false)
      expect(canReach(graph, 'newsletter', 'analytics', new Set())).toBe(false)
    })
  })

  describe('Dependency Path Finding', () => {
    function findAllDependencyPaths(graph: ModuleDependencyGraph): Record<string, string[][]> {
      const paths: Record<string, string[][]> = {}

      Object.keys(graph).forEach(target => {
        paths[target] = []

        const canReach = (
          source: string,
          target: string,
          visited: Set<string>
        ): boolean => {
          if (source === target) return true
          if (visited.has(source)) return false

          visited.add(source)

          for (const dependent of graph[source].dependents) {
            if (canReach(dependent, target, visited)) {
              return true
            }
          }

          return false
        }

        // Find all modules that can reach target
        Object.keys(graph).forEach(source => {
          if (source !== target && canReach(source, target, new Set())) {
            // Simplified path finding - just indicate reachability
            paths[target].push([source, target])
          }
        })
      })

      return paths
    }

    it('should find dependency paths', () => {
      const graph: ModuleDependencyGraph = {}

      mockModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      mockModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
          if (graph[module.moduleKey]) {
            graph[module.moduleKey].dependsOn.push(dep)
          }
        })
      })

      const paths = findAllDependencyPaths(graph)

      expect(paths['comments']).toBeDefined()
      expect(paths['analytics']).toBeDefined()
      expect(paths['seo-tools']).toBeDefined()
    })
  })

  describe('Topological Sort', () => {
    function topologicalSort(graph: ModuleDependencyGraph): string[] {
      const visited = new Set<string>()
      const result: string[] = []

      function visit(node: string) {
        if (visited.has(node)) return
        visited.add(node)

        for (const dep of graph[node].dependsOn) {
          visit(dep)
        }

        result.push(node)
      }

      Object.keys(graph).forEach(node => visit(node))

      return result
    }

    it('should return sorted modules', () => {
      const graph: ModuleDependencyGraph = {}

      mockModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      mockModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
          if (graph[module.moduleKey]) {
            graph[module.moduleKey].dependsOn.push(dep)
          }
        })
      })

      const sorted = topologicalSort(graph)

      expect(sorted).toHaveLength(6)
    })

    it('dependencies should come before dependents', () => {
      const graph: ModuleDependencyGraph = {}

      mockModules.forEach(module => {
        graph[module.moduleKey] = {
          dependsOn: [],
          dependents: []
        }
      })

      mockModules.forEach(module => {
        module.dependencies.forEach((dep: string) => {
          if (graph[dep]) {
            graph[dep].dependents.push(module.moduleKey)
          }
          if (graph[module.moduleKey]) {
            graph[module.moduleKey].dependsOn.push(dep)
          }
        })
      })

      const sorted = topologicalSort(graph)
      const blogIndex = sorted.indexOf('blog')
      const commentsIndex = sorted.indexOf('comments')

      expect(blogIndex).toBeLessThan(commentsIndex)
    })
  })

  describe('Dependency Validation', () => {
    it('should validate module key exists', () => {
      const existingKeys = mockModules.map(m => m.moduleKey)

      expect(existingKeys).toContain('ai-assistant')
      expect(existingKeys).toContain('blog')
      expect(existingKeys).toContain('comments')
    })

    it('should validate dependency exists', () => {
      const existingKeys = mockModules.map(m => m.moduleKey)
      const comments = mockModules.find(m => m.moduleKey === 'comments')

      expect(comments?.dependencies).toBeDefined()
      comments?.dependencies.forEach((dep: string) => {
        expect(existingKeys).toContain(dep)
      })
    })

    it('should prevent self-dependency', () => {
      const validDependencies = mockModules.filter(
        m => !m.dependencies.includes(m.moduleKey)
      )

      expect(validDependencies).toHaveLength(mockModules.length)
    })
  })
})
