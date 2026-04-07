/**
 * 与 [...path].ts 相同：占位 /api/robokit（无子路径时）。
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
