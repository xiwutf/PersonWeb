-- 文档知识管家 Agent 数据库表结构

-- 文档表
CREATE TABLE IF NOT EXISTS `Document` (
    `id` BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `title` VARCHAR(500) NOT NULL COMMENT '文档标题',
    `file_name` VARCHAR(500) NOT NULL COMMENT '原始文件名',
    `file_path` VARCHAR(1000) NOT NULL COMMENT '文件存储路径',
    `file_type` VARCHAR(50) NOT NULL COMMENT '文件类型 (pdf, docx, txt, etc.)',
    `file_size` BIGINT NOT NULL DEFAULT 0 COMMENT '文件大小 (bytes)',
    `status` VARCHAR(50) NOT NULL DEFAULT 'pending' COMMENT '处理状态 (pending, processing, completed, failed)',
    `summary` TEXT COMMENT '文档摘要',
    `knowledge_structure` JSON COMMENT '知识结构 (JSON格式)',
    `total_chunks` INT NOT NULL DEFAULT 0 COMMENT '分段总数',
    `user_id` VARCHAR(100) COMMENT '用户 ID',
    `error_message` TEXT COMMENT '错误信息（处理失败时）',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `processed_at` DATETIME COMMENT '处理完成时间',
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='文档表';

-- 文档分段表
CREATE TABLE IF NOT EXISTS `DocumentChunk` (
    `id` BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `document_id` BIGINT NOT NULL COMMENT '文档 ID',
    `chunk_index` INT NOT NULL COMMENT '分段索引（从0开始）',
    `content` TEXT NOT NULL COMMENT '分段内容',
    `summary` TEXT COMMENT '分段摘要',
    `metadata` JSON COMMENT '元数据 (JSON格式，包含页码、位置等信息)',
    `vector_id` VARCHAR(200) COMMENT '向量数据库中的 ID',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    INDEX `idx_document_id` (`document_id`),
    INDEX `idx_chunk_index` (`document_id`, `chunk_index`),
    FOREIGN KEY (`document_id`) REFERENCES `Document`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='文档分段表';

-- 问答历史表（可选，用于记录问答历史）
CREATE TABLE IF NOT EXISTS `DocumentQuery` (
    `id` BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `document_id` BIGINT COMMENT '文档 ID（如果是对单个文档的问答）',
    `user_id` VARCHAR(100) COMMENT '用户 ID',
    `query` TEXT NOT NULL COMMENT '用户问题',
    `answer` TEXT NOT NULL COMMENT 'AI 生成的答案',
    `relevant_chunks` JSON COMMENT '相关文档片段 ID 列表',
    `confidence` DECIMAL(5, 4) COMMENT '置信度 (0-1)',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    INDEX `idx_document_id` (`document_id`),
    INDEX `idx_user_id` (`user_id`),
    INDEX `idx_created_at` (`created_at`),
    FOREIGN KEY (`document_id`) REFERENCES `Document`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='问答历史表';

