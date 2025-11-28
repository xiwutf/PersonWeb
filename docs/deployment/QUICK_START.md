# 快速开始指南

5 分钟快速搭建开发环境。

## 🚀 一键配置（推荐）

### Windows

```powershell
.\scripts\setup-dev-env.ps1
```

### Linux/macOS

```bash
chmod +x scripts/setup-dev-env.sh
./scripts/setup-dev-env.sh
```

## 📝 手动配置

### 1. 安装依赖

```bash
# 前端
npm install

# 后端
cd backend/PersonalSite.Api
dotnet restore
cd ../..
```

### 2. 配置环境变量

```bash
# 复制环境变量模板
cp .env.example .env

# 编辑 .env 文件，设置 API 地址
# NUXT_PUBLIC_API_BASE=http://localhost:5234/api
```

### 3. 配置数据库

编辑 `backend/PersonalSite.Api/appsettings.Development.json`：

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=personal_site;User=root;Password=your_password;SslMode=None;"
  }
}
```

### 4. 创建数据库

```sql
CREATE DATABASE personal_site CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### 5. 执行数据库脚本

```bash
mysql -u root -p personal_site < database/all_tables.sql
```

### 6. 启动服务

**终端 1 - 后端：**
```bash
cd backend/PersonalSite.Api
dotnet run
```

**终端 2 - 前端：**
```bash
npm run dev
```

### 7. 访问应用

- 前端：http://localhost:3000
- API 文档：http://localhost:5234/swagger

## ✅ 验证安装

1. 前端页面正常显示
2. API 文档可以访问
3. 数据库连接成功

## 📱 移动端访问

如果想在手机上测试：

1. **启动移动端开发服务器**
   ```bash
   # Windows
   .\scripts\dev-mobile.ps1
   
   # Linux/macOS
   ./scripts/dev-mobile.sh
   
   # 或直接使用
   npm run dev:mobile
   ```

2. **在手机上访问**
   - 确保手机和电脑连接到同一个 WiFi
   - 在手机浏览器输入：`http://你的电脑IP:3000`
   - 例如：`http://192.168.1.100:3000`

详细说明请查看 [移动端访问指南](./MOBILE_ACCESS.md)

## 📚 详细文档

查看 [开发环境配置指南](./DEVELOPMENT_SETUP.md) 获取完整说明。

