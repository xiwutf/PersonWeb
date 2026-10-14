-- READ ONLY. Single-statement consoles: execute each SELECT separately.
-- Review results before using the manual migration. These queries do not verify backups.
SELECT DATABASE();

SELECT 'reading_books' AS table_name, COUNT(*) AS row_count FROM reading_books
UNION ALL SELECT 'reading_progress', COUNT(*) FROM reading_progress
UNION ALL SELECT 'reading_annotations', COUNT(*) FROM reading_annotations
UNION ALL SELECT 'reading_preferences', COUNT(*) FROM reading_preferences;

SELECT TABLE_NAME, ENGINE FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
('reading_books','reading_progress','reading_annotations','reading_preferences','reading_sessions');

SELECT TABLE_NAME,COLUMN_NAME,COLUMN_TYPE FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
('reading_books','reading_progress','reading_annotations','reading_preferences')
AND COLUMN_NAME IN ('owner_id','owner_key');

SELECT TABLE_NAME,CONSTRAINT_NAME,REFERENCED_TABLE_NAME,DELETE_RULE
FROM information_schema.REFERENTIAL_CONSTRAINTS
WHERE CONSTRAINT_SCHEMA=DATABASE() AND TABLE_NAME IN
('reading_books','reading_progress','reading_annotations','reading_preferences');

SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME
FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
('reading_books','reading_progress','reading_annotations','reading_preferences')
ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX;
