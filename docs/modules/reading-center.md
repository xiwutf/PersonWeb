# 私人阅读中心

## 当前边界

阶段一提供 .NET 数据模型、私有 EPUB 导入/读取、分页查询和密码持久授权。
阶段二提供 `/library` 响应式封面书架、上传、搜索与分页，点击封面直达 `/library/read/{id}`。
阅读器支持目录、分页、字号、浅深主题、键盘/触控翻页及可选全屏；独立布局保留原有授权门禁。
阶段三接通自动保存和跨设备进度同步；书架支持删除与标记已读；不实现博客发布功能。
现有阅读 Inbox 的 `/api/reading` 和公开摘句独立保留，禁止把私人书籍/笔记写入这些入口。

本次变更文件：

- 后端入口：`backend/PersonalSite.Api/Program.cs`、`Data/AppDbContext.cs`、
  `Controllers/ReadingAuthController.cs`、`Controllers/LibraryController.cs`。
- 后端模型：`Models/ReadingCenterModels.cs`、`Models/ReadingSession.cs`、
  `Data/ReadingCenterModelConfiguration.cs`。
- 后端服务：`Services/Reading/ReadingLibraryOptions.cs`、`ReadingLibraryService.cs`、
  `ReadingOwnerFilter.cs`、`ReadingSessionService.cs`、`ReadingSecurityRegistration.cs`。
  以上后端缩写路径均相对 `backend/PersonalSite.Api/`。
- 前端：`app.vue`、`layouts/library.vue`、`pages/library/index.vue`、
  `components/library/LibraryAccessGate.vue`、`composables/useLibraryAccess.ts`、
  `assets/css/library-access.css`。
- 测试：`backend/PersonalSite.Api.Tests/ReadingCenter/ReadingLibraryHttpTests.cs`、
  `ReadingLibraryServiceTests.cs`（同目录）、`test/utils/library-access.test.ts`。
- 配置与迁移：`scripts/New-ReadingAccessConfig.ps1`、
  `database/20261013_reading_ownerkey_sessions_v01.sql`、本文。

原博客 `AuthController.cs` 已恢复原实现，不再添加此前数据库阅读登录标记。

## 数据库变更

使用现有 MySQL 8 数据库和手工 SQL 方式。不要新建数据库，不在启动时自动建阅读表。
先备份并确认现有 `user.id` 是 signed BIGINT、表使用 InnoDB，然后运行：

```sh
mysql -h <host> -u <user> -p <existing_database>
```

在 MySQL 客户端中：

```sql
SOURCE D:/00-Project/AI/PersonWeb/database/20261013_reading_center_v01.sql;
SHOW TABLES LIKE 'reading_%';
SHOW CREATE TABLE reading_books;
SHOW CREATE TABLE reading_progress;
SHOW CREATE TABLE reading_annotations;
SHOW CREATE TABLE reading_preferences;
```

脚本版本由文件名标识；重复执行不重复创建，但 `IF NOT EXISTS` 不会修复已有表的结构差异。
在目标环境人工审核执行；本次开发不自动执行脚本。此处描述旧版前置结构；下面的OwnerKey迁移解除用户外键。
进度和标注通过 `(owner_id, book_id)` 复合外键保证归属一致。数据库时间由应用写入 UTC。
MySQL DDL 不是整体事务；执行失败需检查已建表，不能假定全部回滚。

## 阅读访问密码与持久会话

阅读中心使用独立访问密码，保留博客现有登录机制；不使用用户名、博客用户 ID 或 JWT。
使用 .NET 8 共享框架的 ASP.NET Core Identity PasswordHasher（PBKDF2、随机盐、恒定时间校验）。
服务端配置只保存 Identity V3 SHA-512 密码哈希（100000..1000000 次迭代），不保存访问密码明文。

运行 PowerShell 7 的交互工具：

```powershell
pwsh -File scripts/New-ReadingAccessConfig.ps1
# 更换密码时必须保留现有 OwnerKey：
pwsh -File scripts/New-ReadingAccessConfig.ps1 -OwnerKey <existing-owner-uuid>
```

输出哈希和首次设置用的随机 OwnerKey，放入不提交的后端配置或环境变量：

```json
{
  "ReadingLibrary": {
    "OwnerKey": "<永久固定的小写UUID>",
    "AccessPasswordHash": "<工具生成的Identity密码哈希>",
    "AllowedOrigins": ["https://<实际前端域名>"],
    "SessionDays": 30,
    "AbsoluteSessionDays": 90,
    "StorageRoot": "App_Data/reading-private"
  }
}
```

环境变量为 ReadingLibrary__OwnerKey、ReadingLibrary__AccessPasswordHash、
ReadingLibrary__AllowedOrigins__0（更多域名按序编号）、ReadingLibrary__SessionDays、
ReadingLibrary__AbsoluteSessionDays。OwnerKey 不因密码/域名变更而变化，缺失或配置不安全返回503。
访问密码未设置示例值，也不复用博客 ADMIN_PASSWORD。

### 线上首次启用清单（门禁出现「服务暂不可用」时按此核对）

1. **发布含 `/api/library` 的 API 构建**，并确认 Nginx 把 `api.xifg.com.cn/api/library` 转到 .NET（不是静态站或 Nitro）。
2. **数据库**：空库先 `SOURCE 20261013_reading_center_v01.sql`，再按备份确认流程执行
   `20261013_reading_ownerkey_sessions_v01.sql`（或 `_preflight` + `_manual`），直到存在
   `reading_sessions` 且四张书/进度表为 `owner_key`。
3. **服务器配置**（`appsettings.Production.json` 或环境变量，勿提交仓库）：

```json
{
  "ReadingLibrary": {
    "OwnerKey": "<用 New-ReadingAccessConfig.ps1 生成并永久固定>",
    "AccessPasswordHash": "<同一脚本生成的 Identity 哈希>",
    "SessionDays": 30,
    "AbsoluteSessionDays": 90,
    "AllowedOrigins": ["https://xifg.com.cn"],
    "StorageRoot": "/var/lib/personweb/reading-private"
  }
}
```

`AllowedOrigins` 必须与浏览器地址栏 Origin **完全一致**（含 `https://`，无尾斜线；若用 `www` 或 `xlfg` 别名要各加一条）。
4. **HTTPS**：前端与 `https://api.xifg.com.cn` 全程 HTTPS；Cookie 名 `__Secure-personweb_reading`，跨 HTTP 不可用。
5. 本机生成哈希：`pwsh -File scripts/New-ReadingAccessConfig.ps1`，把输出写入服务器后 **重启 API**。
6. 浏览器开发者工具看 `GET /api/library/auth/session`：`reading_not_configured`=缺配置；
   CORS/网络失败=Origin 未放行；DB 异常多表现为 503 `reading_unavailable`。

会话 Cookie 为 __Secure-personweb_reading，HttpOnly、Secure、SameSite=Strict、host-only，
Path=/api/library，Expires 默认为30天。随机标识为256位，数据库仅存SHA-256哈希、归属、
创建/有效/绝对到期时间和撤销时间。每次请求检查会话；密码哈希或 OwnerKey 变化后旧会话失效。
主动退出先持久化撤销再删除Cookie；续期不得超过初次授权后的90天，不自动改变会话标识。
前端首次加载检查会话，剩余不足7天时续期，并每12小时重新检查；Cookie凭据不放localStorage。

所有当前和未来的 MVC /api/library 接口由全局 ReadingOwnerFilter 保护，只有密码登录与
会话探测入口例外（探测仍需要有效Cookie才能成功）。旧博客/Nitro Bearer token 不授予阅读权限。
写操作要求精确匹配 AllowedOrigins 的 Origin 和 X-Reading-CSRF；CSRF值由会话随机标识
派生，通过登录/会话探测响应取得，客户端仅保存在内存中。登录也检查Origin，防止登录CSRF。
阅读CORS策略仅允许配置来源及credentials，不继承博客AllowAll。会话响应全部no-store。
密码入口全局每分钟最多5次（含正确密码），超限429、Retry-After=60，不依赖代理IP。
这是单进程内存限速；多实例部署前需网关统一限速。目前不引入分布式服务。

生产必须HTTPS，并且前端与API应同站点（例如同一主域名下的子域名）；Strict Cookie不支持
无关联站点的跨站部署。允许的Origin含scheme和port，不含路径或尾斜线。HTTPS前端不得访问HTTP API。
本地 localhost 可以HTTP配置Origin，但浏览器对Secure Cookie的支持需实测；手机局域网HTTP
不能持久授权，须使用可信HTTPS开发环境。不降低Secure/SameSite设置。
`xlfg.com.cn` 别名与现有 `api.xifg.com.cn` 属于不同站点，阅读中心须改用 `xifg.com.cn` 主站。

| 方法 | 路径 | 说明 |
|---|---|---|
| POST | /api/library/auth/login | JSON password；200返回authenticated/expiresAt/csrfToken并设置Cookie |
| GET | /api/library/auth/session | 200返回有效会话摘要，401需重新授权 |
| POST | /api/library/auth/renew | Cookie + Origin + X-Reading-CSRF；200续期 |
| POST | /api/library/auth/logout | Cookie + Origin + X-Reading-CSRF；204撤销 |

不返回原始会话标识。数据库/存储不可用返回503，不暴露连接或秘密。过期或撤销会话行不会自动清理，
后续运维可经授权删除过期记录；它们不会授予权限。

## OwnerKey 迁移（本次只编写，未执行）

旧版20261013_reading_center_v01.sql保留不变。已创建四张旧表后，使用独立版本：
20261013_reading_ownerkey_sessions_v01.sql。脚本修改四表owner_id为owner_key，解除user外键，
保留书籍、进度和笔记的复合归属外键及原有唯一索引，新增reading_sessions；不修改user表。

执行必须另获授权，并先停用阅读写请求、核对表结构/数据、确认可恢复数据库和私有文件备份。
脚本要求显式设置@reading_backup_confirmed=1及非空@reading_backup_reference；没有确认或
任何一张阅读表有数据时SIGNAL停止。存在数据需单独审核映射，不能默认全部归给站主。
备份变量只是操作人员确认，脚本不会凭空验证备份文件。执行前先独立只读检查，避免进入DDL。

该脚本需要支持 `DELIMITER` 和复合 `CREATE PROCEDURE` 的执行器，推荐原生 MySQL 客户端
通过 `SOURCE` 加载。`DELIMITER` 是客户端命令，不能把脚本直接粘贴到只支持单语句的规划/SQL窗口。
若出现 `Multiple SQL statements are not supported`，应停止并换用兼容客户端；
过程创建失败后的 `CALL`、`DROP PROCEDURE` 报不存在属于连带错误，不能据此重复执行迁移。

只支持单语句且无法创建过程的执行器，可使用同版本的 `_preflight.sql` 和 `_manual.sql`：
先逐条执行只读检查，核对数据库、空表、字段、索引和外键；实际核验备份并停写后，
再逐条复制 manual 文件的编号 DDL，每条成功后才继续。不要整份提交，不执行 CALL/DROP PROCEDURE。
manual 版本没有自动前置检查，SELECT 的结果不会阻止后续 DDL，必须人工确认；
有数据、备份未确认、结构不符或中途失败都必须停止。它不修改旧 SQL，也不修改 user 表。

```sql
-- 仅在备份已实际核验且执行获授权后：
SET @reading_backup_confirmed = 1;
SET @reading_backup_reference = '<可恢复备份的实际引用>';
SOURCE D:/00-Project/AI/PersonWeb/database/20261013_reading_ownerkey_sessions_v01.sql;
```

MySQL DDL不可整体回滚，必须停写并逐项检查；命名锁只防止重复执行此迁移，不阻止其他写入。
脚本不会覆盖已执行旧SQL，部分执行/重复执行会拒绝并要求人工检查。升级代码依赖新表结构，
未迁移时保持阅读服务不可用；不要借此启用默认身份。应用启动不执行此迁移。

## 私有本地存储

默认目录位于 API `ContentRootPath/App_Data/reading-private`，不属于 `wwwroot`，也不映射静态路由。
`App_Data` 已被 Git 忽略。只返回书籍元数据与 API 地址，不返回存储 key、绝对路径或永久 URL。
配置为项目内其他目录、公共目录、构建目录、内容目录或其父目录会失败；项目外可使用绝对路径。
目录及文件不允许符号链接/junction；部署账户需要独占写权限，避免其他本机用户替换路径。
Linux 新目录以 0700 创建；Windows 和已有目录需要由部署人员设置仅服务账户可读写的 ACL。

生产建议配置持久卷中的绝对路径，例如 `/var/lib/personweb/reading-private`
（见 `backend/PersonalSite.Api/appsettings.Production.json.example`）。
部署替换应用目录时不要删除阅读存储；备份必须同时包含数据库和私有文件。
本期不接入 OSS；后续如需要，应使用私有 Bucket 和同样的授权 API，不能复用公共媒体 URL。

**任意电脑都能看**：在**线上站点**（如 `https://xifg.com.cn/library`）上传即可——文件写入服务器
`StorageRoot`，库里只记 `storage_key`。不要用本机 `localhost:5234` 上传再指望线上能打开
（库可共用，磁盘不共用）。已有本机文件可一次性拷到服务器同名 `storage_key`，或在线上重传。

上传流先写随机临时文件，校验通过后转为随机 `.epub` 文件，保存数据库；明确失败/取消会清理文件。
同一用户、同一 SHA-256 重复上传返回已有书籍。正常异常会清理；进程崩溃或磁盘权限异常仍可能
留下孤儿文件，日志中记录清理失败。数据库提交确认丢失时会重新查询；已提交的文件保留。
若无法确认数据库状态，则保留文件并记录运维核对日志，优先避免误删有效内容。
后续运维清理需核对数据库，不能按时间盲删有效书籍。

## EPUB strict-v2 安全范围

复用 .NET `ZipArchive`、`XmlReader`，CSS 使用 `AngleSharp.Css 0.18.0` 解析，不自行开发排版引擎。
这是一套保守的**验证并拒绝**策略，不是完整 EPUB 兼容实现或通用 HTML/CSS 净化器。
通过的书籍原字节不重写，SHA-256 + content_version 标识不可变内容，便于跨设备 CFI 恢复。

默认限制：上传 100 MiB、展开总量 100 MiB、单条目 10 MiB、2000 条目、单条目压缩比 100。
HTTP body 上限为 101 MiB，为 multipart 留出空间；反向代理也需配置相应限制。
文件大小上限提高不会放宽解压安全限制；展开后超过100 MiB的 EPUB 仍会被拒绝。
展开限制可在 `ReadingLibrary` 设置 `MaxExpandedBytes`、`MaxEntryBytes`、`MaxEntries`、
`MaxCompressionRatio`，只能在评估资源预算后调整。ZIP 从不解压到磁盘。

支持单 rendition、可重排 EPUB 2.0/3.0、规范 XHTML、NCX、基础非取资源 CSS、PNG/JPEG/GIF。
校验 mimetype、container、OPF、manifest/spine、资源存在性和元数据；拒绝路径穿越、重复路径、
符号链接、DTD/实体、XML处理指令、脚本/事件属性、外部资源和不明资源。
DRM、通用 SVG、嵌入字体、固定版式、HTML而非XHTML、额外未声明文件和复杂CSS不在当前范围。
允许 XHTML 中只含单个本地 PNG/JPEG/GIF 的 SVG 包装，并检查引用路径和图片签名；
不允许 SVG 脚本、事件、动画、foreignObject、外部引用或独立 SVG 资源。
兼容无行为的 data-* 属性、UTF-8 Content-Type 编码声明及 calibre:cover 标记；仍拒绝其他 http-equiv。
CSS 解析后只接受非取资源的白名单排版属性、@page 和仅包含 local(...) 本机字体源的 @font-face。
url(...)、外部/嵌入字体、@import、@media、其他函数、注释、转义及 !important 仍返回422。
浏览器仍使用禁用字体加载的 CSP，字体不可用时回退系统字体；不会下载字体资源。
原始 EPUB 字节不变，SHA-256 和 DOM/CFI 不重写；新导入记录 validation_profile=strict-v2，
旧记录保留原标记，无需数据库迁移。不支持的合法 EPUB 也可能被拒绝，不能将拒绝等同于恶意。

内容接口返回 EPUB ZIP attachment、`nosniff` 和禁用活动内容的 CSP；这些 HTTP header
不会自动保护 epub.js 解包后的 iframe。客户端仍须禁止 scripted content、外部网络资源，
检查引擎 sandbox，并将笔记当文本处理。不要把包内 XHTML 当公开静态页面提供。

## 已实现 API

全部要求有效阅读会话Cookie，响应不进入共享缓存。成功沿用 `{code:0,message,data}`。
错误使用实际 HTTP 状态码，业务校验包含稳定 `error` 字符串。

| 方法 | 路径 | 参数/结果 |
|---|---|---|
| POST | `/api/library/books` | multipart 字段 `file`；201，返回书籍元数据，重复上传复用已有 ID |
| GET | `/api/library/books` | `page=1&pageSize=24&q=...`；返回 total/items；pageSize 1..100，q 最长100 |
| GET | `/api/library/books/{id}` | 返回书籍信息、已存在进度摘要 |
| GET | `/api/library/books/{id}/content` | 受保护 EPUB 二进制；客户端可读取 ArrayBuffer；不支持 Range |
| GET | `/api/library/books/{id}/cover` | 受保护 PNG/JPEG/GIF；支持 EPUB 3 cover-image、EPUB 2 cover meta；无封面返回404 |

列表默认按进度 `last_read_at`、上传时间、ID倒序排序；没有进度时按上传时间排序。
列表用单次分页联表取得摘要，不对每本书额外查询。
封面按请求从私有 EPUB 中读取，不新增数据库字段或公共文件。提取前复用完整安全校验，
不会解压到磁盘；缺少封面时前端使用书名占位。前端只对进入可视区的书请求封面，并释放 Blob URL。
大批量或大型 EPUB 的封面读取仍会重复校验，可在后续有性能证据时补充私有缓存。

## 客户端阅读器（阶段二）

### MOBI 导入（复用 Calibre）

`POST /api/library/books` 接受 EPUB 或 MOBI，均限制100 MiB；原有会话、Origin、CSRF和归属校验保持不变。
MOBI 先检查 PalmDB 的 BOOKMOBI 标识，再调用已安装的 Calibre `ebook-convert` 转为 EPUB 2，
使用 `--no-svg-cover --no-default-epub-cover`，随后执行现有完整 EPUB 安全校验并导入。
只保存已验证的 EPUB，原始 MOBI和转换缓存均为临时文件；不新增表、转换服务或第二套阅读器。

Windows 自动检测 Program Files/Calibre2 和 PATH；其他系统检测 PATH 中的 ebook-convert。
也可通过服务端配置指定绝对路径，不能由上传请求指定命令或参数：

```json
{
  "ReadingLibrary": {
    "CalibreExecutable": "C:\\Program Files\\Calibre2\\ebook-convert.exe",
    "ConversionTimeoutSeconds": 120,
    "ConversionMemoryBytes": 1073741824
  }
}
```

部署机器必须安装 Calibre；缺失时 MOBI 返回503 converter_unavailable，EPUB不受影响。
每个后端实例同时只执行一项转换，忙时返回429 conversion_busy；默认120秒超时返回504。
以200ms间隔检查转换进程内存和私有工作目录总大小（最大300 MiB、8000个文件）；超限返回413，
取消、超时或超限时结束进程树并清理私有临时目录。此为应用级监控，不是操作系统级硬资源/网络沙箱；
部署时应让后端和转换工具以低权限账号运行，保持 Calibre 更新，并按需要通过宿主机策略限制网络和资源。
使用独立临时配置/缓存目录，禁用 Calibre Python 模板，不加载站主个人 Calibre 配置。
不绕过 DRM，不关闭 EPUB 校验；损坏、DRM或无法通过安全校验的转换结果仍会拒绝。

前端接受两种后缀，显示上传/转换状态，上传请求等待上限180秒；完成后刷新书架。
反向代理的请求超时也应覆盖转换时间（默认至少180秒），并沿用现有上传大小限制。
CFI、内容版本、阅读预设和跨设备进度始终关联持久保存的 EPUB，不在阅读时重复转换。
同一 MOBI 的转换输出可能包含不同的生成元数据；去重仍按实际 EPUB 字节哈希，不承诺对源 MOBI 去重。

`components/library/LibraryReader.client.vue` 只在客户端动态 import `epubjs@0.3.93`（BSD-2-Clause）。
使用已有会话请求下载 ArrayBuffer，再交给引擎处理，避免把私有内容 URL 作为普通静态资源。
保留 `credentials: include`、禁用缓存与写操作 CSRF；没有第二次登录，不改变全局 CORS。
新功能不要求 SQL 迁移；沿用现有 owner_key 和会话表。

iframe 明确关闭 allowScriptedContent/allowPopups；渲染前加入 CSP，拒绝脚本、远程请求、
字体、对象和表单，仅允许包内 Blob 图片/CSS与必要的行内排版。上传仍执行完整 strict-v2 校验。
章节卸载、路由切换、授权失效时释放监听器、引擎和 Blob；加载失败或30秒超时提供重试。
阅读主题和字号仅在本次打开期间生效；样式重排不更新阅读进度或偏好表。

### 默认 Calibre 阅读预设

默认预设为「暖纸·幼圆·舒适阅读」，参数依据用户导出的 `profiles.json` 固化在
`utils/library-reading-preset.ts`；通过 epub.js themes.register/select 注入每个正文 iframe。
预设按钮恢复默认字号与暖纸主题；手动字号、深色主题和翻页保留。重排前捕获 CFI，更新后重新定位；
样式切换不保存进度、不改原书。图片/表格几何和标题字号沿用原结构，图片段落不强制缩进。

| Calibre 参数 | 网页预设 |
|---|---|
| YouYuan / 幼圆 | 系统字体优先，sans-serif 回退；不下载字体 |
| 22px / 行高1.7 | p、li、blockquote 为22px / 37.4px，相关 span 继承 |
| 段后0.55em / 首行2em | 默认12.1px / 44px；强制覆盖正文书内 CSS |
| 最大正文760px | 桌面分页容器最大840px，减左右各40px后为760px |
| 左右40px / 上下28px | 桌面一致；手机左右20px，上下28px |
| 横竖屏均1栏 | spread:none + paginated |
| sepia-light | 暖纸浅色；背景#f4ecd8、文字#453b31为近似值，导出文件未提供精确值 |
| override_book_colors=always | 覆盖正文文字/背景颜色，保留图片本身 |
| 链接#866344 | 暖纸主题一致；深色使用高对比链接色 |
| h1/h2/h3 行高1.4 | 一致，保留其字号 |

Windows Chrome 验证应使用 CDP CSS.getPlatformFontsForNode 确认实际 YouYuan 字形，
不能仅凭 font-family 声明或 document.fonts.check 推断。缺字体设备使用回退字体。
epub.js 0.3.93 registerCss 在新章节的注入存在缺口，因此采用规则对象注册；更新时仅清空
本预设的生成样式表，避免重复规则累计。分页 gap 为左右边距的两倍，与引擎计算保持一致。

epub.js 原依赖范围包含已弃用且有安全公告的 XML 解析器。package.json 针对 epubjs 作用域
固定覆盖 `@xmldom/xmldom@0.9.12`、`lodash@4.18.1`，不全量升级原博客依赖。
浏览器阅读使用原生 DOMParser，引擎不在 SSR 执行；覆盖版本须持续做排版、链接与安全回归。
手机浏览器须使用同站点 HTTPS 才能使用 Secure Cookie；桌面localhost例外不能当作手机HTTP部署方案。

浏览器请求必须使用 credentials: 'include'、cache: 'no-store'，写入额外发送 X-Reading-CSRF。
读取内容接口返回 ArrayBuffer，由客户端交给动态加载的 epub.js，不直接插入 XHTML。

状态：401缺失/过期/撤销会话；403Origin或CSRF校验失败；404不存在或无归属；
413超上传限制；422无效/不支持/不安全 EPUB；503缺少安全配置；500内部存储/数据库异常。
模型绑定失败（缺失file、无效参数）由 ASP.NET 返回400。日志保留内部异常，响应不暴露路径/连接信息。

## 自动保存与跨设备进度（阶段三）

沿用 `reading_progress`，无需迁移。以下接口验证阅读 Cookie 会话与 OwnerKey；PUT 同时验证 Origin 和 X-Reading-CSRF。

- `GET /api/library/books/{id}/progress`：`data` 为进度或 null；包含 bookId、contentVersion、cfi、chapterHref、progression、revision、lastReadAt、updatedAt。
- `PUT /api/library/books/{id}/progress`：上述位置字段加 expectedRevision、sessionId。首次 expectedRevision=0；后续使用服务端确认 revision。sessionId 是本次保存的 UUID，失败重试保留同一 UUID 和请求内容。
- `GET /api/library/continue`：`data` 为 `{ book, progress }` 或 null。列表同时返回真实 progression/lastReadAt，按最近阅读排序；内容版本已过期的摘要不展示。

服务端核对内容版本、CFI 基本结构及百分比；通过 EF revision 并发条件和已有唯一索引拒绝迟到更新，返回409 progress_conflict。相同请求重试返回已提交响应，不再次增加 revision 或 last_read_at；允许主动回读，绝不取最大百分比。时间为服务端 UTC，精度与 MySQL DATETIME(6) 一致。

阅读器先并发取得书籍、文件和进度，再生成 epub.js 字符位置索引并恢复 CFI；初次定位、恢复失败回退和字号/主题/窗口重排不保存。只有有效翻页、目录跳转与书内链接产生待保存位置。保存防抖1秒且串行发送；页面隐藏、pagehide 和离开阅读器时尽力用 keepalive 提交。浏览器强制结束、断网不能保证最后请求送达，“已同步”只表示服务端已确认。

同步失败保留请求，可手动重试；409 后获取服务端进度并保留最新本地位置。用户明确选择“使用服务端位置”或“保留本设备位置”，后者重新取得 revision 后提交；若再次冲突仍需选择。内容版本不一致不恢复旧 CFI，服务端同时拒绝过期版本写入。

### AI 伴读与笔记（V0.1）

沿用 `reading_annotations`，无需新迁移。AI 解释仅面板会话内可见，不入库。

- `GET /api/library/annotations`：跨书笔记时间线（含书名/作者，最多 500 条；需阅读会话）。
- `GET /api/reading/book-notes`：Life `/life/books` 公开笔记时间线（站主 OwnerKey，不含 EPUB）。
- `GET /api/library/books/{id}/annotations`：当前书笔记列表（新到旧）。
- `POST /api/library/books/{id}/annotations`：创建笔记；字段含 contentVersion、cfiRange、chapterHref、quote、note、kind、color、requestId；同一书 requestId 幂等。
- `POST /api/library/ai/explain`：阅读 Cookie + CSRF；body 含 quote、question、history；服务端代理 Python `website-chat`；失败返回 `ai_unavailable`，不影响阅读/进度/笔记。

尚未实现：单条 PATCH/DELETE、高亮颜色管理、偏好同步、AI 解释持久化、向量检索。
阅读器恢复初始位置和重排时禁止误写进度，只有实际阅读更新 last_read_at。
V0.1仍以一次点击直达 `/library/read/{id}` 为主路径，详情不是必经页。

## 验证

```sh
dotnet test backend/PersonalSite.Api.Tests/PersonalSite.Api.Tests.csproj --filter FullyQualifiedName~ReadingCenter
dotnet test backend/PersonalSite.Api.Tests/PersonalSite.Api.Tests.csproj
dotnet build backend/PersonalSite.Api/PersonalSite.Api.csproj
```

测试覆盖真实本地 HTTP/MVC/Cookie会话、CSRF与Origin、撤销/续期和OwnerKey归属校验、私有文件读取、异常清理和恶意 EPUB。
默认测试数据库为 EF InMemory；真实 MySQL 验证显式启用，使用已有表与随机临时 OwnerKey，事务回滚，不执行建表/迁移。Windows 示例：

```powershell
$env:READING_MYSQL_VERIFY_CONFIG = (Resolve-Path 'backend/PersonalSite.Api/appsettings.Development.json').Path
dotnet test backend/PersonalSite.Api.Tests/PersonalSite.Api.Tests.csproj
Remove-Item Env:READING_MYSQL_VERIFY_CONFIG
```

涵盖 EF MySQL revision 条件更新，以及 Kestrel HTTP + Cookie + CSRF 的上传、GET/PUT、重试、冲突、continue 和列表摘要；事务回滚可能消耗临时自增 ID，不产生持久业务行。上线前还需在 MySQL 验证联表查询、
唯一键/外键；验证持久卷、ACL、反向代理大小限制。前端自动化可使用：

```sh
npm run test:run -- test/utils/library-progress.test.ts test/utils/library-access.test.ts test/utils/library-reader.test.ts test/utils/library-reading-preset.test.ts test/utils/admin-runtime-auth.test.ts
npm run build
```

浏览器验收应使用临时 EPUB，覆盖空书架、上传、搜索、封面直达、目录、正文锚点、字号、
主题、键盘、触控与失败提示；API 模拟下的浏览器验证不能替代真实 MySQL/HTTPS/实体手机联调。
