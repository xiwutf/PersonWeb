-- 为 projects 表添加缺失的 AI 字段
-- 此脚本会检查列是否存在，只添加缺失的列
-- 注意：MySQL 8.0.19+ 支持 IF NOT EXISTS，旧版本需要手动检查

-- 方法 1: 使用存储过程检查并添加（适用于所有 MySQL 版本）
DELIMITER $$

CREATE PROCEDURE IF NOT EXISTS AddColumnIfNotExists(
    IN tableName VARCHAR(64),
    IN columnName VARCHAR(64),
    IN columnDefinition TEXT
)
BEGIN
    DECLARE columnExists INT DEFAULT 0;
    
    SELECT COUNT(*) INTO columnExists
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = tableName
      AND COLUMN_NAME = columnName;
    
    IF columnExists = 0 THEN
        SET @sql = CONCAT('ALTER TABLE `', tableName, '` ADD COLUMN `', columnName, '` ', columnDefinition);
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$

DELIMITER ;

-- 调用存储过程添加缺失的列
CALL AddColumnIfNotExists('projects', 'ai_title', 'VARCHAR(200) NULL COMMENT ''AI 生成的标题''');
CALL AddColumnIfNotExists('projects', 'ai_highlights', 'TEXT NULL COMMENT ''AI 生成的亮点''');
CALL AddColumnIfNotExists('projects', 'ai_description', 'LONGTEXT NULL COMMENT ''AI 生成的详情描述''');
CALL AddColumnIfNotExists('projects', 'ai_scenarios', 'TEXT NULL COMMENT ''AI 生成的使用场景''');
CALL AddColumnIfNotExists('projects', 'ai_target_users', 'VARCHAR(500) NULL COMMENT ''AI 生成的目标用户''');
CALL AddColumnIfNotExists('projects', 'ai_short_card_text', 'VARCHAR(200) NULL COMMENT ''AI 生成的卡片短文本''');
CALL AddColumnIfNotExists('projects', 'view_count', 'INT DEFAULT 0 COMMENT ''访问量统计''');

-- 删除临时存储过程
DROP PROCEDURE IF EXISTS AddColumnIfNotExists;

-- 方法 2: 如果 MySQL 版本 >= 8.0.19，可以直接使用 IF NOT EXISTS（但根据错误信息，可能不支持）
-- ALTER TABLE `projects` 
-- ADD COLUMN IF NOT EXISTS `ai_title` VARCHAR(200) NULL COMMENT 'AI 生成的标题',
-- ADD COLUMN IF NOT EXISTS `ai_highlights` TEXT NULL COMMENT 'AI 生成的亮点',
-- ADD COLUMN IF NOT EXISTS `ai_description` LONGTEXT NULL COMMENT 'AI 生成的详情描述',
-- ADD COLUMN IF NOT EXISTS `ai_scenarios` TEXT NULL COMMENT 'AI 生成的使用场景',
-- ADD COLUMN IF NOT EXISTS `ai_target_users` VARCHAR(500) NULL COMMENT 'AI 生成的目标用户',
-- ADD COLUMN IF NOT EXISTS `ai_short_card_text` VARCHAR(200) NULL COMMENT 'AI 生成的卡片短文本',
-- ADD COLUMN IF NOT EXISTS `view_count` INT DEFAULT 0 COMMENT '访问量统计';

