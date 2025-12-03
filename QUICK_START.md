# 快速启动指南

## 当前状态

- ✅ Python Agent 服务：运行中（http://localhost:8001）
- ❌ .NET 后端服务：未运行（需要启动）
- ✅ 前端服务：运行中（http://localhost:3000）

## 启动 .NET 后端服务

### 方法 1: 使用命令行（推荐）

打开新的 PowerShell 或终端窗口，运行：

```powershell
cd backend/PersonalSite.Api
dotnet run
```

### 方法 2: 使用 Visual Studio

1. 打开 `PersonWeb.sln` 或 `PersonalSite.Api.csproj`
2. 按 `F5` 启动
3. 或右键项目 → "调试" → "启动新实例"

### 方法 3: 使用 dotnet watch（自动重载）

```powershell
cd backend/PersonalSite.Api
dotnet watch run
```

## 验证服务启动

启动后，应该看到类似输出：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5234
```

然后可以访问：
- Swagger UI: http://localhost:5234/swagger
- 健康检查: http://localhost:5234/api/health

## 完整启动流程

1. **Python Agent**（已在运行）
   ```powershell
   cd ai-service
   uvicorn app.main:app --host 0.0.0.0 --port 8001 --reload
   ```

2. **.NET 后端**（需要启动）
   ```powershell
   cd backend/PersonalSite.Api
   dotnet run
   ```

3. **前端**（已在运行）
   ```powershell
   npm run dev
   ```

## 服务地址

- 前端：http://localhost:3000
- .NET 后端：http://localhost:5234
- Python Agent：http://localhost:8001

## 故障排查

### 问题：端口 5234 被占用

**解决：**
```powershell
# 查看占用端口的进程
netstat -ano | findstr :5234

# 或修改端口
dotnet run --urls "http://localhost:5235"
```

### 问题：编译错误

**解决：**
```powershell
dotnet clean
dotnet restore
dotnet build
```

### 问题：数据库连接失败

检查 `appsettings.json` 中的数据库连接字符串是否正确。

