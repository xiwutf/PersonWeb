# 测试文档知识管家 Agent 连接脚本

Write-Host "=== 文档知识管家 Agent 连接测试 ===" -ForegroundColor Cyan
Write-Host ""

# 1. 测试 Python Agent 健康检查
Write-Host "1. 测试 Python Agent 健康检查..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-WebRequest -Uri "http://localhost:8001/health" -Method GET -UseBasicParsing
    Write-Host "   ✓ Python Agent 运行正常" -ForegroundColor Green
    Write-Host "   响应: $($healthResponse.Content)" -ForegroundColor Gray
} catch {
    Write-Host "   ✗ Python Agent 未运行或无法访问" -ForegroundColor Red
    Write-Host "   错误: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 2. 测试文档处理接口（需要 Token）
Write-Host "2. 测试文档处理接口..." -ForegroundColor Yellow

# 从配置读取 Token（这里使用默认值，实际应该从配置文件读取）
$token = "default-internal-token-change-in-production"

# 创建一个测试文件路径（需要根据实际情况修改）
$testFilePath = "D:\00-Project\AI\PersonWeb\backend\PersonalSite.Api\uploads\documents\test.txt"

# 检查文件是否存在
if (-not (Test-Path $testFilePath)) {
    Write-Host "   警告: 测试文件不存在: $testFilePath" -ForegroundColor Yellow
    Write-Host "   请先上传一个文档，或创建测试文件" -ForegroundColor Yellow
} else {
    Write-Host "   ✓ 测试文件存在: $testFilePath" -ForegroundColor Green
}

$requestBody = @{
    document_id = "test-001"
    file_path = $testFilePath
    file_type = "txt"
    user_id = "test-user"
} | ConvertTo-Json

try {
    $headers = @{
        "X-Internal-Token" = $token
        "Content-Type" = "application/json"
    }
    
    Write-Host "   发送请求到: http://localhost:8001/api/ai/document/process" -ForegroundColor Gray
    Write-Host "   Token: $token" -ForegroundColor Gray
    
    $response = Invoke-WebRequest -Uri "http://localhost:8001/api/ai/document/process" `
        -Method POST `
        -Headers $headers `
        -Body $requestBody `
        -ContentType "application/json" `
        -UseBasicParsing
    
    Write-Host "   ✓ 请求成功" -ForegroundColor Green
    Write-Host "   状态码: $($response.StatusCode)" -ForegroundColor Gray
    Write-Host "   响应: $($response.Content)" -ForegroundColor Gray
} catch {
    Write-Host "   ✗ 请求失败" -ForegroundColor Red
    Write-Host "   错误: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "   响应内容: $responseBody" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== 测试完成 ===" -ForegroundColor Cyan

