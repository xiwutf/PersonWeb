/**
 * 统一错误处理 Composable
 */
import { useNotification } from './useToast'

export const useErrorHandler = () => {
  const { error } = useNotification()

  /**
   * 处理错误
   * @param err 错误对象
   * @param defaultMessage 默认错误消息
   */
  const handleError = (err: unknown, defaultMessage = '操作失败，请稍后重试') => {
    let errorMessage = defaultMessage

    if (err instanceof Error) {
      errorMessage = err.message
    } else if (typeof err === 'object' && err !== null) {
      // 处理 API 错误响应
      if ('message' in err) {
        errorMessage = String(err.message)
      } else if ('response' in err) {
        const response = (err as any).response
        if (response?.data?.message) {
          errorMessage = response.data.message
        } else if (response?.statusText) {
          errorMessage = response.statusText
        }
      }
    } else if (typeof err === 'string') {
      errorMessage = err
    }

    // 开发环境输出详细错误信息
    if (process.env.NODE_ENV === 'development') {
      console.error('[ErrorHandler]', err)
    }

    // 显示错误提示
    error(errorMessage)

    return errorMessage
  }

  return {
    handleError
  }
}

