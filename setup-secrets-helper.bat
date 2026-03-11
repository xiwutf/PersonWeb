@echo off
echo ========================================
echo GitHub Secrets 设置助手
echo ========================================
echo.

echo 你的项目配置信息：
echo.
echo 1. 数据库信息：
echo    - 主机：rm-2zereaqi1k536nd38zo.mysql.rds.aliyuncs.com
echo    - 用户名：xifg
echo    - 数据库名：personal_site
echo.

echo 2. API 基础路径：
echo    - 开发环境：http://localhost:5234/api
echo.

echo 3. OSS 配置（需要新建）：
echo.
echo 推荐的 Bucket 名称：
echo    personweb-2024-%RANDOM%
echo    或者使用你的用户名：lijing327-personweb
echo.
echo 推荐的 Endpoint（选择最近的地域）：
echo    华东1（杭州）：oss-cn-hangzhou.aliyuncs.com
echo    华东2（上海）：oss-cn-shanghai.aliyuncs.com
echo    华南1（深圳）：oss-cn-shenzhen.aliyuncs.com
echo.

echo ========================================
echo 需要在 GitHub Secrets 中设置：
echo.
echo OSS_BUCKET=your-bucket-name
echo OSS_ENDPOINT=oss-cn-hangzhou.aliyuncs.com
echo OSS_AK=your-access-key-id
echo OSS_SK=your-access-key-secret
echo NUXT_PUBLIC_API_BASE=https://your-domain.com/api
echo.
echo ========================================
echo.

:: 提示输入
set /p bucket_name=请输入 Bucket 名称（推荐：personweb-2024-%RANDOM%）：
if "%bucket_name%"=="" set bucket_name=personweb-2024-%RANDOM%

set /p endpoint=请输入 Endpoint（推荐：oss-cn-hangzhou.aliyuncs.com）：
if "%endpoint%"=="" set endpoint=oss-cn-hangzhou.aliyuncs.com

set /p access_key=请输入 AccessKey ID：
if "%access_key%"=="" goto error

set /p secret_key=请输入 AccessKey Secret：
if "%secret_key%"=="" goto error

set /p api_base=请输入 API 基础 URL（生产环境，如：https://your-domain.com/api）：
if "%api_base%"=="" set api_base=https://your-domain.com/api

echo.
echo ========================================
echo 请复制以下内容到 GitHub Secrets：
echo.
echo 1. Name: OSS_BUCKET
echo    Secret: %bucket_name%
echo.
echo 2. Name: OSS_ENDPOINT
echo    Secret: %endpoint%
echo.
echo 3. Name: OSS_AK
echo    Secret: %access_key%
echo.
echo 4. Name: OSS_SK
echo    Secret: %secret_key%
echo.
echo 5. Name: NUXT_PUBLIC_API_BASE
echo    Secret: %api_base%
echo.
echo ========================================
echo.
echo 设置完成后，向 master 分支推送代码即可自动部署！
pause
goto end

:error
echo 错误：请填写所有必需的字段！
pause
exit /b 1

:end
pause