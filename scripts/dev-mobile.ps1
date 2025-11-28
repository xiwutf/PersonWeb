# 移动端开发服务器启动脚本
# 使用方法：.\scripts\dev-mobile.ps1

# 获取本机 IP 地址
$ip = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {
    $_.InterfaceAlias -notlike "*Loopback*" -and 
    $_.IPAddress -notlike "169.254.*" -and
    $_.IPAddress -notlike "127.*"
}).IPAddress | Select-Object -First 1

if (-not $ip) {
    Write-Host "无法获取 IP 地址，请手动检查网络连接" -ForegroundColor Red
    exit 1
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  移动端开发服务器" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "本机 IP 地址: " -NoNewline
Write-Host $ip -ForegroundColor Green
Write-Host ""
Write-Host "在手机上访问: " -NoNewline
Write-Host "http://$ip:3000" -ForegroundColor Yellow
Write-Host ""
Write-Host "确保：" -ForegroundColor Yellow
Write-Host "  1. 手机和电脑连接到同一个 WiFi" -ForegroundColor Gray
Write-Host "  2. 防火墙允许 Node.js 通过" -ForegroundColor Gray
Write-Host "  3. 如果 API 请求失败，请修改 .env 文件中的 API 地址" -ForegroundColor Gray
Write-Host ""
Write-Host "按 Ctrl+C 停止服务器" -ForegroundColor Gray
Write-Host ""

# 设置环境变量并启动
$env:HOST = "0.0.0.0"
npm run dev

