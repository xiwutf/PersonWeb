-- Projects 表创建脚本
-- 数据库: personal_site
-- 表名: Projects (Entity Framework 默认使用复数形式)

-- 如果表已存在，先删除（谨慎使用）
-- DROP TABLE IF EXISTS `Projects`;

-- 创建 Projects 表
CREATE TABLE IF NOT EXISTS `Projects` (
    `Id` CHAR(36) NOT NULL COMMENT '项目ID (GUID)',
    `Title` VARCHAR(100) NOT NULL COMMENT '项目标题',
    `Description` VARCHAR(500) DEFAULT '' COMMENT '项目描述',
    `CoverUrl` VARCHAR(200) DEFAULT NULL COMMENT '封面图片URL',
    `DemoUrl` VARCHAR(200) DEFAULT NULL COMMENT '演示地址',
    `GithubUrl` VARCHAR(200) DEFAULT NULL COMMENT 'GitHub仓库地址',
    `Status` VARCHAR(50) DEFAULT 'Active' COMMENT '项目状态: Active, Completed, Archived',
    `TechStack` TEXT DEFAULT NULL COMMENT '技术栈 (JSON数组或逗号分隔)',
    `Content` LONGTEXT DEFAULT NULL COMMENT '项目内容 (Markdown格式)',
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`Id`),
    INDEX `idx_created_at` (`CreatedAt`),
    INDEX `idx_status` (`Status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='项目表';

-- 插入示例数据（可选）
-- INSERT INTO `Projects` (`Id`, `Title`, `Description`, `Status`, `CreatedAt`, `UpdatedAt`) VALUES
-- (UUID(), '示例项目', '这是一个示例项目', 'Active', NOW(), NOW());

