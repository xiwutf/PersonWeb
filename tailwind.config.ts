import type { Config } from 'tailwindcss'

export default <Partial<Config>>{
    plugins: [
        require('@tailwindcss/typography')
    ],
    theme: {
        extend: {
            colors: {
                // 保留原有的 primary 色阶（用于兼容现有代码）
                primary: {
                    50: 'rgb(var(--color-primary-50) / <alpha-value>)',
                    100: 'rgb(var(--color-primary-100) / <alpha-value>)',
                    200: 'rgb(var(--color-primary-200) / <alpha-value>)',
                    300: 'rgb(var(--color-primary-300) / <alpha-value>)',
                    400: 'rgb(var(--color-primary-400) / <alpha-value>)',
                    500: 'rgb(var(--color-primary-500) / <alpha-value>)',
                    600: 'rgb(var(--color-primary-600) / <alpha-value>)',
                    700: 'rgb(var(--color-primary-700) / <alpha-value>)',
                    800: 'rgb(var(--color-primary-800) / <alpha-value>)',
                    900: 'rgb(var(--color-primary-900) / <alpha-value>)',
                    950: 'rgb(var(--color-primary-950) / <alpha-value>)',
                },
                // 新增语义化颜色，通过 CSS 变量使用，支持多主题切换
                // 主色调（注意：primary 对象已存在，这里使用不同的键名）
                'primary-base': 'var(--color-primary)',
                'primary-soft': 'var(--color-primary-soft)',
                'primary-hover': 'var(--color-primary-hover)',
                // 背景色（支持 bg-bg-body, bg-bg-card 等）
                'bg-body': 'var(--color-bg-body)',
                'bg-card': 'var(--color-bg-card)',
                'bg-elevated': 'var(--color-bg-elevated)',
                // 文字颜色（支持 text-text-main, text-text-muted 等）
                'text-main': 'var(--color-text-main)',
                'text-muted': 'var(--color-text-muted)',
                'text-disabled': 'var(--color-text-disabled)',
                // 边框颜色（支持 border-border-subtle 等）
                'border-subtle': 'var(--color-border-subtle)',
                'border-default': 'var(--color-border-default)',
                'border-strong': 'var(--color-border-strong)',
                // 图表颜色（支持 chart-primary, chart-secondary 等）
                'chart-primary': 'var(--chart-primary)',
                'chart-secondary': 'var(--chart-secondary)',
                'chart-tertiary': 'var(--chart-tertiary)',
                'chart-quaternary': 'var(--chart-quaternary)',
                'chart-quinary': 'var(--chart-quinary)',
                'chart-senary': 'var(--chart-senary)',
                'chart-septenary': 'var(--chart-septenary)',
                'chart-octonary': 'var(--chart-octonary)',
                'chart-nonary': 'var(--chart-nonary)',
                'chart-denary': 'var(--chart-denary)',
                // 为了兼容性，添加 primary 作为 var(--color-primary) 的别名
                // 注意：这会覆盖上面的 primary 对象，但实际使用时优先使用 primary-500 等具体色阶
                // 如果需要直接使用主题主色，可以用 primary-base
            },
            // 扩展圆角，使用 CSS 变量
            borderRadius: {
                'sm': 'var(--radius-sm)',
                'md': 'var(--radius-md)',
                'lg': 'var(--radius-lg)',
                'xl': 'var(--radius-xl)'
            },
            // 扩展阴影，使用 CSS 变量
            boxShadow: {
                'sm': 'var(--shadow-sm)',
                'md': 'var(--shadow-md)',
                'lg': 'var(--shadow-lg)'
            }
        }
    }
}
