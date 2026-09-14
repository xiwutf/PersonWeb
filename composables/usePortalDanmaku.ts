import { resolveDotNetApiBase } from '~/utils/admin-runtime-auth'

export interface PortalDanmakuItem {
  id: number
  content: string
  emoji?: string | null
  color?: string | null
  messageType?: string | null
}

interface ApprovedMessagesResponse {
  code?: number
  data?: PortalDanmakuItem[]
  message?: string
}

const DANMAKU_LIMIT = 36

function resolveApiBase(): string {
  const config = useRuntimeConfig()
  const configured = typeof config.public.apiBase === 'string' ? config.public.apiBase : undefined

  if (import.meta.client) {
    return resolveDotNetApiBase(undefined, configured)
  }

  if (configured && !configured.startsWith('/')) {
    return configured.replace(/\/$/, '')
  }

  return 'http://localhost:5234/api'
}

/** 公开弹幕列表：直连 .NET，跳过 admin token 探测，避免首屏多一次 /api/auth/session */
export async function fetchPortalDanmaku(limit = DANMAKU_LIMIT): Promise<PortalDanmakuItem[]> {
  const base = resolveApiBase()
  const response = await $fetch<ApprovedMessagesResponse>(
    `${base}/VisitorInteraction/messages/approved`,
    {
      query: { limit: Math.min(Math.max(limit, 1), DANMAKU_LIMIT) },
    },
  )

  if (response?.code !== undefined && response.code !== 0) {
    throw new Error(response.message || '获取留言失败')
  }

  const list = response?.code === 0 ? response.data : (response as unknown as PortalDanmakuItem[])
  return Array.isArray(list) ? list : []
}

export function usePortalDanmaku() {
  return useLazyAsyncData(
    'portal-danmaku',
    () => fetchPortalDanmaku(),
    {
      // 入口页为静态预渲染；客户端尽早拉数，不阻塞首屏 HTML
      server: false,
      default: () => [] as PortalDanmakuItem[],
    },
  )
}
