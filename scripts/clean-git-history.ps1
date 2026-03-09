# ============================================
# Git 历史敏感信息清理脚本
# 使用 git-filter-repo 重写 Git 历史，替换所有敏感信息
# ============================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Git 历史敏感信息清理工具" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查是否安装了 git-filter-repo
$filterRepo = pip show git-filter-repo 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "[!] 正在安装 git-filter-repo..." -ForegroundColor Yellow
    pip install git-filter-repo
}

# 检查工作区是否干净
$status = git status --porcelain
if ($status) {
    Write-Host "[!] 检测到未提交的更改，请先提交后再运行此脚本" -ForegroundColor Red
    Write-Host "    运行: git add -A && git commit -m 'security: remove sensitive data from codebase'" -ForegroundColor Yellow
    exit 1
}

Write-Host "[1/4] 创建仓库备份..." -ForegroundColor Green
$backupPath = "../PersonWeb-backup-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
Copy-Item -Recurse -Path "." -Destination $backupPath -Force
Write-Host "       备份已保存到: $backupPath" -ForegroundColor Gray

Write-Host "[2/4] 准备敏感信息替换规则..." -ForegroundColor Green
$replacementsFile = "scripts/sensitive-replacements.txt"
if (-not (Test-Path $replacementsFile)) {
    Write-Host "[!] 找不到替换规则文件: $replacementsFile" -ForegroundColor Red
    exit 1
}

Write-Host "[3/4] 正在重写 Git 历史（这可能需要几分钟）..." -ForegroundColor Green
git filter-repo --replace-text $replacementsFile --force

Write-Host "[4/4] 重新添加远程仓库..." -ForegroundColor Green
git remote add origin https://github.com/Lijing327/PersonWeb.git 2>$null

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  清理完成!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "接下来你需要：" -ForegroundColor Yellow
Write-Host "  1. 验证清理结果（搜索不应返回任何结果）:" -ForegroundColor White
Write-Host "     git log --all -p | Select-String 'YOUR_OLD_API_KEY_PREFIX'" -ForegroundColor Gray
Write-Host "     git log --all -p | Select-String 'YOUR_OLD_DB_PASSWORD'" -ForegroundColor Gray
Write-Host ""
Write-Host "  2. 强制推送到 GitHub:" -ForegroundColor White
Write-Host "     git push origin --force --all" -ForegroundColor Gray
Write-Host ""
Write-Host "  3. 立即轮换所有泄露的凭据:" -ForegroundColor Red
Write-Host "     - 阿里云 RDS 数据库密码" -ForegroundColor Gray
Write-Host "     - DeepSeek API Key" -ForegroundColor Gray
Write-Host "     - JWT 签名密钥" -ForegroundColor Gray
Write-Host ""
Write-Host "  4. 在 GitHub 仓库设置中配置 Secrets:" -ForegroundColor White
Write-Host "     Settings -> Secrets and variables -> Actions" -ForegroundColor Gray
