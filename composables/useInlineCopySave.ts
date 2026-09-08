export type InlineCopyEndpoint = '/api/content/life/home' | '/api/content/work/home'

export function useInlineCopySave(endpoint: InlineCopyEndpoint) {
  async function saveField(path: string, value: string) {
    return await $fetch(endpoint, {
      method: 'PATCH',
      credentials: 'include',
      body: { path, value },
    })
  }

  return { saveField }
}
