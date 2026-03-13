# 开源项目配置指南

本文档指导你如何根据自己的需要配置这个开源项目。

## 需要更新的项目信息

请根据你的实际情况更新以下文件中的信息：

### 1. package.json

```json
{
  "author": "Xie Feng <your-email@example.com>",
  "homepage": "https://github.com/yourusername/personal-site",
  "repository": {
    "url": "https://github.com/yourusername/personal-site.git"
  },
  "bugs": {
    "url": "https://github.com/yourusername/personal-site/issues"
  }
}
```

**需要更新的地方**:
- `author`: 你的名字和邮箱
- `homepage`: 你的 GitHub 项目地址
- `repository.url`: 你的 GitHub 仓库 URL
- `bugs.url`: 你的 GitHub Issues 地址

### 2. README.md

搜索并替换以下内容：
- `your-email@example.com` → 你的实际邮箱
- `yourusername` → 你的 GitHub 用户名
- `https://github.com/yourusername/personal-site` → 你的项目 URL

### 3. CONTRIBUTING.md

- 更新 "如何报告 Bug" 部分中的链接
- 更新 "功能请求" 部分中的链接
- 更新电子邮件地址

### 4. CODE_OF_CONDUCT.md

- 如需更改，更新指导委员会的联系信息

### 5. SECURITY.md

- 更新 `your-email@example.com` 为你的邮箱

### 6. AUTHORS.md

- 更新作者信息
- 添加贡献者信息

### 7. LICENSE

- 确保版权年份和作者名称正确
- 根据需要调整日期

## 项目结构

根目录下的开源相关文档：

```
项目根目录/
├── LICENSE                    # MIT License (开源许可证)
├── CHANGELOG.md              # 更新日志
├── CODE_OF_CONDUCT.md        # 社区行为准则
├── CONTRIBUTING.md           # 贡献指南
├── SECURITY.md               # 安全政策
├── AUTHORS.md                # 作者信息
├── README.md                 # 项目文档
├── docs/open-source/         # 开源相关文档（本目录）
│   ├── OPEN_SOURCE.md        # 开源宣言
│   ├── OPENSOURCE_CHECKLIST.md
│   └── OPENSOURCE_SETUP.md   # 本配置指南
├── .github/
│   ├── ISSUE_TEMPLATE/       # Issue 模板
│   │   ├── bug_report.md
│   │   ├── feature_request.md
│   │   └── documentation.md
│   └── pull_request_template.md  # PR 模板
├── .gitignore               # Git 忽略文件
└── ... 项目文件
```

## 发布流程检查清单

发布到 GitHub 时，请检查以下项目：

### 文档
- [ ] README.md 完整且准确
- [ ] CONTRIBUTING.md 清晰易懂
- [ ] CODE_OF_CONDUCT.md 已设置
- [ ] LICENSE 文件存在
- [ ] AUTHORS.md 已更新

### 配置
- [ ] package.json 信息准确
- [ ] GitHub Issues 模板已配置
- [ ] PR 模板已配置
- [ ] .gitignore 合适

### 代码质量
- [ ] 代码遵循编码规范
- [ ] 已去除敏感信息（密钥、密码等）
- [ ] 所有 TODO 注释已处理或转换为 Issue
- [ ] 测试通过

### Git
- [ ] 提交历史清晰
- [ ] 版本标签正确（如 v1.0.0）
- [ ] 分支结构清晰

## 开启讨论功能

在 GitHub 仓库设置中：

1. 进入 **Settings** → **Discussions**
2. 启用 Discussions
3. 创建以下讨论类别：
   - **Announcements**: 项目公告
   - **General**: 一般讨论
   - **Ideas**: 功能建议
   - **Q&A**: 问答

## CI/CD 建议

考虑添加以下 GitHub Actions：

```yaml
# .github/workflows/test.yml
name: Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-node@v2
        with:
          node-version: '18'
      - run: npm ci
      - run: npm run lint
      - run: npm run test
```

## 保护分支规则

在 **Settings** → **Branches** → **Branch protection rules** 中配置 main 分支：

- [ ] Require pull request reviews before merging
- [ ] Require status checks to pass before merging
- [ ] Require branches to be up to date before merging
- [ ] Include administrators

## 社区工具

推荐集成的工具：

- **Dependabot**: 自动依赖更新
- **CodeQL**: 代码安全分析
- **Changelog**: 自动生成变更日志

## 营销和推广

发布项目后：

1. 在 GitHub Awesome 列表中提交
2. 在 Product Hunt 中展示
3. 在 dev.to 或类似平台写文章
4. 在社交媒体上分享
5. 在相关论坛中讨论

## 长期维护

- ✅ 定期审查和响应 Issues
- ✅ 合并有用的 Pull Requests
- ✅ 更新依赖项
- ✅ 修复安全问题
- ✅ 发布新版本
- ✅ 更新文档

---

准备好了吗？祝你的开源项目成功！🚀
