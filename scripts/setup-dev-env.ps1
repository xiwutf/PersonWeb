# Windows PowerShell 开发环境快速配置脚本
# 使用方法：在项目根目录执行 .\scripts\setup-dev-env.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  开发环境快速配置脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 Node.js
Write-Host "检查 Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version
    Write-Host "✓ Node.js 已安装: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Node.js 未安装，请先安装 Node.js 18+" -ForegroundColor Red
    exit 1
}

# 检查 .NET SDK
Write-Host "检查 .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK 已安装: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET SDK 未安装，请先安装 .NET 8.0+" -ForegroundColor Red
    exit 1
}

# 检查 MySQL（可选）
Write-Host "检查 MySQL..." -ForegroundColor Yellow
try {
    $mysqlVersion = mysql --version
    Write-Host "✓ MySQL 已安装" -ForegroundColor Green
} catch {
    Write-Host "⚠ MySQL 未检测到，请确保数据库服务可用" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "开始配置环境..." -ForegroundColor Cyan

# 创建前端 .env 文件
if (-not (Test-Path ".env")) {
    Write-Host "创建 .env 文件..." -ForegroundColor Yellow
    Copy-Item ".env.example" ".env" -ErrorAction SilentlyContinue
    if (Test-Path ".env") {
        Write-Host "✓ .env 文件已创建，请编辑配置" -ForegroundColor Green
    }
} else {
    Write-Host "✓ .env 文件已存在" -ForegroundColor Green
}

# 创建后端配置文件
$backendConfigPath = "backend\PersonalSite.Api\appsettings.Development.json"
if (-not (Test-Path $backendConfigPath)) {
    Write-Host "创建后端配置文件..." -ForegroundColor Yellow
    $examplePath = "backend\PersonalSite.Api\appsettings.Development.json.example"
    if (Test-Path $examplePath) {
        Copy-Item $examplePath $backendConfigPath
        Write-Host "✓ 后端配置文件已创建，请编辑数据库连接字符串" -ForegroundColor Green
    }
} else {
    Write-Host "✓ 后端配置文件已存在" -ForegroundColor Green
}

# 安装前端依赖
Write-Host ""
Write-Host "安装前端依赖..." -ForegroundColor Yellow
if (Test-Path "node_modules") {
    Write-Host "检测到 node_modules，跳过安装（如需重新安装，请先删除 node_modules）" -ForegroundColor Yellow
} else {
    npm install
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ 前端依赖安装完成" -ForegroundColor Green
    } else {
        Write-Host "✗ 前端依赖安装失败" -ForegroundColor Red
        exit 1
    }
}

# 恢复后端依赖
Write-Host ""
Write-Host "恢复后端依赖..." -ForegroundColor Yellow
Set-Location "backend\PersonalSite.Api"
dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ 后端依赖恢复完成" -ForegroundColor Green
} else {
    Write-Host "✗ 后端依赖恢复失败" -ForegroundColor Red
    Set-Location ..\..
    exit 1
}
Set-Location ..\..

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  配置完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "下一步：" -ForegroundColor Yellow
Write-Host "1. 编辑 .env 文件配置 API 地址" -ForegroundColor White
Write-Host "2. 编辑 backend\PersonalSite.Api\appsettings.Development.json 配置数据库" -ForegroundColor White
Write-Host "3. 执行数据库脚本创建表结构" -ForegroundColor White
Write-Host "4. 启动后端: cd backend\PersonalSite.Api && dotnet run" -ForegroundColor White
Write-Host "5. 启动前端: npm run dev" -ForegroundColor White
Write-Host ""

