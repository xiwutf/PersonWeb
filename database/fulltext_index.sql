-- 全文搜索索引创建脚本
-- 为文章、知识库、项目表添加全文索引

-- 1. 为 article 表添加全文索引
-- 索引字段：title, summary, content_md
ALTER TABLE `article` 
ADD FULLTEXT INDEX `ft_article_search` (`title`, `summary`, `content_md`);

-- 2. 为 knowledge_base 表添加全文索引
-- 索引字段：title, content
ALTER TABLE `knowledge_base` 
ADD FULLTEXT INDEX `ft_knowledge_search` (`title`, `content`);

-- 3. 为 projects 表添加全文索引
-- 索引字段：title, description, content
ALTER TABLE `projects` 
ADD FULLTEXT INDEX `ft_project_search` (`title`, `description`, `content`);

-- 注意：
-- - MySQL 5.6+ 支持 InnoDB 全文索引
-- - 全文索引最小词长度默认为 4（可通过 ft_min_word_len 配置）
-- - 中文搜索需要配置 ngram 解析器（MySQL 5.7.6+）

