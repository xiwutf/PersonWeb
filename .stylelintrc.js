/**
 * Stylelint 配置文件
 *
 * 用途：强制执行设计系统规范，禁止硬编码间距、圆角、字体大小等
 */

module.exports = {
  extends: [
    'stylelint-config-recommended',
    'stylelint-config-prettier',
  ],
  plugins: [],
  rules: {
    // 禁止在非媒体查询中使用 px 单位（推荐使用 rem）
    'unit-disallowed-allowed': [
      'error',
      {
        ignoreProperties: [
          // 允许的布局属性
          /^width$/,
          /^height$/,
          /^flex$/,
          /^grid$/,
        ]
      }
    ],

    // 建议使用语义变量而非硬编码
    'declaration-property-value-no-unknown': [
      'warn',
      {
        ignoreProperties: [
          // 允许的布局属性
          /^width$/,
          /^height$/,
          /^flex$/,
          /^grid$/,
          // 允许的动画属性
          /^transform$/,
          /^transition$/,
        ]
      }
    ],

    // 字体大小应该是相对单位
    'font-size-no-px': [
      'warn',
      {
        message: '使用相对单位（rem）而非 px 单位',
        ignore: [
          'font-size', // 小字号例外
          'line-height', // 行高例外
        ]
      }
    ],

    // 防止重复的简写属性
    'shorthand-property-no-redundant-mapping': [
      'error',
      {
        severity: 'warning',
      message: '避免重复的 margin/padding 简写',
      ignoreProperties: [
          '--', // 忽略 CSS 变量
        ]
      }
    ],

    // 禁止空的规则块
    'block-no-empty': [
      'warn',
      {
        message: '避免空的规则块'
      }
    ],

    // 禁止选择器中使用空规则
    'selector-pseudo-class-no-empty': [
      'warn',
      {
        message: '避免空的伪类选择器'
      }
    ],

    // 禁止 !important（除特殊情况）
    'declaration-no-important': [
      'warn',
      {
        message: '避免使用 !important，除非必要',
        ignoreProperties: [
          // 在特定主题覆盖场景中允许
          'z-index',
        ]
      }
    ],

    // 禁止多个选择器
    'selector-max-specificity': [
      'warn',
      {
        message: '避免过具体的选择器'
      }
    ],

    // 禁止通用选择器（建议使用类名）
    'selector-class-pattern': [
      'error',
      {
        message: '避免通用选择器，建议使用语义化类名',
        pattern: '^\\.[a-z][a-z0-9]+$',
      }
    ],

    // Vue 特定规则
    'vue/attribute-hyphenation': [
      'error',
      {
        message: 'Vue 组件中属性使用短横线分隔',
        ignoreSelectors: [
          '^v-bind$',
        ]
      }
    ],

    'vue/script-setup-uses-vars': [
      'warn',
      {
        message: 'script setup 中的属性应该使用短横线分隔'
      }
    ],

    'vue/multiline-html-element': [
      'warn',
      {
        message: 'Vue 组件中多行 HTML 元素应该放在单独行'
      }
    ],
  },
}
