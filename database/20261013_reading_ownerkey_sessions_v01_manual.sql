-- MANUAL single-statement executor variant of 20261013_reading_ownerkey_sessions_v01.sql.
-- No procedure, DELIMITER, session variables, or automatic precondition checks.
-- DO NOT EXECUTE THIS FILE AS A BATCH. Copy ONE numbered SQL statement each time.
-- Before ANY DDL: confirm recoverable DB/private-file backup, stop reading writes,
-- review the companion preflight results, require all four tables EMPTY,
-- exactly four legacy owner_id columns/four expected foreign keys and NO reading_sessions.
-- If data exists, backup is unconfirmed, or any check differs: STOP.
-- This variant relies on manual review; SELECT results do not automatically stop later DDL.
-- If a step fails: STOP and inspect. DDL is not transactional; never rerun all steps blindly.
-- Do not modify or delete the blog user table. No database changes were executed by creating this file.

-- STEP 1: execute only this statement, then check for success.
ALTER TABLE reading_progress DROP FOREIGN KEY fk_reading_progress_book;

-- STEP 2: execute only this statement, then check for success.
ALTER TABLE reading_annotations DROP FOREIGN KEY fk_reading_annotation_book;

-- STEP 3: execute only this statement, then check for success.
ALTER TABLE reading_books DROP FOREIGN KEY fk_reading_book_owner;

-- STEP 4: execute only this statement, then check for success.
ALTER TABLE reading_preferences DROP FOREIGN KEY fk_reading_preferences_owner;

-- STEP 5: execute only this statement, then check for success.
ALTER TABLE reading_books CHANGE COLUMN owner_id owner_key CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    RENAME INDEX uk_reading_book_owner_id TO uk_reading_book_owner_key;

-- STEP 6: execute only this statement, then check for success.
ALTER TABLE reading_progress CHANGE COLUMN owner_id owner_key CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL;

-- STEP 7: execute only this statement, then check for success.
ALTER TABLE reading_annotations CHANGE COLUMN owner_id owner_key CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL;

-- STEP 8: execute only this statement, then check for success.
ALTER TABLE reading_preferences CHANGE COLUMN owner_id owner_key CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL;

-- STEP 9: execute only this statement, then check for success.
ALTER TABLE reading_progress ADD CONSTRAINT fk_reading_progress_book FOREIGN KEY (owner_key,book_id)
    REFERENCES reading_books(owner_key,id) ON DELETE CASCADE;

-- STEP 10: execute only this statement, then check for success.
ALTER TABLE reading_annotations ADD CONSTRAINT fk_reading_annotation_book FOREIGN KEY (owner_key,book_id)
    REFERENCES reading_books(owner_key,id) ON DELETE CASCADE;

-- STEP 11: execute only this statement, then check for success.
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
