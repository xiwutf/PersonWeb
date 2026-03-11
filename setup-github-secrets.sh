#!/bin/bash

# GitHub Secrets 设置脚本
# 使用方法：./setup-github-secrets.sh <username> <repo-name>

USERNAME=$1
REPO_NAME=$2

if [ -z "$USERNAME" ] || [ -z "$REPO_NAME" ]; then
    echo "Usage: ./setup-github-secrets.sh <username> <repo-name>"
    echo "Example: ./setup-github-secrets.sh Lijing327 PersonWeb"
    exit 1
fi

# GitHub API URL
API_URL="https://api.github.com/repos/$USERNAME/$REPO_NAME"

# 设置 GitHub token (需要提前设置)
# 你可以从 https://github.com/settings/tokens 创建 Personal Access Token
# 需要: repo, workflow 权限
if [ -z "$GITHUB_TOKEN" ]; then
    echo "请设置 GITHUB_TOKEN 环境变量"
    echo "或者直接编辑下面的 token 值"
    GITHUB_TOKEN="your_github_token_here"
fi

# 要设置的 secrets
SECRETS=(
    "OSS_BUCKET=your-bucket-name"
    "OSS_ENDPOINT=oss-cn-hangzhou.aliyuncs.com"
    "OSS_AK=your-access-key-id"
    "OSS_SK=your-access-key-secret"
    "NUXT_PUBLIC_API_BASE=https://your-domain.com/api"
)

# 设置每个 secret
for secret in "${SECRETS[@]}"; do
    name=$(echo "$secret" | cut -d'=' -f1)
    value=$(echo "$secret" | cut -d'=' -f2-)

    echo "Setting secret: $name"

    # 使用 GitHub API 设置 secret
    curl -X PUT \
        -H "Authorization: token $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github.v3+json" \
        "$API_URL/actions/secrets/$name" \
        -d "{\"encrypted_value\":\"$value\"}"

    echo -e "\n"
done

echo "所有 secrets 设置完成！"