#!/bin/bash

# AI 服务启动失败快速诊断脚本
# 专门用于诊断退出码 3 的问题

echo "=== AI 服务启动失败诊断（退出码 3）==="
echo ""

# 1. 检查服务文件中的启动命令
echo "1. 检查服务文件启动命令..."
if [ -f /etc/systemd/system/ai-service.service ]; then
    EXEC_START=$(grep "^ExecStart=" /etc/systemd/system/ai-service.service | head -1)
    echo "当前命令: $EXEC_START"
    
    if echo "$EXEC_START" | grep -q "^ExecStart=/srv/ai-service/venv/bin/uvicorn[[:space:]]"; then
        echo "❌ 错误：使用了直接调用 uvicorn 的方式"
        echo "   应该使用: python -m uvicorn"
        echo ""
        echo "修复方法："
        echo "  sudo sed -i 's|^ExecStart=/srv/ai-service/venv/bin/uvicorn|ExecStart=/srv/ai-service/venv/bin/python -m uvicorn|g' /etc/systemd/system/ai-service.service"
        echo "  sudo systemctl daemon-reload"
        echo "  sudo systemctl restart ai-service"
    else
        echo "✓ 启动命令格式正确"
    fi
else
    echo "❌ 服务文件不存在"
fi
echo ""

# 2. 检查 Python 环境
echo "2. 检查 Python 环境..."
if [ -f /srv/ai-service/venv/bin/python ]; then
    PYTHON_VERSION=$(/srv/ai-service/venv/bin/python --version 2>&1)
    echo "✓ Python 版本: $PYTHON_VERSION"
else
    echo "❌ Python 可执行文件不存在: /srv/ai-service/venv/bin/python"
fi
echo ""

# 3. 检查 uvicorn
echo "3. 检查 uvicorn..."
if [ -f /srv/ai-service/venv/bin/uvicorn ]; then
    echo "✓ uvicorn 可执行文件存在"
    UVICORN_VERSION=$(/srv/ai-service/venv/bin/uvicorn --version 2>&1 || echo "无法获取版本")
    echo "  版本: $UVICORN_VERSION"
else
    echo "❌ uvicorn 可执行文件不存在"
    echo "   尝试安装: cd /srv/ai-service && source venv/bin/activate && pip install uvicorn[standard]"
fi
echo ""

# 4. 测试模块导入
echo "4. 测试应用模块导入..."
cd /srv/ai-service
IMPORT_ERROR=$(sudo -u www-data /srv/ai-service/venv/bin/python -c "from app.main import app" 2>&1)
if [ $? -eq 0 ]; then
    echo "✓ 模块导入成功"
else
    echo "❌ 模块导入失败"
    echo "错误信息："
    echo "$IMPORT_ERROR"
    echo ""
    echo "可能的原因："
    echo "  - 缺少依赖包，运行: pip install -r requirements.txt"
    echo "  - app/__init__.py 文件缺失"
    echo "  - 导入的模块文件不存在"
fi
echo ""

# 5. 检查工作目录和文件
echo "5. 检查关键文件..."
FILES_TO_CHECK=(
    "/srv/ai-service/app/main.py"
    "/srv/ai-service/app/__init__.py"
    "/srv/ai-service/app/core/config.py"
    "/srv/ai-service/app/core/app_logging.py"
    "/srv/ai-service/requirements.txt"
)

for file in "${FILES_TO_CHECK[@]}"; do
    if [ -f "$file" ]; then
        echo "✓ $(basename $file) 存在"
    else
        echo "❌ $(basename $file) 不存在: $file"
    fi
done
echo ""

# 6. 检查权限
echo "6. 检查权限..."
if [ -d /srv/ai-service ]; then
    OWNER=$(stat -c '%U:%G' /srv/ai-service 2>/dev/null || stat -f '%Su:%Sg' /srv/ai-service 2>/dev/null)
    echo "目录所有者: $OWNER"
    
    if sudo -u www-data test -r /srv/ai-service/app/main.py 2>/dev/null; then
        echo "✓ www-data 可以读取 main.py"
    else
        echo "❌ www-data 无法读取 main.py"
        echo "   修复: sudo chown -R www-data:www-data /srv/ai-service"
    fi
    
    if sudo -u www-data test -x /srv/ai-service/venv/bin/python 2>/dev/null; then
        echo "✓ www-data 可以执行 Python"
    else
        echo "❌ www-data 无法执行 Python"
    fi
fi
echo ""

# 7. 检查日志目录
echo "7. 检查日志目录..."
if [ -d /srv/ai-service/logs ]; then
    echo "✓ 日志目录存在"
    if sudo -u www-data test -w /srv/ai-service/logs 2>/dev/null; then
        echo "✓ www-data 可以写入日志目录"
    else
        echo "❌ www-data 无法写入日志目录"
        echo "   修复: sudo chmod 775 /srv/ai-service/logs"
    fi
    
    if [ -f /srv/ai-service/logs/ai-service.log ]; then
        echo "最近的日志（最后 20 行）："
        tail -n 20 /srv/ai-service/logs/ai-service.log 2>/dev/null || echo "无法读取日志文件"
    fi
else
    echo "⚠️  日志目录不存在"
    echo "   创建: sudo mkdir -p /srv/ai-service/logs && sudo chown www-data:www-data /srv/ai-service/logs"
fi
echo ""

# 8. 查看系统日志
echo "8. 最近的系统日志（最后 30 行）："
sudo journalctl -u ai-service -n 30 --no-pager 2>/dev/null || echo "无法读取系统日志"
echo ""

# 9. 检查端口占用
echo "9. 检查端口占用..."
if sudo netstat -tlnp 2>/dev/null | grep -q ":8001 " || sudo ss -tlnp 2>/dev/null | grep -q ":8001 "; then
    echo "⚠️  端口 8001 已被占用"
    sudo netstat -tlnp 2>/dev/null | grep ":8001 " || sudo ss -tlnp 2>/dev/null | grep ":8001 "
else
    echo "✓ 端口 8001 未被占用"
fi
echo ""

# 10. 尝试手动启动并捕获错误
echo "10. 尝试手动启动（捕获详细错误）..."
cd /srv/ai-service
echo "执行命令: sudo -u www-data /srv/ai-service/venv/bin/python -m uvicorn app.main:app --host 127.0.0.1 --port 8001"
echo ""

# 使用 timeout 限制运行时间，并捕获输出
sudo -u www-data timeout 5 /srv/ai-service/venv/bin/python -m uvicorn app.main:app --host 127.0.0.1 --port 8001 2>&1 || {
    EXIT_CODE=$?
    echo "退出码: $EXIT_CODE"
    if [ $EXIT_CODE -eq 124 ]; then
        echo "✓ 服务启动成功（5秒后自动停止）"
    else
        echo "❌ 服务启动失败"
    fi
}
echo ""

echo "=== 诊断完成 ==="
echo ""
echo "如果问题仍然存在，请运行修复脚本："
echo "  bash /srv/ai-service/fix-service.sh"
echo ""
echo "或查看完整日志："
echo "  sudo journalctl -u ai-service -n 100 --no-pager"

