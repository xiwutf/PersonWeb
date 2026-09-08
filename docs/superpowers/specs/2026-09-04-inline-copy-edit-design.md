# 前台登录可编辑展示文案（方案 1）设计文档

**日期**：2026-09-04  
**状态**：已确认，P0+P1 已实现  
**范围**：方案 A + 方案 1（YAML 展示文案；点哪里改哪里）

---

## 一、目标

管理员通过 `/admin/login` 登录后，可在前台公开页（`/life`、`/work` 等）直接点击文案进行编辑并保存；未登录访客只读，页面外观与现在一致（无编辑框、无铅笔图标）。

成功标准：

1. 未登录：HTML/交互与现网一致，无编辑 UI，无写接口可调用成功。
2. 已登录：轻量「已登录 · 可编辑」提示；可点文字编辑并保存到 PRIMARY SoT 文件。
3. 写接口必须 `checkAuth`；白名单外路径一律拒绝。
4. Git 文件仍是唯一业务源（PRIMARY）；不引入第二套 CMS 库表。

---

## 二、非目标（明确不做）

- 不在前台编辑导航 / IA（`constants/work-ia.ts`）
- 不在前台编辑文章 / 项目 / 工具（继续走 `/admin/*`）
- 不做富文本、图片上传、拖拽排版
- 不做多用户 / 细粒度角色（只有 admin）
- 不自动 git commit / push

---

## 三、权限模型

| 层 | 行为 |
| --- | --- |
| **登录** | 现有 `/admin/login` → httpOnly `admin_token` |
| **探测** | `GET /api/auth/session` → `{ authenticated }`（`credentials: 'include'`） |
| **写保护** | Nitro 写接口调用 `checkAuth(event)`；失败 401 |
| **UI 门闩** | 仅 `authenticated === true` 时挂载编辑能力；前端隐藏不是安全边界 |

前端新增轻量 composable（建议 `useAdminSession`）：客户端拉取 session 一次，暴露 `isAdmin`、`pending`；失败视为未登录。

---

## 四、数据与 API

### 4.1 PRIMARY 白名单（可写）

| World | 文件 | 首期页面 |
| --- | --- | --- |
| Life | `content/life/home.yml` | `/life` |
| Work | `content/work/home.yml` | `/work` |

后续同管道扩展（同设计，不另起炉灶）：

- Work：`about.md`、`ai.yml`、`capabilities.yml`、`contact.yml`
- Life：`now.yml`、`moments.yml`、`profile.md`

### 4.2 读（已有）

保持：

- `GET /api/content/life/home`
- `GET /api/content/work/home`

### 4.3 写（新增）

建议统一 PATCH（字段级更新，避免整文件覆盖丢注释/未编辑键）：

```
PATCH /api/content/life/home
PATCH /api/content/work/home
```

请求体：

```json
{
  "path": "hero.greeting",
  "value": "你好，我是"
}
```

规则：

1. `checkAuth` 必须通过。
2. `path` 必须命中该文件的**允许字段白名单**（点分路径；数组用 `hero.lines.0`）。
3. 服务端：读 YAML → 按 path 写入 → `yaml.stringify` 写回磁盘 → 返回更新后的完整内容对象（与 GET 同形）。
4. 非法 path / 类型不符 → 400；未登录 → 401；磁盘不可写 → 503。
5. 写成功后对该资源的公开 GET 缓存应失效或缩短（现有 `Cache-Control: public, max-age=60` 可接受；可选写后 `setHeader` 无缓存）。

实现落点：

- 读写扩展：`server/utils/content-files.ts`（`writeYamlFile` + path setter）
- 路由：`server/api/content/life/home.patch.ts`、`server/api/content/work/home.patch.ts`
- 禁止任意路径拼接（不得接受 `../../`）

### 4.4 生产约束

站点进程必须对 `content/` 有写权限。纯静态 `generate` 托管无法持久化；本功能面向 **Node Nitro 运行时**（`nuxt build` + `node .output/server`）或本地 `npm run dev`。规格与实现注释中写明；写失败时 UI toast「无法写入内容文件」。

---

## 五、前端交互（方案 1）

### 5.1 组件

新增 `components/content/InlineEditableText.vue`（名称可微调）：

Props：

- `modelValue` / 显示文本
- `fieldPath`：如 `hero.name`
- `as`：语义标签 `p | h1 | h2 | span | small | strong`（默认 `span`）
- `multiline`：可选，textarea
- `disabled`：未登录时强制只读（也可由父级不渲染该组件）

行为：

1. 未登录：渲染普通文本节点，**无** contenteditable / 无悬停框 / 无额外 DOM 噪音（可用普通插槽或 `v-if="!isAdmin"` 回退到纯文本）。
2. 已登录：悬停细框（用 tokens：`--color-border-*` / `--color-primary-soft`，禁止硬编码色）。
3. 单击进入编辑；`Enter` 保存（单行）、`Shift+Enter` 换行（多行）、`Esc` 取消、失焦保存或确认按钮（推荐：失焦保存 + Esc 取消）。
4. 保存中禁用输入；成功后更新本地 state；失败 toast 并回滚显示值。

### 5.2 页面挂载

**Phase 1a — `/life`**（`pages/life/index.vue` + `content/life/home.yml`）

可编辑字段（字符串）：

- `hero.kicker` / `hero.greeting` / `hero.name` / `hero.lines.*` / `closing`
- `sections.*.title` / `sections.*.number`（number 可选；建议首期只开 title）
- `empty.moments` / `empty.notes`
- `about.description` / `about.linkText`

**Phase 1b — `/work`**（`pages/work.vue` + `content/work/home.yml`）

可编辑字段：

- `brand.name` / `brand.tagline`
- `hero.kicker` / `hero.title` / `hero.english_name`（API 侧为 `englishName`，写回 YAML 键 `english_name`）
- `hero.role` / `hero.value`
- `hero.actions.*.label`（href/to/variant 首期只读）
- `panel.focus.*` / `panel.status.*` / `panel.tools.*` / `panel.current_suffix`
- `sections.*.title` / `sections.*.description` / `sections.*.label` / `sections.*.link_text`
- `footer.name` / `footer.description`
- `seo.title` / `seo.description`（可选；无可见节点则首期跳过）

不编辑（首期）：

- `hero.links`（来自 `contact.yml` SoT）
- `contact.rows`（contact SoT）
- 项目列表正文（DB）

### 5.3 登录提示条

已登录时在 `/life`、`/work` 顶部（header 下或角落）显示极轻提示：

> 已登录 · 点击文案可编辑

一条即可；不挡主内容；不出现在未登录。

### 5.4 性能

- Session 探测仅客户端、懒加载；不阻塞 SSR 首屏文案。
- 编辑组件与写逻辑不进入未登录访客的关键路径包（`isAdmin` 为假时不动态 import 重型编辑器——本方案仅用原生 input/textarea，包体可忽略）。
- 公开页保持 SSR；不因编辑功能 `ssr: false`。

---

## 六、错误处理

| 情况 | 行为 |
| --- | --- |
| 未登录调 PATCH | 401；UI 不应发出 |
| path 不在白名单 | 400 |
| YAML 解析失败 | 500；保留原文件不写 |
| 磁盘只读 / 权限不足 | 503 + 明确文案 |
| 保存冲突（可选） | 首期不做乐观锁；后写覆盖 |

---

## 七、测试

1. 单元：path whitelist、setter、未授权 PATCH。
2. 集成：登录后 PATCH `life/home` 改 `hero.name`，GET 返回新值；文件内容变更。
3. UI 守卫：未登录页面源码/DOM 不出现「可编辑」提示（至少组件分支测试）。

---

## 八、文档联动

实现后更新：

- `docs/CONTENT_ARCHITECTURE.md`：Work/Life Display copy 的 Admin 行改为「前台 inline edit → Nitro PATCH → 文件」
- `docs/PROJECT_STRUCTURE_GUIDE.md`：内容编辑位置补充前台登录编辑

---

## 九、实施分期

| 期 | 交付 |
| --- | --- |
| **P0** | `useAdminSession` + `InlineEditableText` + Life `home` PATCH + `/life` 接线 |
| **P1** | Work `home` PATCH + `/work` 接线 |
| **P2** | about / ai / capabilities / contact / life now·moments·profile（同模式复制） |

默认一次交付 **P0 + P1**；P2 另开任务。

---

## 十、风险

1. **生产写盘**：若部署为纯静态，功能不可用——需 Node 服务 + 可写 `content/`。
2. **YAML 注释**：`yaml.stringify` 可能丢掉原文件注释；可接受，或写回时尽量保持 key 顺序。
3. **现有弱鉴权**：沿用当前 admin token 机制；本功能不单独升级鉴权体系，但写接口绝不能绕过 `checkAuth`。
