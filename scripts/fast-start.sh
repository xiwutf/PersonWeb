#!/bin/bash

# 快速启动脚本 - 优化后的启动流程

echo "🚀 启动溪午听风个人网站（优化模式）"
echo ""

# 1. 检查 node_modules
if [ ! -d "node_modules" ]; then
    echo "📦 首次启动，正在安装依赖..."
    npm install
fi

# 2. 检查是否需要优化
if [ -f "package.json" ]; then
    # 检查 node_modules 大小
    MODULES_SIZE=$(du -sh node_modules 2>/dev/null | cut -f1)
    echo "📊 node_modules 大小: $MODULES_SIZE"

    # 如果超过 500MB，提示清理
    SIZE_MB=$(du -sm node_modules 2>/dev/null | cut -f1)
    if (( $(echo "$SIZE_MB > 500" | bc -l) )); then
        echo "⚠️  node_modules 较大，建议运行清理："
        echo "   npm run clean:modules"
        echo ""
    fi
fi

# 3. 检查是否有环境变量文件
if [ ! -f ".env" ] && [ -f ".env.example" ]; then
    echo "📝 创建 .env 文件..."
    cp .env.example .env
    echo "✅ .env 文件已创建"
    echo ""
fi

# 4. 启动开发服务器
echo "🏃 启动开发服务器..."
echo "   地址: http://localhost:3000"
echo "   使用 Ctrl+C 停止"
echo ""

npm run dev