# Nginx 配置修复说明

## 问题发现

您的 Nginx 配置中，后端服务端口配置为 `5000`，但实际后端服务运行在 `5234` 端口。

## 需要修改的地方

### 1. 主站 HTTPS 配置（xifg.com.cn）

**当前配置**：
```nginx
location /api/ {
    proxy_pass http://127.0.0.1:5000;  # ❌ 错误：应该是 5234
    ...
}
```

**应该改为**：
```nginx
location /api/ {
    proxy_pass http://127.0.0.1:5234;  # ✅ 正确：后端实际运行在 5234 端口
    ...
}
```

### 2. API 子域名配置（api.xifg.com.cn）

**当前配置**：
```nginx
location / {
    proxy_pass http://127.0.0.1:5000;  # ❌ 错误：应该是 5234
    ...
}
```

**应该改为**：
```nginx
location / {
    proxy_pass http://127.0.0.1:5234;  # ✅ 正确：后端实际运行在 5234 端口
    ...
}
```

## 完整的正确配置

```nginx
# 1. 所有 HTTP 请求强制跳转到 HTTPS
server {
    listen 80 default_server;
    listen [::]:80 default_server;
    server_name xifg.com.cn www.xifg.com.cn api.xifg.com.cn;

    return 301 https://$host$request_uri;
}

# 2. HTTPS：主站 (xifg.com.cn / www.xifg.com.cn)
server {
    listen 443 ssl http2 default_server;
    listen [::]:443 ssl http2 default_server;
    server_name xifg.com.cn www.xifg.com.cn;

    # ===== SSL 证书配置 =====
    ssl_certificate     /etc/letsencrypt/live/xifg.com.cn/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/xifg.com.cn/privkey.pem;
    include /etc/letsencrypt/options-ssl-nginx.conf;
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem;

    # ===== API 反向代理：/api/ 走 .NET 后端 =====
    location /api/ {
        # ✅ 修复：后端服务实际运行在 5234 端口（不是 5000）
        proxy_pass http://127.0.0.1:5234;

        # ===== 转发客户端真实 IP（修复线上访问仍然显示未知的问题）=====
        proxy_set_header Host              $host;
        proxy_set_header X-Real-IP         $remote_addr;
        proxy_set_header X-Forwarded-For   $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-Host  $host;

        proxy_http_version 1.1;
        proxy_set_header Upgrade           $http_upgrade;
        proxy_set_header Connection        keep-alive;
        proxy_cache_bypass                 $http_upgrade;

        # 超时设置（可选，但建议配置）
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # ===== 前端静态站：其余所有路径反代到 OSS =====
    location / {
        proxy_pass http://oss-frontend.xifg.com.cn;
        proxy_set_header Host oss-frontend.xifg.com.cn;
        proxy_ssl_server_name on;
    }
}

# 3. HTTPS：API 子域名 (api.xifg.com.cn)
server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name api.xifg.com.cn;

    ssl_certificate     /etc/letsencrypt/live/xifg.com.cn/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/xifg.com.cn/privkey.pem;
    include /etc/letsencrypt/options-ssl-nginx.conf;
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem;

    # 整个 api.xifg.com.cn 都是 API 服务
    location / {
        # ✅ 修复：后端服务实际运行在 5234 端口（不是 5000）
        proxy_pass http://127.0.0.1:5234;

        # ===== 转发客户端真实 IP（修复线上访问仍然显示未知的问题）=====
        proxy_set_header Host              $host;
        proxy_set_header X-Real-IP         $remote_addr;
        proxy_set_header X-Forwarded-For   $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-Host  $host;

        proxy_http_version 1.1;
        proxy_set_header Upgrade           $http_upgrade;
        proxy_set_header Connection        keep-alive;
        proxy_cache_bypass                 $http_upgrade;

        # 超时设置（可选，但建议配置）
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }
}
```

## 修改步骤

1. **编辑 Nginx 配置文件**：
   ```bash
   sudo nano /etc/nginx/sites-available/xifg.com.cn
   # 或
   sudo nano /etc/nginx/nginx.conf
   ```

2. **修改端口**：
   - 将所有 `proxy_pass http://127.0.0.1:5000;` 改为 `proxy_pass http://127.0.0.1:5234;`

3. **测试配置**：
   ```bash
   sudo nginx -t
   ```

4. **重新加载 Nginx**：
   ```bash
   sudo nginx -s reload
   # 或
   sudo systemctl reload nginx
   ```

5. **验证后端服务**：
   ```bash
   # 检查后端服务是否在 5234 端口运行
   sudo netstat -tlnp | grep 5234
   # 或
   sudo ss -tlnp | grep 5234
   ```

## 验证 IP 转发是否生效

修改配置后，可以通过以下方式验证：

1. **查看后端日志**：
   ```bash
   # 查看后端应用日志，确认是否获取到真实 IP
   tail -f /path/to/backend/logs/app.log | grep "Analytics Track"
   ```

2. **测试访问**：
   - 使用手机 4G 网络访问 `https://xifg.com.cn/projects`
   - 在后台「访客分析」页面查看，应该能看到公网 IP

3. **检查数据库**：
   ```sql
   -- 查看最近访问记录，确认 IP 是否为公网 IP
   SELECT id, visitor_id, ip, path, timestamp 
   FROM visit_logs 
   ORDER BY timestamp DESC 
   LIMIT 10;
   ```

## 注意事项

1. **端口确认**：
   - 如果您的后端实际运行在其他端口，请根据实际情况修改
   - 可以通过 `ps aux | grep dotnet` 或 `netstat -tlnp | grep dotnet` 查看

2. **多层代理**：
   - 如果前面还有 CDN 或负载均衡器，需要确保它们也正确转发 IP
   - `X-Forwarded-For` 可能包含多个 IP，后端会自动选择第一个非私网 IP

3. **防火墙**：
   - 确保后端服务监听 `127.0.0.1:5234`（只允许本地访问）
   - 或监听 `0.0.0.0:5234`（允许外部访问，但需要配置防火墙）

## 相关文件

- 后端端口配置：`backend/PersonalSite.Api/Properties/launchSettings.json`
- IP 获取逻辑：`backend/PersonalSite.Api/Controllers/AnalyticsController.cs` - `GetClientIpAddress()` 方法
- ForwardedHeaders 配置：`backend/PersonalSite.Api/Program.cs`

