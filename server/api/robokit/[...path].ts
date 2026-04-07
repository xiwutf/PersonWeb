/**
 * 占位：部分本机环境/扩展会向同源 dev 地址轮询 /api/robokit/*。
 * 若不注册该路由，请求会落到 SPA，Vue Router 无法匹配并刷屏警告。
 * 此处仅返回 204，不实现 Robokit 协议；真实机器人请指向独立服务端口。
 */
export default defineEventHandler((event) => {
  if (event.method === 'OPTIONS') {
    setResponseStatus(event, 204)
    return null
  }
  setResponseStatus(event, 204)
  setResponseHeader(event, 'Cache-Control', 'no-store')
  return null
})
