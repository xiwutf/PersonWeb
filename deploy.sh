#!/bin/bash

# 生产环境部署脚本
# 用于将 Nuxt 3 应用部署到生产环境

set -e  # 遇到错误立即退出

echo "🚀 开始生产环境部署..."

# 颜色输出
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# 检查 Node.js 版本
echo -e "${YELLOW}检查 Node.js 版本...${NC}"
node_version=$(node -v)
echo "当前 Node.js 版本: $node_version"

# 安装依赖
echo -e "${YELLOW}安装依赖...${NC}"
npm ci --production=false

# 使用生产环境变量构建
echo -e "${YELLOW}使用生产环境配置构建...${NC}"
export NODE_ENV=production
export NUXT_PUBLIC_API_BASE=https://api.xifg.com.cn/api

# 构建应用
npm run build:prod

# 检查构建结果
if [ -d ".output" ]; then
    echo -e "${GREEN}✅ 构建成功！${NC}"
    echo -e "${GREEN}构建输出目录: .output${NC}"
    
    # 显示构建信息
    echo -e "${YELLOW}构建信息:${NC}"
    echo "  - API 基础路径: https://api.xifg.com.cn/api"
    echo "  - 环境: production"
    echo "  - 输出目录: .output"
    
    # 可选：显示文件大小
    if command -v du &> /dev/null; then
        echo -e "${YELLOW}构建产物大小:${NC}"
        du -sh .output
    fi
else
    echo -e "${RED}❌ 构建失败！未找到 .output 目录${NC}"
    exit 1
fi

echo -e "${GREEN}🎉 部署准备完成！${NC}"
echo ""
echo "下一步操作："
echo "  1. 将 .output 目录部署到服务器"
echo "  2. 配置 Nginx 或其他 Web 服务器"
echo "  3. 确保 API 服务 https://api.xifg.com.cn/api 可访问"

