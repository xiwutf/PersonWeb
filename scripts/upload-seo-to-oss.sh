#!/bin/bash
# 仅上传 SEO 文件到 OSS（需已安装 ossutil 并配置 ~/.ossutilconfig）
# 用法:
#   export OSS_BUCKET=你的桶名
#   bash scripts/upload-seo-to-oss.sh

set -e
cd "$(dirname "$0")/.."

node scripts/generate-sitemap.js

if [ -z "$OSS_BUCKET" ]; then
  echo "请设置环境变量 OSS_BUCKET，例如: export OSS_BUCKET=my-bucket"
  exit 1
fi

if ! command -v ossutil &> /dev/null; then
  echo "未找到 ossutil，请先安装: https://help.aliyun.com/document_detail/120075.html"
  exit 1
fi

ossutil cp public/robots.txt "oss://${OSS_BUCKET}/robots.txt" -f
ossutil cp public/sitemap.xml "oss://${OSS_BUCKET}/sitemap.xml" -f

echo "✅ 已上传 robots.txt、sitemap.xml 到 oss://${OSS_BUCKET}/"
echo "验证: curl -sI https://xifg.com.cn/robots.txt"
