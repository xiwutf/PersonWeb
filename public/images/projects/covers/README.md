# 项目封面图

本目录存放案例墙自动匹配的封面资源。后端 `CoverUrl` 为空时使用。

- **WebP**（`*.webp`）：主要展示用封面，由 `npm run optimize:covers` 从 PNG 生成（宽 ≤1024，quality 80）
- **SVG**（`*.svg`）：轻量占位与未命名项目备选

| WebP | 匹配项目 |
| --- | --- |
| `mindtrace.webp` | MindTrace / Chrome 扩展 |
| `personweb.webp` | 个人数字资产平台 / 本网站 |
| `personweb-system.webp` | 个人网站系统 |
| `labs.webp` | 实验室 / Three.js |
| `ai-service.webp` | AI Service / Python 服务 |
| `tools.webp` | 开发者工具箱 |
| `ai-assistant.webp` | AI 创作助手 / AI 智能助手 |
| `tool-market.webp` | 工具市场平台 |
| `theme-store.webp` | 主题商店 |
| `love-cube.webp` | 恋爱魔方 |
| `iot-control.webp` | 智能物联网 |
| `finance-assistant.webp` | 智能理财助手 |
| `investment-system.webp` | 投资管理系统 |

| SVG | 用途 |
| --- | --- |
| `analytics.svg` | 访客分析系统 |
| `variant-a~d.svg` | 其他未命名项目的备选封面 |
| `default.svg` | 最终兜底 |

映射规则见 `constants/projects/covers.ts`。
