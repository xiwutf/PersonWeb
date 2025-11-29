# Nginx 反向代理 IP 转发配置

## 问题说明

当网站部署在 Nginx 反向代理后面时，后端应用获取到的客户端 IP 可能是内网 IP（如 10.x.x.x、172.x.x.x、192.168.x.x），而不是真实的公网 IP。这会导致：

1. 访客分析中 IP 地址显示为 `-` 或内网 IP
2. 地理位置显示为 `未知`
3. 无法正确统计访客来源

## 解决方案

### 1. Nginx 配置

在 Nginx 配置文件中，确保为后端 API 服务添加以下头部：

```nginx
server {
    listen 80;
    listen 443 ssl http2;
    server_name api.xifg.com.cn;  # 根据实际域名修改

    # SSL 配置（如果使用 HTTPS）
    # ssl_certificate /path/to/cert.pem;
    # ssl_certificate_key /path/to/key.pem;

    # 后端 API 反向代理
    location /api/ {
        proxy_pass http://127.0.0.1:5234;  # 后端服务地址（注意：后端实际运行在 5234 端口，不是 5000）
        
        # ============================================
        # 关键配置：转发客户端真实 IP（修复线上访问仍然显示未知的问题）
        # ============================================
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-Host $host;
        
        # 超时设置
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
        
        # 缓冲设置
        proxy_buffering on;
        proxy_buffer_size 4k;
        proxy_buffers 8 4k;
    }
}
```

### 2. 配置说明

- **X-Real-IP**: 直接传递客户端 IP（单个 IP）
- **X-Forwarded-For**: 传递完整的 IP 链（格式：`client, proxy1, proxy2`）
- **X-Forwarded-Proto**: 传递协议（http/https）
- **X-Forwarded-Host**: 传递原始主机名

### 3. 后端处理逻辑

后端代码已经实现了以下逻辑（修复线上访问仍然显示未知的问题）：

1. **优先级顺序**：
   - 优先从 `X-Forwarded-For` 获取第一个非私网 IP
   - 其次从 `X-Real-IP` 获取
   - 最后从 `RemoteIpAddress` 获取

2. **私网 IP 处理**：
   - 私网 IP（10.x.x.x、172.16-31.x.x、192.168.x.x）仍然会写入数据库
   - 但地理位置会显示为 `未知`（因为无法解析私网 IP 的地理位置）

3. **ForwardedHeaders 中间件**：
   - 已在 `Program.cs` 中配置 `UseForwardedHeaders()`
   - 自动处理 `X-Forwarded-For` 和 `X-Real-IP` 头部

### 4. 验证配置

配置完成后，可以通过以下方式验证：

1. **查看访问日志**：
   ```bash
   # 查看后端日志，确认是否获取到真实 IP
   tail -f /path/to/backend/logs/app.log | grep "Analytics Track"
   ```

2. **检查数据库**：
   ```sql
   -- 查看最近访问记录
   SELECT id, visitor_id, ip, path, timestamp 
   FROM visit_logs 
   ORDER BY timestamp DESC 
   LIMIT 20;
   ```

3. **测试访问**：
   - 使用手机 4G 网络访问 `https://xifg.com.cn/projects`
   - 在后台「访客分析」页面查看，应该能看到：
     - 公网 IP（非内网 IP）
     - 地理位置（国家/省份，或显示"未知"如果是私网 IP）
     - 当前路径 `/projects`
     - 非 0 的浏览量

### 5. 常见问题

**Q: 配置后仍然显示内网 IP？**

A: 检查以下几点：
1. Nginx 配置是否正确（确保 `proxy_set_header` 指令已添加）
2. Nginx 是否已重新加载配置：`nginx -s reload`
3. 后端服务是否已重启
4. 检查是否有多层代理（CDN、负载均衡器等），需要确保所有层都正确转发 IP

**Q: X-Forwarded-For 包含多个 IP，如何选择？**

A: 后端代码会自动选择第一个非私网 IP。如果都是私网 IP，会使用第一个 IP（但地理位置显示为"未知"）。

**Q: 如何信任特定的代理服务器？**

A: 在 `Program.cs` 中配置 `KnownProxies`：
```csharp
options.KnownProxies.Add(IPAddress.Parse("10.0.0.1")); // 代理服务器 IP
```

### 6. 安全建议

1. **生产环境**：建议配置 `KnownProxies`，只信任特定的代理服务器 IP
2. **防止 IP 伪造**：如果可能，在 Nginx 层面验证 `X-Forwarded-For` 的来源
3. **日志记录**：记录所有 IP 获取过程，便于排查问题

## 相关文件

- 后端 IP 获取逻辑：`backend/PersonalSite.Api/Controllers/AnalyticsController.cs` - `GetClientIpAddress()` 方法
- ForwardedHeaders 配置：`backend/PersonalSite.Api/Program.cs`
- 前端追踪插件：`plugins/analytics.client.ts`

