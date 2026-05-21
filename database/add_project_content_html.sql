-- 为 projects 表增加发布态 HTML 正文（AI 产出直接入库，前台优先读取）
-- 执行前请确认表名为 projects（与 AppDbContext 配置一致）

ALTER TABLE `projects`
ADD COLUMN `ContentHtml` LONGTEXT NULL COMMENT '项目正文 HTML（发布态）' AFTER `Content`;
