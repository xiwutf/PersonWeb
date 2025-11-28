# 开发环境配置指南

本文档帮助你在新电脑上快速搭建开发环境。

## 📋 前置要求

### 必需软件

1. **Node.js** (版本 18+)
   - 下载地址：https://nodejs.org/
   - 验证安装：`node --version` 和 `npm --version`

2. **.NET SDK** (版本 8.0+)
   - 下载地址：https://dotnet.microsoft.com/download
   - 验证安装：`dotnet --version`

3. **MySQL** (版本 5.7+ 或 8.0+)
   - 下载地址：https://dev.mysql.com/downloads/mysql/
   - 或者使用 Docker：`docker run -d -p 3306:3306 -e MYSQL_ROOT_PASSWORD=your_password mysql:8.0`

4. **Git**
   - 下载地址：https://git-scm.com/downloads
   - 验证安装：`git --version`

### 可选软件

- **Visual Studio Code** 或 **Visual Studio**（用于编辑代码）
- **MySQL Workbench** 或 **Navicat**（用于数据库管理）
- **Postman** 或 **Insomnia**（用于 API 测试）

---

## 🚀 快速开始

### 1. 克隆项目

```bash
git clone <your-repo-url>
cd PersonWeb
```

### 2. 配置环境变量

#### 前端环境变量

创建 `.env` 文件（开发环境）：

```bash
# 复制模板文件
cp .env.example .env
```

编辑 `.env` 文件：

```env
# API 基础路径（开发环境）
NUXT_PUBLIC_API_BASE=http://localhost:5234/api
```

创建 `.env.production` 文件（生产环境）：

```env
# API 基础路径（生产环境）
NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api
```

#### 后端环境变量

编辑 `backend/PersonalSite.Api/appsettings.Development.json`：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=personal_site;User=root;Password=your_password;SslMode=None;"
  }
}
```

**重要**：不要将包含敏感信息的配置文件提交到 Git！

### 3. 安装前端依赖

```bash
# 安装 npm 依赖
npm install
```

### 4. 安装后端依赖

```bash
# 进入后端目录
cd backend/PersonalSite.Api

# 恢复 NuGet 包
dotnet restore

# 返回项目根目录
cd ../..
```

### 5. 配置数据库

#### 创建数据库

```sql
CREATE DATABASE IF NOT EXISTS personal_site 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;
```

#### 执行数据库脚本

按顺序执行 `database/` 目录下的 SQL 脚本：

```bash
# 1. 基础表结构
mysql -u root -p personal_site < database/all_tables.sql

# 2. 项目表
mysql -u root -p personal_site < database/projects_table.sql

# 3. 文章版本历史
mysql -u root -p personal_site < database/article_version_history.sql

# 4. 错误日志表
mysql -u root -p personal_site < database/error_log_table.sql

# 5. 友情链接表
mysql -u root -p personal_site < database/friend_links_table.sql

# 6. 知识库表
mysql -u root -p personal_site < database/knowledge_base_tables.sql

# 7. 技能树表
mysql -u root -p personal_site < database/skill_tree_tables.sql

# 8. 时间胶囊表
mysql -u root -p personal_site < database/time_capsule_table.sql

# 9. 投资表（可选）
mysql -u root -p personal_site < database/investment_tables.sql

# 10. 全文索引（可选，提升搜索性能）
mysql -u root -p personal_site < database/fulltext_index.sql
```

或者使用数据库管理工具（如 MySQL Workbench）逐个执行 SQL 文件。

### 6. 启动开发服务器

#### 启动后端（终端 1）

```bash
cd backend/PersonalSite.Api
dotnet run
```

后端服务将在 `http://localhost:5234` 启动。

验证后端：
- API 文档：http://localhost:5234/swagger
- 健康检查：http://localhost:5234/api/health

#### 启动前端（终端 2）

```bash
# 在项目根目录
npm run dev
```

前端服务将在 `http://localhost:3000` 启动。

---

## 📁 项目结构

```
PersonWeb/
├── backend/                 # 后端代码（ASP.NET Core）
│   └── PersonalSite.Api/   # API 项目
├── components/              # Vue 组件
├── pages/                   # 页面路由
├── server/                  # Nuxt 服务端 API
├── database/                # 数据库 SQL 脚本
├── docs/                    # 项目文档
├── public/                  # 静态资源
├── types/                   # TypeScript 类型定义
├── nuxt.config.ts           # Nuxt 配置
├── package.json             # 前端依赖
└── README.md                # 项目说明
```

---

## 🔧 配置说明

### 前端配置

#### `nuxt.config.ts`

主要配置项：
- **API 基础路径**：通过环境变量 `NUXT_PUBLIC_API_BASE` 配置
- **Content 模块**：Markdown 内容管理
- **Tailwind CSS**：样式框架

#### 环境变量

- `.env`：开发环境配置
- `.env.production`：生产环境配置

### 后端配置

#### `appsettings.json`

生产环境配置（不要提交敏感信息到 Git）

#### `appsettings.Development.json`

开发环境配置，包含：
- 数据库连接字符串
- 日志级别

#### 重要配置项

1. **数据库连接字符串**
   ```json
   "ConnectionStrings": {
     "Default": "Server=localhost;Database=personal_site;User=root;Password=your_password;SslMode=None;"
   }
   ```

2. **JWT 配置**
   ```json
   "Jwt": {
     "Key": "your-secret-key-here",
     "Issuer": "PersonalSite.Api",
     "Audience": "PersonalSite.Web"
   }
   ```

3. **OSS 配置**（如果使用阿里云 OSS）
   ```json
   "AliyunOSS": {
     "Endpoint": "your-endpoint",
     "AccessKeyId": "your-access-key-id",
     "AccessKeySecret": "your-access-key-secret",
     "BucketName": "your-bucket-name"
   }
   ```

---

## 🗄️ 数据库配置

### 连接字符串格式

```
Server=服务器地址;Database=数据库名;User=用户名;Password=密码;SslMode=None;
```

### 本地开发示例

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=personal_site;User=root;Password=123456;SslMode=None;"
  }
}
```

### 远程数据库示例

```json
{
  "ConnectionStrings": {
    "Default": "Server=rm-xxxxx.mysql.rds.aliyuncs.com;Database=personal_site;User=your_user;Password=your_password;SslMode=None;"
  }
}
```

---

## 🛠️ 常用命令

### 前端

```bash
# 开发模式
npm run dev

# 生产构建
npm run build

# 预览生产版本
npm run preview

# 安装依赖
npm install

# 清理缓存
rm -rf .nuxt node_modules
npm install
```

### 后端

```bash
# 进入后端目录
cd backend/PersonalSite.Api

# 运行项目
dotnet run

# 恢复依赖
dotnet restore

# 构建项目
dotnet build

# 清理构建
dotnet clean
```

### 数据库

```bash
# 连接数据库
mysql -u root -p

# 导入 SQL 文件
mysql -u root -p personal_site < database/all_tables.sql

# 导出数据库
mysqldump -u root -p personal_site > backup.sql
```

---

## 🐛 常见问题

### 1. 端口被占用

**问题**：`EADDRINUSE: address already in use :::3000`

**解决**：
```bash
# Windows
netstat -ano | findstr :3000
taskkill /PID <PID> /F

# Linux/macOS
lsof -ti:3000 | xargs kill -9
```

### 2. 数据库连接失败

**检查项**：
- 数据库服务是否启动
- 连接字符串是否正确
- 用户名密码是否正确
- 防火墙是否允许连接

### 3. 依赖安装失败

**解决**：
```bash
# 清理缓存
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

### 4. .NET SDK 版本不匹配

**检查**：
```bash
dotnet --version
```

**要求**：.NET 8.0 或更高版本

### 5. 前端 API 请求失败

**检查**：
- 后端服务是否启动
- API 地址是否正确（检查 `.env` 文件）
- 跨域配置是否正确

---

## 📝 开发工作流

### 日常开发流程

1. **启动后端**
   ```bash
   cd backend/PersonalSite.Api
   dotnet run
   ```

2. **启动前端**
   ```bash
   npm run dev
   ```

3. **访问应用**
   - 前端：http://localhost:3000
   - 后端 API 文档：http://localhost:5234/swagger

### 代码提交前检查

- [ ] 代码格式化（Prettier/ESLint）
- [ ] 运行测试（如果有）
- [ ] 检查控制台错误
- [ ] 确认环境变量已正确配置
- [ ] 确认敏感信息未提交到 Git

---

## 🔐 安全注意事项

1. **不要提交敏感信息**
   - `.env` 文件已在 `.gitignore` 中
   - `appsettings.Development.json` 已在 `.gitignore` 中
   - 数据库密码、API 密钥等不要提交

2. **使用环境变量**
   - 开发环境使用 `.env`
   - 生产环境使用服务器环境变量

3. **定期更新依赖**
   ```bash
   npm audit
   npm audit fix
   ```

---

## 📚 相关文档

- [后端启动指南](./START_BACKEND.md)
- [API 配置说明](../config/API_CONFIG.md)
- [环境兼容性说明](../config/ENV_COMPATIBILITY.md)
- [功能开发状态](../features/IMPLEMENTATION_STATUS.md)

---

## 🆘 获取帮助

如果遇到问题：

1. 查看本文档的"常见问题"部分
2. 查看 `docs/troubleshooting/` 目录下的故障排除文档
3. 检查项目 Issues
4. 联系项目维护者

---

**最后更新**：2025-01-XX

