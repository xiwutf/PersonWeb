# 脚本说明

本地开发请使用 `npm run dev` 或 `scripts/setup-dev-env.*`。

## 常用命令

| 脚本 | 命令 | 用途 |
| --- | --- | --- |
| `build-css.js` | `npm run build:css` | 合并和压缩 CSS |
| `optimize-images.js` | `npm run optimize:images` | 优化图片 |
| `optimize-project-covers.js` | `npm run optimize:covers` | 优化项目封面 |
| `optimize-all.js` | `npm run optimize:all` | 运行构建前优化 |
| `clean-node-modules.js` | `npm run clean:modules` | 清理依赖缓存 |
| `generate-sitemap.js` | `npm run generate:sitemap` | 生成 sitemap |
| `build-articles-static.js` | `npm run generate:articles` | 生成静态文章索引 |
| `setup-dev-env.ps1` / `.sh` | 见 README | 新环境一键配置 |
| `dev-mobile.ps1` / `.sh` | 见 README | 允许局域网访问的开发服务 |

文章迁移相关脚本在 `scripts/migrate/`，仅在需要从旧 MySQL 正文核对 Git SoT 时使用。
