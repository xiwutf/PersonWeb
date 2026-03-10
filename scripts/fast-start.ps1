# 快速启动脚本 - Windows 版本

Write-Host "🚀 启动溪午听风个人网站（优化模式）" -ForegroundColor Green
Write-Host ""

# 1. 检查 node_modules
if (-not (Test-Path "node_modules")) {
    Write-Host "📦 首次启动，正在安装依赖..." -ForegroundColor Yellow
    npm install
}

# 2. 检查 node_modules 大小
if (Test-Path "node_modules") {
    $size = (Get-ChildItem "node_modules" -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
    Write-Host "📊 node_modules 大小: [$( [math]::Round($size, 2) ) MB]" -ForegroundColor Cyan

    if ($size -gt 500) {
        Write-Host "⚠️  node_modules 较大，建议运行清理：" -ForegroundColor Yellow
        Write-Host "   npm run clean:modules" -ForegroundColor White
        Write-Host ""
    }
}

# 3. 检查环境变量文件
if (-not (Test-Path ".env") -and (Test-Path ".env.example")) {
    Write-Host "📝 创建 .env 文件..." -ForegroundColor Yellow
    Copy-Item ".env.example" ".env"
    Write-Host "✅ .env 文件已创建" -ForegroundColor Green
    Write-Host ""
}

# 4. 启动开发服务器
Write-Host "🏃 启动开发服务器..." -ForegroundColor Green
Write-Host "   地址: http://localhost:3000" -ForegroundColor White
Write-Host "   按 Ctrl+C 停止" -ForegroundColor White
Write-Host ""

npm run dev