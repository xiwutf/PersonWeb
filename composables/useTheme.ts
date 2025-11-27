export const useTheme = () => {
    const route = useRoute()

    // Define route to theme mapping
    const themeMapping: Record<string, string> = {
        'tools': 'tools',
        'projects': 'projects',
        'blog': 'blog',
        'ai': 'ai',
        'life': 'life',
        'about': 'about',
        'english': 'english'
    }

    // Update theme based on current route
    const updateTheme = () => {
        const path = route.path.split('/')[1] || 'home'
        const theme = themeMapping[path] || 'home'

        if (process.client) {
            document.documentElement.setAttribute('data-theme', theme)
        }
    }

    // Watch for route changes
    watch(() => route.path, () => {
        updateTheme()
    }, { immediate: true })

    return {
        updateTheme
    }
}
