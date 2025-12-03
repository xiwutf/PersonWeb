# 启动 .NET 后端服务

## 快速启动

### 方法 1: 使用 Visual Studio
1. 打开 `PersonWeb.sln` 或 `PersonalSite.Api.csproj`
2. 按 `F5` 或点击"启动"按钮
3. 服务会在 `http://localhost:5234` 启动

### 方法 2: 使用命令行

```powershell
# 进入后端目录
cd backend/PersonalSite.Api

# 运行服务
dotnet run
```

### 方法 3: 使用 dotnet watch（开发模式，自动重载）

```powershell
cd backend/PersonalSite.Api
dotnet watch run
```

## 验证服务是否启动

打开浏览器访问：
- Swagger UI: http://localhost:5234/swagger
- 健康检查: http://localhost:5234/api/health

## 常见问题

### 问题：端口被占用

**解决方案：**
1. 修改 `Properties/launchSettings.json` 中的端口
2. 或使用 `dotnet run --urls "http://localhost:5235"` 指定其他端口

### 问题：数据库连接失败

**检查：**
1. `appsettings.json` 中的数据库连接字符串
2. MySQL 服务是否运行
3. 数据库是否存在

### 问题：编译错误

**解决：**
1. 清理项目：`dotnet clean`
2. 还原包：`dotnet restore`
3. 重新编译：`dotnet build`

## 开发环境完整启动流程

1. **启动 MySQL 数据库**（如果使用本地数据库）

2. **启动 Python Agent 服务**
   ```powershell
   cd ai-service
   uvicorn app.main:app --host 0.0.0.0 --port 8001 --reload
   ```

3. **启动 .NET 后端服务**
   ```powershell
   cd backend/PersonalSite.Api
   dotnet run
   ```

4. **启动前端服务**
   ```powershell
   npm run dev
   ```

## 服务地址

- 前端：http://localhost:3000
- .NET 后端：http://localhost:5234
- Python Agent：http://localhost:8001

