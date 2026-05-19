# SEO 修复：首页 403 与 robots/sitemap 404

## 现象（你已确认）

```text
curl -sI https://xifg.com.cn/           → 403 Forbidden（OSS AccessDenied）
curl -sI https://xifg.com.cn/index.html → 200 OK
curl -sI https://xifg.com.cn/robots.txt   → 404
curl -sI https://xifg.com.cn/sitemap.xml → 404
```

Google 默认抓取 `/`，根路径 403 会导致整站无法收录。

---

## 一、修复 Nginx（必做，约 5 分钟）

编辑站点配置（示例路径 `/etc/nginx/sites-available/xifg.com.cn`），在 **HTTPS 主站** `server { server_name xifg.com.cn ... }` 内修改：

### 1. 根路径 `/` 必须返回 index.html

在现有 `location /api/` **之后**、通用 `location /` **之前**加入：

```nginx
# 首页：OSS 对目录 / 会 403，必须显式回源 index.html
location = / {
    proxy_pass http://oss-frontend.xifg.com.cn/index.html;
    proxy_set_header Host oss-frontend.xifg.com.cn;
    proxy_ssl_server_name on;
}
```

### 2. SPA 回退（可选但强烈建议）

未命中文件时回 `200.html`，避免 `/blog/xxx` 变成空文件或裸 404：

```nginx
location / {
    proxy_pass http://oss-frontend.xifg.com.cn;
    proxy_set_header Host oss-frontend.xifg.com.cn;
    proxy_ssl_server_name on;

    proxy_intercept_errors on;
    error_page 404 = @oss_fallback;
}

location @oss_fallback {
    proxy_pass http://oss-frontend.xifg.com.cn/200.html;
    proxy_set_header Host oss-frontend.xifg.com.cn;
    proxy_ssl_server_name on;
}
```

### 3. 重载并验证

```bash
sudo nginx -t && sudo systemctl reload nginx
```

本机验证（三条都应为 **200**）：

```powershell
curl.exe -sI "https://xifg.com.cn/"
curl.exe -sI "https://xifg.com.cn/robots.txt"
curl.exe -sI "https://xifg.com.cn/sitemap.xml"
```

---

## 二、阿里云 OSS 控制台（建议同步检查）

1. **静态网站托管**：默认首页 `index.html`，默认 404 页 `200.html`（与 Nuxt generate 产物一致）。
2. **防盗链**：若开启 Referer 白名单，需允许空 Referer（否则部分爬虫 403）。
3. **读写权限**：通过 Nginx 反代的 Bucket 需允许服务器 IP 读取对象；自定义域名 `oss-frontend.xifg.com.cn` 与 Nginx `proxy_set_header Host` 一致。

---

## 三、部署 robots.txt 与 sitemap.xml

项目已提供：

- `public/robots.txt`
- `scripts/generate-sitemap.js` → 生成 `public/sitemap.xml`

**上传到 OSS 前**在项目根目录执行：

```bash
npm run generate:sitemap   # 或 npm run generate（会自动生成）
npm run generate           # 或你现有的构建/上传流程
```

> `public/sitemap.xml` 由脚本生成，已加入 `.gitignore`，无需提交。

确认 OSS 根目录存在：

- `index.html`
- `200.html`
- `robots.txt`
- `sitemap.xml`

`npm run generate` 会把 `public/` 下文件复制到输出目录，随 `.output/public`（或你实际上传目录）一并上传。

---

## 四、Google Search Console

1. 添加资源 `https://xifg.com.cn`
2. 提交站点地图：`https://xifg.com.cn/sitemap.xml`
3. 网址检查 → 测试 `https://xifg.com.cn/` → 应显示 **可编入索引**
4. 点击「请求编入索引」

修复后 1～2 周 `site:xifg.com.cn` 通常会重新出现结果。

## 上传 robots / sitemap

见 **[UPLOAD_SEO_TO_OSS.md](./UPLOAD_SEO_TO_OSS.md)**（GitHub Actions 一键 / 控制台 / ossutil）。

---

## 五、相关文件

| 文件 | 说明 |
|------|------|
| `public/robots.txt` | 爬虫规则 |
| `scripts/generate-sitemap.js` | 生成 sitemap |
| `docs/archive/legacy/deployment/NGINX_CONFIG_FIX.md` | 历史 Nginx 参考 |
