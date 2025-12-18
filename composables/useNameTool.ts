/**
 * 智能取名助手 Composable
 * 封装 API 调用、状态管理、错误处理
 */

import type {
  NameGenerateRequest,
  NameGenerateResponse,
  NameFavorite,
  FavoriteCreateRequest,
  FavoriteListResponse
} from '~/types/name-tool'

export const useNameTool = () => {
  const api = useApi()
  const message = useSafeMessage()

  // 生成名字
  const generateNames = async (
    request: NameGenerateRequest
  ): Promise<NameGenerateResponse | null> => {
    try {
      const response = await api.post<NameGenerateResponse>(
        '/NameTool/generate',
        request
      )
      return response
    } catch (error: any) {
      console.error('生成名字失败:', error)
      message.error(error.message || '生成名字失败，请重试')
      return null
    }
  }

  // 再来一批（重新生成）
  const regenerateNames = async (
    request: NameGenerateRequest
  ): Promise<NameGenerateResponse | null> => {
    try {
      const response = await api.post<NameGenerateResponse>(
        '/NameTool/regenerate',
        request
      )
      return response
    } catch (error: any) {
      console.error('重新生成名字失败:', error)
      message.error(error.message || '重新生成失败，请重试')
      return null
    }
  }

  // 获取收藏列表
  const getFavorites = async (
    page: number = 1,
    pageSize: number = 20
  ): Promise<FavoriteListResponse | null> => {
    try {
      const response = await api.get<FavoriteListResponse>(
        '/NameTool/favorites',
        {
          params: { page, pageSize }
        }
      )
      return response
    } catch (error: any) {
      console.error('获取收藏列表失败:', error)
      message.error(error.message || '获取收藏列表失败')
      return null
    }
  }

  // 收藏名字
  const addFavorite = async (
    favorite: FavoriteCreateRequest
  ): Promise<NameFavorite | null> => {
    try {
      const response = await api.post<NameFavorite>(
        '/NameTool/favorites',
        favorite
      )
      message.success('收藏成功')
      return response
    } catch (error: any) {
      console.error('收藏失败:', error)
      message.error(error.message || '收藏失败，请重试')
      return null
    }
  }

  // 取消收藏
  const removeFavorite = async (id: number): Promise<boolean> => {
    try {
      await api.delete(`/NameTool/favorites/${id}`)
      message.success('已取消收藏')
      return true
    } catch (error: any) {
      console.error('取消收藏失败:', error)
      message.error(error.message || '取消收藏失败，请重试')
      return false
    }
  }

  // 复制名字到剪贴板
  const copyName = async (name: string): Promise<boolean> => {
    try {
      if (process.client && navigator.clipboard) {
        await navigator.clipboard.writeText(name)
        message.success('已复制到剪贴板')
        return true
      } else {
        // 降级方案：使用传统方法
        const textArea = document.createElement('textarea')
        textArea.value = name
        textArea.style.position = 'fixed'
        textArea.style.opacity = '0'
        document.body.appendChild(textArea)
        textArea.select()
        document.execCommand('copy')
        document.body.removeChild(textArea)
        message.success('已复制到剪贴板')
        return true
      }
    } catch (error) {
      console.error('复制失败:', error)
      message.error('复制失败，请手动复制')
      return false
    }
  }

  return {
    generateNames,
    regenerateNames,
    getFavorites,
    addFavorite,
    removeFavorite,
    copyName
  }
}

