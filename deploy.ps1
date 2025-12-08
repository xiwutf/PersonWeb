# 生产环境部署脚本 (PowerShell)
# 用于将 Nuxt 3 应用部署到生产环境

$ErrorActionPreference = "Stop"

Write-Host "🚀 开始生产环境部署..." -ForegroundColor Cyan

# 检查 Node.js 版本
Write-Host "`n检查 Node.js 版本..." -ForegroundColor Yellow
$nodeVersion = node -v
Write-Host "当前 Node.js 版本: $nodeVersion" -ForegroundColor Green

# 检查是否存在 .env.production
if (-not (Test-Path ".env.production")) {
    Write-Host "⚠️  警告: .env.production 文件不存在，将使用默认配置" -ForegroundColor Yellow
}

# 安装依赖
Write-Host "`n安装依赖..." -ForegroundColor Yellow
npm ci --production=false
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ 依赖安装失败！" -ForegroundColor Red
    exit 1
}

# 设置生产环境变量
$env:NODE_ENV = "production"
$env:NUXT_PUBLIC_API_BASE = "https://api.xifg.com.cn/api"

Write-Host "`n环境变量配置:" -ForegroundColor Yellow
Write-Host "  NODE_ENV: $env:NODE_ENV" -ForegroundColor Green
Write-Host "  NUXT_PUBLIC_API_BASE: $env:NUXT_PUBLIC_API_BASE" -ForegroundColor Green

# 构建应用
Write-Host "`n使用生产环境配置构建..." -ForegroundColor Yellow
npm run build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ 构建失败！" -ForegroundColor Red
    exit 1
}

# 检查构建结果
if (Test-Path ".output") {
    Write-Host "`n✅ 构建成功！" -ForegroundColor Green
    Write-Host "构建输出目录: .output" -ForegroundColor Green
    
    # 显示构建信息
    Write-Host "`n构建信息:" -ForegroundColor Yellow
    Write-Host "  - API 基础路径: https://api.xifg.com.cn/api" -ForegroundColor White
    Write-Host "  - 环境: production" -ForegroundColor White
    Write-Host "  - 输出目录: .output" -ForegroundColor White
    
    # 显示文件大小
    $outputSize = (Get-ChildItem -Path ".output" -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
    Write-Host "  - 构建产物大小: $([math]::Round($outputSize, 2)) MB" -ForegroundColor White
} else {
    Write-Host "`n❌ 构建失败！未找到 .output 目录" -ForegroundColor Red
    exit 1
}

Write-Host "`n🎉 部署准备完成！" -ForegroundColor Green
Write-Host "`n下一步操作：" -ForegroundColor Yellow
Write-Host "  1. 将 .output 目录部署到服务器" -ForegroundColor White
Write-Host "  2. 配置 Nginx 或其他 Web 服务器" -ForegroundColor White
Write-Host "  3. 确保 API 服务 https://api.xifg.com.cn/api 可访问" -ForegroundColor White

