import { readFile } from 'node:fs/promises'
import { join } from 'node:path'
import type { ModuleDefinition, ModuleManifest } from '~/types/module'

/**
 * 读取仓库内 modules/<key>/module.json，供模块商店「安装」流程组装 ModuleManifest。
 */
export default defineEventHandler(async (event) => {
  const moduleKey = getRouterParam(event, 'moduleKey')
  if (!moduleKey) {
    throw createError({ statusCode: 400, statusMessage: '缺少模块标识' })
  }

  const safeKey = moduleKey.replace(/[^a-zA-Z0-9_-]/g, '')
  if (safeKey !== moduleKey) {
    throw createError({ statusCode: 400, statusMessage: '非法模块标识' })
  }

  const filePath = join(process.cwd(), 'modules', moduleKey, 'module.json')

  let raw: string
  try {
    raw = await readFile(filePath, 'utf-8')
  } catch {
    throw createError({ statusCode: 404, statusMessage: '未找到模块清单文件' })
  }

  const parsed = JSON.parse(raw) as ModuleDefinition
  const manifest: ModuleManifest = {
    module: parsed,
    build: {
      timestamp: new Date().toISOString(),
      hash: 'local',
      size: raw.length,
    },
    dependencies: {},
  }

  return {
    success: true,
    data: {
      downloadUrl: '',
      manifest,
    },
  }
})
