-- ============================================
-- 技能表结构更新脚本
-- 为现有的 skill 表添加 current_rating 和 target_rating 字段
-- ============================================

-- 检查并添加 current_rating 字段（如果不存在）
SET @dbname = DATABASE();
SET @tablename = 'skill';
SET @columnname = 'current_rating';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (COLUMN_NAME = @columnname)
  ) > 0,
  'SELECT 1',
  CONCAT('ALTER TABLE ', @tablename, ' ADD COLUMN `current_rating` DECIMAL(3,1) DEFAULT 0.0 COMMENT ''当前评分（0-10）'' AFTER `icon`')
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

-- 检查并添加 target_rating 字段（如果不存在）
SET @columnname = 'target_rating';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (COLUMN_NAME = @columnname)
  ) > 0,
  'SELECT 1',
  CONCAT('ALTER TABLE ', @tablename, ' ADD COLUMN `target_rating` DECIMAL(3,1) DEFAULT NULL COMMENT ''目标评分'' AFTER `current_rating`')
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

-- 移除外键约束（如果存在）
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (CONSTRAINT_NAME LIKE '%category%')
      AND (REFERENCED_TABLE_NAME IS NOT NULL)
  ) > 0,
  (SELECT CONCAT('ALTER TABLE ', @tablename, ' DROP FOREIGN KEY `', CONSTRAINT_NAME, '`')
   FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
   WHERE
     (TABLE_SCHEMA = @dbname)
     AND (TABLE_NAME = @tablename)
     AND (CONSTRAINT_NAME LIKE '%category%')
     AND (REFERENCED_TABLE_NAME IS NOT NULL)
   LIMIT 1),
  'SELECT 1'
));
PREPARE dropFK FROM @preparedStatement;
EXECUTE dropFK;
DEALLOCATE PREPARE dropFK;

