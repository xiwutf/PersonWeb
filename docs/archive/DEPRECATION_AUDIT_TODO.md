# 废弃文件审计待办清单

本清单记录了“疑似废弃，但还不适合直接删除”的文件。

处理原则：

- 先确认是否仍在真实流程中使用
- 再决定删除、归档或保留
- 删除前同步更新对应 README 或文档说明

## 已直接删除

以下文件已在本轮审计中删除，因为证据比较充分：

- `pages/admin/articles/index-with-naive.vue.example`
- `components/OptimizedImage.vue`
- `scripts/clean-git-history.ps1`

## 需人工确认

### 1. 启动辅助脚本

候选文件：

- `scripts/fast-start.ps1`
- `scripts/fast-start.sh`

当前观察：

- 没有被 `README.md`、`package.json` 或部署主流程引用
- 只在 `scripts/README.md` 和 `scripts/benchmark.js` 中出现

确认问题：

- 团队是否还有人依赖这两个脚本做本地快速启动？
- 如果不再使用，是否要同步删除 `scripts/README.md` 中对应说明？

建议：

- 如果已被 `setup-dev-env.*` 或标准 `npm run dev` 流程替代，可删除

### 2. 内容导库脚本

候选文件：

- `scripts/import-all-content-to-db.js`
- `scripts/import-all-content-to-db.md`
- `scripts/import-blog-to-db.js`
- `scripts/import-blog-to-db.md`

当前观察：

- 只被各自脚本文档引用
- 没有进入主 README、package scripts 或部署主入口

确认问题：

- 现在是否还使用“Markdown 导入数据库”的流程？
- 这些脚本是否只属于历史迁移阶段？

建议：

- 如果内容管理已经改为后台录入或其他数据流，这组文件可以整体删除或移入归档

### 3. 模块安装相关 composable

候选文件：

- `composables/useModuleInstaller.ts`

当前观察：

- 主项目代码中未发现使用点
- 当前模块商店页面直接使用的是 `useModuleManager`

确认问题：

- 这是否是未落地的旧方案？
- 后续是否还计划接入安装进度、批量安装等逻辑？

建议：

- 如果确认不再接入，可删除
- 如果未来会恢复，可保留并补充文档说明用途

### 4. 模块配置 composable

候选文件：

- `composables/useModuleConfig.ts`

当前观察：

- 主项目代码未使用
- 仅在 `examples/` 示例模块语境中出现

确认问题：

- 是否仍保留模块示例体系？
- 如果示例仍然有教学价值，这个文件可能应保留

建议：

- 若不再维护示例模块体系，可与 `examples/` 一起评估清理

### 5. 字体样式 composable

候选文件：

- `composables/useFontStyle.ts`

当前观察：

- 代码侧未发现实际调用
- 只在文档和结构说明中被提及

确认问题：

- 是否计划在后台字体管理或主题系统中接入？
- 若不接入，文档是否也应一并清理？

建议：

- 若确认未使用，可删除并同步更新文档索引

## 下一步建议

建议按下面顺序清理：

1. 先确认 `scripts/fast-start.*`
2. 再确认 `scripts/import-*-to-db.*`
3. 最后确认 `composables/` 下未接入能力

这样对现有运行流程影响最小。
