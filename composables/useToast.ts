/**
 * Toast 通知 Composable
 * 统一管理用户提示消息
 */
import { useToast } from 'vue-toast-notification'
import 'vue-toast-notification/dist/theme-sugar.css'

export const useNotification = () => {
  const toast = useToast({
    position: 'top-right',
    duration: 3000
  })

  return {
    /**
     * 成功提示
     */
    success: (message: string) => {
      toast.success(message, {
        duration: 3000
      })
    },

    /**
     * 错误提示
     */
    error: (message: string) => {
      toast.error(message, {
        duration: 4000
      })
    },

    /**
     * 警告提示
     */
    warning: (message: string) => {
      toast.warning(message, {
        duration: 3000
      })
    },

    /**
     * 信息提示
     */
    info: (message: string) => {
      toast.info(message, {
        duration: 3000
      })
    }
  }
}

