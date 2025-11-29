-- ============================================
-- 移除工具合集相关表的外键约束
-- 根据数据库设计原则，不使用外键约束以便后期维护
-- ============================================

-- 移除 tool_collection_item 表的外键约束
SET @dbname = DATABASE();

-- 查找并移除 collection_id 的外键约束
SET @tablename = 'tool_collection_item';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (CONSTRAINT_NAME LIKE '%collection%')
      AND (REFERENCED_TABLE_NAME IS NOT NULL)
  ) > 0,
  (SELECT CONCAT('ALTER TABLE ', @tablename, ' DROP FOREIGN KEY `', CONSTRAINT_NAME, '`')
   FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
   WHERE
     (TABLE_SCHEMA = @dbname)
     AND (TABLE_NAME = @tablename)
     AND (CONSTRAINT_NAME LIKE '%collection%')
     AND (REFERENCED_TABLE_NAME IS NOT NULL)
   LIMIT 1),
  'SELECT 1'
));
PREPARE dropFK FROM @preparedStatement;
EXECUTE dropFK;
DEALLOCATE PREPARE dropFK;

-- 查找并移除 tool_id 的外键约束
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (CONSTRAINT_NAME LIKE '%tool%')
      AND (REFERENCED_TABLE_NAME IS NOT NULL)
  ) > 0,
  (SELECT CONCAT('ALTER TABLE ', @tablename, ' DROP FOREIGN KEY `', CONSTRAINT_NAME, '`')
   FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
   WHERE
     (TABLE_SCHEMA = @dbname)
     AND (TABLE_NAME = @tablename)
     AND (CONSTRAINT_NAME LIKE '%tool%')
     AND (REFERENCED_TABLE_NAME IS NOT NULL)
   LIMIT 1),
  'SELECT 1'
));
PREPARE dropFK FROM @preparedStatement;
EXECUTE dropFK;
DEALLOCATE PREPARE dropFK;

