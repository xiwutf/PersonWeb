# 脚本说明

本地开发请使用 `npm run dev` 或 `scripts/setup-dev-env.*`。

## 常用命令

| 脚本 | 命令 | 用途 |
| --- | --- | --- |
| `optimize-images.js` | `npm run optimize:images` | 优化图片 |
| `optimize-project-covers.js` | `npm run optimize:covers` | 优化项目封面 |
| `generate-sitemap.js` | `npm run generate:sitemap` | 生成 sitemap |
| `build-articles-static.js` | `npm run generate:articles` | 生成静态文章索引 |
| `setup-dev-env.ps1` / `.sh` | 见 README | 新环境一键配置 |
| `dev-mobile.ps1` / `.sh` | 见 README | 允许局域网访问的开发服务 |

一次性迁移、旧 CSS 打包、性能基线等脚本仍在 `scripts/`，需要时直接 `node scripts/...` 运行，不再挂到 npm scripts。
