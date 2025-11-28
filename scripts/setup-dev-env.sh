#!/bin/bash
# Linux/macOS 开发环境快速配置脚本
# 使用方法：chmod +x scripts/setup-dev-env.sh && ./scripts/setup-dev-env.sh

echo "========================================"
echo "  开发环境快速配置脚本"
echo "========================================"
echo ""

# 检查 Node.js
echo "检查 Node.js..."
if command -v node &> /dev/null; then
    NODE_VERSION=$(node --version)
    echo "✓ Node.js 已安装: $NODE_VERSION"
else
    echo "✗ Node.js 未安装，请先安装 Node.js 18+"
    exit 1
fi

# 检查 .NET SDK
echo "检查 .NET SDK..."
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo "✓ .NET SDK 已安装: $DOTNET_VERSION"
else
    echo "✗ .NET SDK 未安装，请先安装 .NET 8.0+"
    exit 1
fi

# 检查 MySQL（可选）
echo "检查 MySQL..."
if command -v mysql &> /dev/null; then
    echo "✓ MySQL 已安装"
else
    echo "⚠ MySQL 未检测到，请确保数据库服务可用"
fi

echo ""
echo "开始配置环境..."

# 创建前端 .env 文件
if [ ! -f ".env" ]; then
    echo "创建 .env 文件..."
    if [ -f ".env.example" ]; then
        cp .env.example .env
        echo "✓ .env 文件已创建，请编辑配置"
    fi
else
    echo "✓ .env 文件已存在"
fi

# 创建后端配置文件
BACKEND_CONFIG="backend/PersonalSite.Api/appsettings.Development.json"
if [ ! -f "$BACKEND_CONFIG" ]; then
    echo "创建后端配置文件..."
    EXAMPLE_CONFIG="backend/PersonalSite.Api/appsettings.Development.json.example"
    if [ -f "$EXAMPLE_CONFIG" ]; then
        cp "$EXAMPLE_CONFIG" "$BACKEND_CONFIG"
        echo "✓ 后端配置文件已创建，请编辑数据库连接字符串"
    fi
else
    echo "✓ 后端配置文件已存在"
fi

# 安装前端依赖
echo ""
echo "安装前端依赖..."
if [ -d "node_modules" ]; then
    echo "检测到 node_modules，跳过安装（如需重新安装，请先删除 node_modules）"
else
    npm install
    if [ $? -eq 0 ]; then
        echo "✓ 前端依赖安装完成"
    else
        echo "✗ 前端依赖安装失败"
        exit 1
    fi
fi

# 恢复后端依赖
echo ""
echo "恢复后端依赖..."
cd backend/PersonalSite.Api
dotnet restore
if [ $? -eq 0 ]; then
    echo "✓ 后端依赖恢复完成"
else
    echo "✗ 后端依赖恢复失败"
    cd ../..
    exit 1
fi
cd ../..

echo ""
echo "========================================"
echo "  配置完成！"
echo "========================================"
echo ""
echo "下一步："
echo "1. 编辑 .env 文件配置 API 地址"
echo "2. 编辑 backend/PersonalSite.Api/appsettings.Development.json 配置数据库"
echo "3. 执行数据库脚本创建表结构"
echo "4. 启动后端: cd backend/PersonalSite.Api && dotnet run"
echo "5. 启动前端: npm run dev"
echo ""

