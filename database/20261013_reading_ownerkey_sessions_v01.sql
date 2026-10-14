-- Reading center v0.1: explicit owner key + persistent sessions.
-- DO NOT run automatically. Stop reading writes and obtain migration authorization first.
-- Old 20261013_reading_center_v01.sql remains unchanged.
-- Before SOURCE, operator must confirm a recoverable database AND private-file backup:
-- SET @reading_backup_confirmed = 1;
-- SET @reading_backup_reference = '<verified backup location/reference>';
-- Select the existing database. This script deliberately refuses any existing reading data.
-- MySQL DDL is not transactional; a failure requires inspection, never blind reruns.
-- Executor: use the native mysql client (SOURCE), which understands DELIMITER.
-- Do not paste into single-statement/planning consoles that reject compound CREATE PROCEDURE.
-- If CREATE PROCEDURE fails, stop: do not continue with CALL or DROP PROCEDURE.

DELIMITER $$
CREATE PROCEDURE migrate_reading_ownerkey_20261013()
BEGIN
    DECLARE lock_taken INT DEFAULT 0;
    DECLARE row_total BIGINT DEFAULT 0;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        IF lock_taken = 1 THEN DO RELEASE_LOCK('personweb-reading-ownerkey-20261013'); END IF;
        RESIGNAL;
    END;
    IF DATABASE() IS NULL THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Select existing PersonWeb database first'; END IF;
    IF COALESCE(@reading_backup_confirmed, 0) <> 1 OR COALESCE(CHAR_LENGTH(@reading_backup_reference), 0) = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'STOP: verified backup not confirmed';
    END IF;
    SELECT GET_LOCK('personweb-reading-ownerkey-20261013', 0) INTO lock_taken;
    IF lock_taken <> 1 THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Another reading migration is running'; END IF;
    IF (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
        ('reading_books','reading_progress','reading_annotations','reading_preferences') AND ENGINE='InnoDB') <> 4 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'STOP: four legacy InnoDB tables required';
    END IF;
    IF (SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
        ('reading_books','reading_progress','reading_annotations','reading_preferences') AND COLUMN_NAME='owner_id' AND COLUMN_TYPE='bigint') <> 4
        OR (SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
        ('reading_books','reading_progress','reading_annotations','reading_preferences') AND COLUMN_NAME='owner_key') <> 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'STOP: unexpected or partially migrated owner columns';
    END IF;
    IF (SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='reading_sessions') <> 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'STOP: session table already exists; inspect migration state';
    END IF;
    IF (SELECT COUNT(*) FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND
        ((TABLE_NAME='reading_books' AND CONSTRAINT_NAME='fk_reading_book_owner' AND REFERENCED_TABLE_NAME='user')
        OR (TABLE_NAME='reading_preferences' AND CONSTRAINT_NAME='fk_reading_preferences_owner' AND REFERENCED_TABLE_NAME='user')
        OR (TABLE_NAME='reading_progress' AND CONSTRAINT_NAME='fk_reading_progress_book' AND REFERENCED_TABLE_NAME='reading_books')
        OR (TABLE_NAME='reading_annotations' AND CONSTRAINT_NAME='fk_reading_annotation_book' AND REFERENCED_TABLE_NAME='reading_books'))) <> 4 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'STOP: expected legacy ownership constraints missing';
    END IF;
    SELECT (SELECT COUNT(*) FROM reading_books) + (SELECT COUNT(*) FROM reading_progress)
        + (SELECT COUNT(*) FROM reading_annotations) + (SELECT COUNT(*) FROM reading_preferences) INTO row_total;
    IF row_total <> 0 THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'STOP: reading data exists; explicit mapping/review required'; END IF;

    ALTER TABLE reading_progress DROP FOREIGN KEY fk_reading_progress_book;
    ALTER TABLE reading_annotations DROP FOREIGN KEY fk_reading_annotation_book;
    ALTER TABLE reading_books DROP FOREIGN KEY fk_reading_book_owner;
    ALTER TABLE reading_preferences DROP FOREIGN KEY fk_reading_preferences_owner;
    ALTER TABLE reading_books CHANGE COLUMN owner_id owner_key CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        RENAME INDEX uk_reading_book_owner_id TO uk_reading_book_owner_key;
    ALTER TABLE reading_progress CHANGE COLUMN owner_id owner_key CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL;
    ALTER TABLE reading_annotations CHANGE COLUMN owner_id owner_key CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL;
    ALTER TABLE reading_preferences CHANGE COLUMN owner_id owner_key CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL;
    ALTER TABLE reading_progress ADD CONSTRAINT fk_reading_progress_book FOREIGN KEY (owner_key,book_id)
        REFERENCES reading_books(owner_key,id) ON DELETE CASCADE;
    ALTER TABLE reading_annotations ADD CONSTRAINT fk_reading_annotation_book FOREIGN KEY (owner_key,book_id)
        REFERENCES reading_books(owner_key,id) ON DELETE CASCADE;
    CREATE TABLE reading_sessions (
        token_hash VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        owner_key CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        credential_version VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
        created_at DATETIME(6) NOT NULL,
        expires_at DATETIME(6) NOT NULL,
        absolute_expires_at DATETIME(6) NOT NULL,
        revoked_at DATETIME(6) NULL,
        PRIMARY KEY (token_hash), KEY idx_reading_session_expiry (expires_at)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
    DO RELEASE_LOCK('personweb-reading-ownerkey-20261013');
END$$
DELIMITER ;
CALL migrate_reading_ownerkey_20261013();
DROP PROCEDURE migrate_reading_ownerkey_20261013;
