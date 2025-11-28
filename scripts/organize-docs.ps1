# Markdown 文档整理脚本
# 将项目根目录的 Markdown 文档移动到 docs/ 分类目录

Write-Host "开始整理 Markdown 文档..." -ForegroundColor Green

# 创建目录（如果不存在）
$directories = @(
    "docs\features",
    "docs\improvements",
    "docs\config",
    "docs\architecture",
    "docs\deployment",
    "docs\troubleshooting",
    "docs\quality",
    "docs\migration",
    "docs\misc"
)

foreach ($dir in $directories) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
        Write-Host "创建目录: $dir" -ForegroundColor Yellow
    }
}

# 功能开发文档
$featureFiles = @(
    "FEATURE_STATUS.md",
    "FEATURE_SUMMARY.md",
    "FEATURE_DEVELOPMENT_LOG.md",
    "OPTIMIZATION_PLAN.md",
    "CREATIVE_FEATURES.md"
)

foreach ($file in $featureFiles) {
    if (Test-Path $file) {
        Move-Item -Path $file -Destination "docs\features\" -Force
        Write-Host "移动: $file -> docs\features\" -ForegroundColor Cyan
    }
}

# 改进文档
$improvementFiles = @(
    "IMPROVEMENT_PLAN.md",
    "IMPROVEMENT_SUMMARY.md",
    "IMPROVEMENT_PROGRESS.md",
    "IMPROVEMENT_FINAL.md",
    "MARKDOWN_EDITOR_UPGRADE.md"
)

foreach ($file in $improvementFiles) {
    if (Test-Path $file) {
        Move-Item -Path $file -Destination "docs\improvements\" -Force
        Write-Host "移动: $file -> docs\improvements\" -ForegroundColor Cyan
    }
}

# 配置文档
$configFiles = @(
    "CONFIG_SUMMARY.md",
    "API_CONFIG.md",
    "ENV_COMPATIBILITY.md"
)

foreach ($file in $configFiles) {
    if (Test-Path $file) {
        Move-Item -Path $file -Destination "docs\config\" -Force
        Write-Host "移动: $file -> docs\config\" -ForegroundColor Cyan
    }
}

# 架构文档
if (Test-Path "ARCHITECTURE.md") {
    Move-Item -Path "ARCHITECTURE.md" -Destination "docs\architecture\" -Force
    Write-Host "移动: ARCHITECTURE.md -> docs\architecture\" -ForegroundColor Cyan
}

# 部署文档
$deploymentFiles = @(
    "DEPLOY.md",
    "START_BACKEND.md"
)

foreach ($file in $deploymentFiles) {
    if (Test-Path $file) {
        Move-Item -Path $file -Destination "docs\deployment\" -Force
        Write-Host "移动: $file -> docs\deployment\" -ForegroundColor Cyan
    }
}

# 故障排除文档
$troubleshootingFiles = @(
    "TROUBLESHOOTING.md",
    "ARTICLES_TROUBLESHOOTING.md",
    "TOOLS_API_FIX.md",
    "GIT_FIX.md"
)

foreach ($file in $troubleshootingFiles) {
    if (Test-Path $file) {
        Move-Item -Path $file -Destination "docs\troubleshooting\" -Force
        Write-Host "移动: $file -> docs\troubleshooting\" -ForegroundColor Cyan
    }
}

# 代码质量文档
if (Test-Path "CODE_QUALITY_REPORT.md") {
    Move-Item -Path "CODE_QUALITY_REPORT.md" -Destination "docs\quality\" -Force
    Write-Host "移动: CODE_QUALITY_REPORT.md -> docs\quality\" -ForegroundColor Cyan
}

# 迁移文档
if (Test-Path "MIGRATION_TO_DATABASE.md") {
    Move-Item -Path "MIGRATION_TO_DATABASE.md" -Destination "docs\migration\" -Force
    Write-Host "移动: MIGRATION_TO_DATABASE.md -> docs\migration\" -ForegroundColor Cyan
}

# 其他文档
if (Test-Path "SLUG_GENERATION_EXPLAINED.md") {
    Move-Item -Path "SLUG_GENERATION_EXPLAINED.md" -Destination "docs\misc\" -Force
    Write-Host "移动: SLUG_GENERATION_EXPLAINED.md -> docs\misc\" -ForegroundColor Cyan
}

Write-Host "`n文档整理完成！" -ForegroundColor Green
Write-Host "请查看 docs\README.md 了解文档结构" -ForegroundColor Yellow

