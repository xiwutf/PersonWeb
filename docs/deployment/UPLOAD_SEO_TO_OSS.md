# 上传 robots.txt / sitemap.xml 到 OSS

首页 Nginx 已修好后，还需把这两个文件放到 OSS **根目录**（与 `index.html` 同级）。

## 方式一：GitHub Actions（推荐，若已配置 Secrets）

1. 将本次代码 push 到 GitHub
2. 打开仓库 → **Actions** → **Deploy SEO files to OSS**
3. 点 **Run workflow** → **Run workflow**
4. 等绿色 ✓ 后，在服务器执行：
   ```bash
   curl -sI https://xifg.com.cn/robots.txt
   curl -s https://xifg.com.cn/robots.txt | head -5
   ```
   应看到 `User-agent: *`，且 `content-length` 很小（不是 26 万字节）

所需 Secrets（与全量前端部署相同）：`OSS_BUCKET`、`OSS_ENDPOINT`、`OSS_AK`、`OSS_SK`

## 方式二：阿里云控制台（无需命令行）

1. 登录 [OSS 控制台](https://oss.console.aliyun.com/)
2. 打开绑定域名 `oss-frontend.xifg.com.cn` 的 **Bucket**
3. 进入 **文件管理** → 根目录
4. 上传两个文件（sitemap 需先本地生成）：
   - `public/robots.txt`
   - 执行 `node scripts/generate-sitemap.js` 后得到 `public/sitemap.xml`（该文件不提交 Git）
5. 覆盖同名文件（若存在）

## 方式三：在 ECS 上用 ossutil

```bash
# 安装 ossutil 后配置 ~/.ossutilconfig，然后：
export OSS_BUCKET=你的桶名
cd /path/to/PersonWeb
bash scripts/upload-seo-to-oss.sh
```

## 可选：Nginx 单独处理 robots/sitemap（避免 404 时回退成 200.html）

在 `location = /` 之后、`location /` 之前增加：

```nginx
    location = /robots.txt {
        proxy_pass http://oss-frontend.xifg.com.cn/robots.txt;
        proxy_set_header Host oss-frontend.xifg.com.cn;
        proxy_ssl_server_name on;
    }

    location = /sitemap.xml {
        proxy_pass http://oss-frontend.xifg.com.cn/sitemap.xml;
        proxy_set_header Host oss-frontend.xifg.com.cn;
        proxy_ssl_server_name on;
    }
```

上传 OSS 后执行 `nginx -t && systemctl reload nginx`。

## 验证

```bash
curl -sI https://xifg.com.cn/robots.txt
curl -s https://xifg.com.cn/robots.txt | head -8
curl -sI https://xifg.com.cn/sitemap.xml
```

## Google Search Console

1. https://search.google.com/search-console
2. 添加 `https://xifg.com.cn`
3. 提交站点地图：`https://xifg.com.cn/sitemap.xml`
4. 对首页请求编入索引
