# 完整网站内容数据插入说明

## 文件说明

`complete_content_data.sql` 是一个完整的网站内容数据插入脚本，包含：

1. **站点配置**（`site_config` 表）
   - 首页 Slogan（多个备选）
   - 身份标签
   - 个人简介（Hero 文案）
   - 关于我页面内容
   - AI 中心介绍

2. **文章数据**（`article` 表）- 5 篇
   - 为什么我要打造自己的数字资产平台？
   - AI 会取代开发者吗？我的真实思考
   - 我的个人网站从 0 到 1 的搭建过程
   - 未来 5 年我要打造哪些产品？（路线图）
   - 一个普通人的独立开发之路

3. **项目数据**（`projects` 表）- 5 个
   - 个人数字资产平台（本网站）
   - AI Service（Python 服务）
   - 访客分析系统（Analytics）
   - 开发者工具箱（Tools）
   - 实验室（Labs）

4. **更新日志**（`changelog` 表）- 4 条
   - 2025.11.29：访客分析与主题系统优化
   - 2025.11.28：AI Service 架构初始化
   - 2025.11.27：数据库与后台管理系统
   - 2025.11.26：访客记录系统上线

5. **未来规划**（`roadmap` 表）- 3 条
   - 短期规划（1 个月）
   - 中期规划（3-6 个月）
   - 长期规划（1 年）

## 执行前准备

### 1. 创建更新日志和未来规划表

首先执行 `changelog_roadmap_tables.sql` 创建表结构：

```bash
mysql -u your_username -p your_database_name < database/changelog_roadmap_tables.sql
```

### 2. 执行数据插入脚本

```bash
mysql -u your_username -p your_database_name < database/complete_content_data.sql
```

## 数据内容说明

### 站点配置（site_config）

配置项可以通过后端 API `/api/Config` 获取，前端可以这样使用：

```typescript
const config = await api.get('/Config')
const slogan = config.home_slogan
const tags = config.home_tags
const heroIntro = config.home_hero_intro
```

### 文章（article）

所有文章都已设置为"已发布"状态（`status = 1`），可以直接在前端显示。

### 项目（projects）

所有项目状态为 `Active`，包含完整的技术栈和内容描述。

### 更新日志（changelog）

更新日志按日期倒序排列，可以通过日期筛选显示。

### 未来规划（roadmap）

规划分为三个时间线：
- `short`：短期（1 个月）
- `medium`：中期（3-6 个月）
- `long`：长期（1 年）

## 前端使用建议

### 1. 首页内容

从 `site_config` 读取：
- `home_slogan`：显示在 Hero 区域
- `home_tags`：显示身份标签
- `home_hero_intro`：显示个人简介

### 2. 关于我页面

从 `site_config` 读取：
- `about_intro`：一句话介绍
- `about_detail`：详细介绍
- `about_skills`：技能栈
- `about_goals`：目标

### 3. 更新日志页面

从 `changelog` 表读取，按日期倒序显示。

### 4. 未来规划页面

从 `roadmap` 表读取，按时间线分组显示。

## 注意事项

1. **表结构**：确保先执行 `changelog_roadmap_tables.sql` 创建表
2. **字段大小写**：
   - `projects` 表字段名是大写（`Id`, `Title` 等）
   - `article` 表字段名是小写（`id`, `title` 等）
3. **唯一键冲突**：脚本使用 `ON DUPLICATE KEY UPDATE`，已存在的数据会被更新
4. **日期格式**：更新日志的日期使用 `DATE` 类型，格式为 `YYYY-MM-DD`

## 验证数据

执行脚本后，可以运行以下 SQL 查询验证：

```sql
-- 查看配置项
SELECT `config_key`, `config_value` FROM `site_config` WHERE `config_key` LIKE 'home_%' OR `config_key` LIKE 'about_%';

-- 查看文章
SELECT `id`, `title`, `status` FROM `article` WHERE `status` = 1 ORDER BY `publish_time` DESC;

-- 查看项目
SELECT `Id`, `Title`, `Status` FROM `projects` WHERE `Status` = 'Active';

-- 查看更新日志
SELECT `version`, `date`, `title` FROM `changelog` WHERE `status` = 1 ORDER BY `date` DESC;

-- 查看未来规划
SELECT `title`, `timeline`, `status`, `progress` FROM `roadmap` ORDER BY `timeline`, `priority` DESC;
```

## 后续维护

- **配置更新**：通过后台管理系统或直接更新 `site_config` 表
- **文章管理**：通过文章管理后台添加/编辑文章
- **项目更新**：通过项目管理后台更新项目状态和内容
- **更新日志**：定期添加新的更新记录
- **未来规划**：根据实际情况更新规划状态和进度

