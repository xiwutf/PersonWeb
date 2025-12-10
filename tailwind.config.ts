import type { Config } from 'tailwindcss'

export default <Partial<Config>>{
    plugins: [
        require('@tailwindcss/typography')
    ],
    theme: {
        extend: {
            colors: {
                // Aurora Design System V3 Mappings
                primary: 'var(--primary)',
                secondary: 'var(--secondary)',
                accent: 'var(--accent)',

                // Backgrounds - for direct usage: bg-bg-app, bg-bg-surface-1, etc.
                bg: {
                    app: 'var(--bg-app)',
                    'surface-1': 'var(--bg-surface-1)',
                    'surface-2': 'var(--bg-surface-2)',
                    card: 'var(--bg-surface-1)',
                    elevated: 'var(--bg-surface-2)',
                },

                // Text - for direct usage: text-text-primary, text-text-main, etc.
                text: {
                    primary: 'var(--text-primary)',
                    secondary: 'var(--text-secondary)',
                    tertiary: 'var(--text-tertiary)',
                    main: 'var(--text-primary)', // Alias for text-primary
                    muted: 'var(--text-secondary)', // Alias for text-secondary
                    disabled: 'var(--text-tertiary)', // Alias for text-tertiary
                },

                // Borders - for direct usage: border-border-subtle, etc.
                border: {
                    subtle: 'var(--border-subtle)',
                    focus: 'var(--border-focus)',
                },

                // Status
                success: 'var(--success)',
                warning: 'var(--warning)',
                error: 'var(--error)',

                // Legacy Compatibility (Safety Fallback)
                'primary-50': 'rgb(var(--color-primary-50, 239 246 255) / <alpha-value>)',
                'primary-500': 'rgb(var(--color-primary-500, 59 130 246) / <alpha-value>)',
                'primary-600': 'rgb(var(--color-primary-600, 37 99 235) / <alpha-value>)',
            },
            borderRadius: {
                'sm': 'var(--radius-sm)',
                'md': 'var(--radius-md)',
                'lg': 'var(--radius-lg)',
                'xl': 'var(--radius-xl)',
            },
            boxShadow: {
                'sm': 'var(--shadow-sm)',
                'md': 'var(--shadow-md)',
                'lg': 'var(--shadow-lg)',
                'glow': 'var(--shadow-glow)',
            }
        }
    }
}
