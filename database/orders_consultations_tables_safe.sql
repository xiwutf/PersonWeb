-- ============================================
-- 订单和咨询系统数据库表结构（安全版本）
-- ============================================
-- 
-- 此文件包含安全的 ALTER TABLE 语句，使用存储过程检查列是否存在
-- 如果列已存在，不会报错

-- ============================================
-- 创建存储过程：安全添加列
-- ============================================
DELIMITER $$

DROP PROCEDURE IF EXISTS `AddColumnIfNotExists`$$

CREATE PROCEDURE `AddColumnIfNotExists`(
    IN tableName VARCHAR(128),
    IN columnName VARCHAR(128),
    IN columnDefinition TEXT
)
BEGIN
    DECLARE columnExists INT DEFAULT 0;
    
    SELECT COUNT(*) INTO columnExists
    FROM information_schema.COLUMNS
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

-- ============================================
-- 使用存储过程安全添加列
-- ============================================

-- 添加是否支持在线下单字段
CALL AddColumnIfNotExists('tool', 'enable_online_order', 'TINYINT(1) DEFAULT 0 COMMENT ''是否显示"立即下单"按钮'' AFTER `is_premium`');

-- 添加适合人群字段
CALL AddColumnIfNotExists('tool', 'fit_for', 'TEXT DEFAULT NULL COMMENT ''适合人群（文本或JSON）'' AFTER `requirements`');

-- 添加不适合情况字段
CALL AddColumnIfNotExists('tool', 'not_fit_for', 'TEXT DEFAULT NULL COMMENT ''不适合情况（文本或JSON）'' AFTER `fit_for`');

-- 添加交付类型字段
CALL AddColumnIfNotExists('tool', 'delivery_type', 'VARCHAR(50) DEFAULT NULL COMMENT ''交付类型（如：即时交付、定制开发等）'' AFTER `not_fit_for`');

-- 添加预计交付时间字段
CALL AddColumnIfNotExists('tool', 'estimated_delivery_time', 'VARCHAR(100) DEFAULT NULL COMMENT ''预计交付时间（如：1-3天、一周内等）'' AFTER `delivery_type`');

-- ============================================
-- 清理存储过程（可选）
-- ============================================
-- DROP PROCEDURE IF EXISTS `AddColumnIfNotExists`;

