# 移动端访问开发环境指南

## 📱 在手机上访问本地开发服务器

### 方法一：使用局域网 IP 地址（推荐）

#### 1. 查找电脑的局域网 IP 地址

**Windows:**
```powershell
# 方法1：使用 PowerShell
ipconfig | findstr IPv4

# 方法2：使用命令提示符
ipconfig

# 查找 "IPv4 地址"，通常是 192.168.x.x 或 10.x.x.x
```

**macOS/Linux:**
```bash
# 方法1
ifconfig | grep "inet "

# 方法2
ip addr show

# 方法3
hostname -I
```

**常见 IP 地址格式：**
- `192.168.1.100`
- `192.168.0.100`
- `10.0.0.100`

#### 2. 确保手机和电脑在同一网络

- ✅ 手机和电脑连接到**同一个 WiFi**
- ❌ 不要使用手机热点（手机作为热点时，电脑无法通过手机 IP 访问）

#### 3. 启动开发服务器并允许外部访问

**前端（Nuxt）：**

默认情况下，Nuxt 开发服务器只监听 `localhost`，需要修改为监听所有网络接口：

**方法 A：修改 package.json（推荐）**

```json
{
  "scripts": {
    "dev": "nuxt dev --host 0.0.0.0",
    "dev:mobile": "nuxt dev --host 0.0.0.0 --port 3000"
  }
}
```

然后运行：
```bash
npm run dev
# 或
npm run dev:mobile
```

**方法 B：使用环境变量**

```bash
# Windows PowerShell
$env:HOST="0.0.0.0"; npm run dev

# Windows CMD
set HOST=0.0.0.0 && npm run dev

# macOS/Linux
HOST=0.0.0.0 npm run dev
```

**后端（ASP.NET Core）：**

编辑 `backend/PersonalSite.Api/Properties/launchSettings.json` 或使用命令行：

```bash
cd backend/PersonalSite.Api
dotnet run --urls "http://0.0.0.0:5234"
```

或者修改 `Program.cs`：
```csharp
app.Urls.Add("http://0.0.0.0:5234");
```

#### 4. 在手机上访问

打开手机浏览器，访问：

```
http://你的电脑IP:3000
```

例如：
- `http://192.168.1.100:3000`
- `http://192.168.0.100:3000`

**后端 API 地址：**

如果前端需要访问后端 API，需要修改 `.env` 文件：

```env
NUXT_PUBLIC_API_BASE=http://你的电脑IP:5234/api
```

例如：
```env
NUXT_PUBLIC_API_BASE=http://192.168.1.100:5234/api
```

### 方法二：使用 ngrok（适合外网访问）

如果需要从外网访问（比如测试分享功能），可以使用 ngrok：

#### 1. 安装 ngrok

```bash
# 下载并安装 ngrok
# https://ngrok.com/download

# 或使用 npm
npm install -g ngrok
```

#### 2. 启动开发服务器

```bash
npm run dev
```

#### 3. 在另一个终端启动 ngrok

```bash
ngrok http 3000
```

#### 4. 使用 ngrok 提供的 URL

ngrok 会提供一个类似这样的 URL：
```
https://abc123.ngrok.io
```

在手机上访问这个 URL 即可。

**注意：**
- 免费版 ngrok URL 每次启动都会变化
- 适合临时测试，不适合长期使用

### 方法三：使用手机浏览器开发者工具

#### Chrome DevTools（推荐）

1. 在电脑上打开 Chrome
2. 访问 `chrome://inspect`
3. 连接手机（USB 或 WiFi）
4. 在手机上打开 Chrome
5. 在电脑上可以看到手机页面，可以调试

#### Safari Web Inspector（iOS）

1. iPhone 设置 → Safari → 高级 → Web 检查器（开启）
2. 用 USB 连接 iPhone 到 Mac
3. Mac 上打开 Safari → 开发 → [你的 iPhone] → 选择页面

---

## 🔧 常见问题解决

### 问题 1：手机无法访问，显示"无法连接"

**解决方案：**

1. **检查防火墙**
   - Windows：允许 Node.js 和 .NET 通过防火墙
   - macOS：系统偏好设置 → 安全性与隐私 → 防火墙

2. **检查 IP 地址是否正确**
   ```bash
   # 重新获取 IP 地址
   ipconfig  # Windows
   ifconfig  # macOS/Linux
   ```

3. **确保端口没有被占用**
   ```bash
   # Windows
   netstat -ano | findstr :3000
   
   # macOS/Linux
   lsof -i :3000
   ```

4. **检查网络连接**
   - 确保手机和电脑在同一 WiFi
   - 尝试 ping 电脑 IP（在手机上使用网络工具）

### 问题 2：页面加载但 API 请求失败

**解决方案：**

1. **修改前端 API 地址**
   
   编辑 `.env` 文件：
   ```env
   NUXT_PUBLIC_API_BASE=http://你的电脑IP:5234/api
   ```

2. **确保后端也允许外部访问**
   ```bash
   dotnet run --urls "http://0.0.0.0:5234"
   ```

3. **检查后端 CORS 配置**
   
   确保 `Program.cs` 中允许来自手机 IP 的请求：
   ```csharp
   builder.Services.AddCors(options =>
   {
       options.AddPolicy("AllowAll", policy =>
       {
           policy.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader();
       });
   });
   ```

### 问题 3：页面显示但样式错乱

**可能原因：**
- 移动端适配问题
- 需要清除浏览器缓存

**解决方案：**
- 在手机上强制刷新（长按刷新按钮）
- 清除浏览器缓存
- 检查移动端优化是否生效

### 问题 4：3D 效果在手机上不显示

**这是正常的！**

移动端为了性能优化，自动禁用了 3D 场景。这是设计如此，不是 bug。

如果想在移动端测试 3D 效果，可以临时修改 `components/Immersive3DScene.vue`：
```typescript
const shouldRender3D = computed(() => true) // 临时启用
```

---

## 📝 快速检查清单

在手机上访问前，确保：

- [ ] 手机和电脑连接到同一个 WiFi
- [ ] 已获取电脑的局域网 IP 地址
- [ ] 开发服务器使用 `--host 0.0.0.0` 启动
- [ ] 防火墙允许 Node.js 和 .NET 通过
- [ ] `.env` 文件中的 API 地址已更新为电脑 IP
- [ ] 后端服务也允许外部访问

---

## 🚀 快速启动脚本

### Windows（PowerShell）

创建 `scripts/dev-mobile.ps1`：

```powershell
# 获取本机 IP
$ip = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.InterfaceAlias -notlike "*Loopback*" -and $_.IPAddress -notlike "169.254.*"}).IPAddress | Select-Object -First 1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  移动端开发服务器" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "本机 IP 地址: $ip" -ForegroundColor Green
Write-Host ""
Write-Host "在手机上访问: http://$ip:3000" -ForegroundColor Yellow
Write-Host ""
Write-Host "按 Ctrl+C 停止服务器" -ForegroundColor Gray
Write-Host ""

# 设置环境变量并启动
$env:HOST = "0.0.0.0"
npm run dev
```

运行：
```powershell
.\scripts\dev-mobile.ps1
```

### macOS/Linux

创建 `scripts/dev-mobile.sh`：

```bash
#!/bin/bash

# 获取本机 IP
IP=$(ifconfig | grep "inet " | grep -v 127.0.0.1 | awk '{print $2}' | head -n 1)

echo "========================================"
echo "  移动端开发服务器"
echo "========================================"
echo ""
echo "本机 IP 地址: $IP"
echo ""
echo "在手机上访问: http://$IP:3000"
echo ""
echo "按 Ctrl+C 停止服务器"
echo ""

# 启动服务器
HOST=0.0.0.0 npm run dev
```

运行：
```bash
chmod +x scripts/dev-mobile.sh
./scripts/dev-mobile.sh
```

---

## 📱 推荐的测试流程

1. **启动开发服务器（使用移动端模式）**
   ```bash
   npm run dev:mobile
   # 或
   HOST=0.0.0.0 npm run dev
   ```

2. **在手机上打开浏览器**
   - 输入电脑 IP 地址 + 端口
   - 例如：`http://192.168.1.100:3000`

3. **测试功能**
   - 检查页面布局
   - 测试触摸交互
   - 测试导航菜单
   - 测试表单输入

4. **使用开发者工具调试**
   - Chrome DevTools 远程调试
   - Safari Web Inspector（iOS）

---

## 🔐 安全提示

⚠️ **重要：** 使用 `0.0.0.0` 监听所有网络接口时，局域网内的其他设备都可以访问你的开发服务器。

**建议：**
- 仅在开发时使用
- 不要在生产环境使用
- 使用完记得关闭服务器
- 如果使用公共 WiFi，注意安全

---

**最后更新**：2025-01-XX

