-- ============================================
-- 删除已下线功能模块的数据表
-- 范围：情报中心 / 副业项目 / 资产管理 / 投资 / 定投 / 价格提醒
-- 保留：tool*、friend_links、user_behavior（前台或其它功能仍在用）
-- ============================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- 情报中心
DROP TABLE IF EXISTS `intelligence_task_log`;
DROP TABLE IF EXISTS `intelligence_analysis`;
DROP TABLE IF EXISTS `intelligence_content`;
DROP TABLE IF EXISTS `intelligence_daily_report`;
DROP TABLE IF EXISTS `intelligence_source`;

-- 副业项目
DROP TABLE IF EXISTS `side_project_attachment`;
DROP TABLE IF EXISTS `side_project_log`;
DROP TABLE IF EXISTS `side_project_milestone`;
DROP TABLE IF EXISTS `side_project_task`;
DROP TABLE IF EXISTS `side_project_requirement`;
DROP TABLE IF EXISTS `side_notification`;
DROP TABLE IF EXISTS `side_project`;

-- 资产 / 投资 / 定投 / 价格提醒
DROP TABLE IF EXISTS `dca_execution`;
DROP TABLE IF EXISTS `dca_plan`;
DROP TABLE IF EXISTS `investment_transaction`;
DROP TABLE IF EXISTS `investment`;
DROP TABLE IF EXISTS `price_alert`;
DROP TABLE IF EXISTS `asset`;

SET FOREIGN_KEY_CHECKS = 1;
