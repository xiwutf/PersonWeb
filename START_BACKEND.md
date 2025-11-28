# 启动后端服务指南

## 问题诊断

如果看到 `ERR_CONNECTION_REFUSED` 错误，说明后端服务没有运行。

## 启动步骤

### 方法一：使用 Visual Studio / Rider

1. 打开 `backend/PersonalSite.Api/PersonalSite.Api.csproj`
2. 选择运行配置为 `http` 或 `https`
3. 按 F5 启动

### 方法二：使用命令行（推荐）

在项目根目录执行：

```bash
# 进入后端目录
cd backend/PersonalSite.Api

# 启动服务
dotnet run

# 或者指定端口
dotnet run --urls "http://localhost:5234"
```

### 方法三：使用已编译的可执行文件

```bash
cd backend/PersonalSite.Api/bin/Debug/net8.0
./PersonalSite.Api.exe
```

## 验证服务是否启动

启动后，你应该看到类似输出：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5234
```

然后访问：
- API 健康检查：http://localhost:5234/api/health
- Swagger 文档：http://localhost:5234/swagger

## 常见问题

### 1. 端口被占用

如果 5234 端口被占用，可以修改 `launchSettings.json` 或使用：
```bash
dotnet run --urls "http://localhost:5235"
```

同时需要修改前端的 API 配置。

### 2. 数据库连接失败

检查 `appsettings.json` 中的数据库连接字符串是否正确。

### 3. 依赖包缺失

运行：
```bash
cd backend/PersonalSite.Api
dotnet restore
```

## 快速启动脚本

### Windows (PowerShell)
```powershell
# start-backend.ps1
cd backend/PersonalSite.Api
dotnet run
```

### Linux/macOS
```bash
#!/bin/bash
# start-backend.sh
cd backend/PersonalSite.Api
dotnet run
```

