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
                'primary-hover': 'var(--color-primary-hover)',
                'primary-soft': 'var(--color-primary-soft)',
                secondary: 'var(--secondary)',
                accent: 'var(--accent)',

                // Backgrounds - for direct usage: bg-bg-app, bg-bg-body, bg-bg-primary, etc.
                bg: {
                    body: 'var(--color-bg-body)',
                    app: 'var(--bg-app)',
                    primary: 'var(--color-bg-body)',
                    secondary: 'var(--color-bg-elevated)',
                    light: 'var(--color-bg-light)',
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
                    main: 'var(--color-text-main)', // 使用新的 CSS 变量
                    muted: 'var(--color-text-muted)', // 使用新的 CSS 变量
                    disabled: 'var(--color-text-disabled)', // 使用新的 CSS 变量
                },

                // Borders - for direct usage: border-border-subtle, border-border-default, etc.
                border: {
                    subtle: 'var(--border-subtle)',
                    default: 'var(--color-border-default)',
                    focus: 'var(--border-focus)',
                },

                // Status
                success: 'var(--success)',
                'success-hover': 'var(--color-success-hover)',
                'success-soft': 'var(--color-success-soft)',
                warning: 'var(--warning)',
                'warning-hover': 'var(--color-warning-hover)',
                'warning-soft': 'var(--color-warning-soft)',
                error: 'var(--error)',
                'error-hover': 'var(--color-error-hover)',
                'error-soft': 'var(--color-error-soft)',
                info: 'var(--color-info)',
                'info-hover': 'var(--color-info-hover)',
                'info-soft': 'var(--color-info-soft)',

                // Chart colors - for direct usage: text-chart-primary, bg-chart-secondary, etc.
                chart: {
                    primary: 'var(--chart-primary)',
                    secondary: 'var(--chart-secondary)',
                    tertiary: 'var(--chart-tertiary)',
                    quaternary: 'var(--chart-quaternary)',
                    quinary: 'var(--chart-quinary)',
                    senary: 'var(--chart-senary)',
                    septenary: 'var(--chart-septenary)',
                    octonary: 'var(--chart-octonary)',
                    nonary: 'var(--chart-nonary)',
                    denary: 'var(--chart-denary)',
                },

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
