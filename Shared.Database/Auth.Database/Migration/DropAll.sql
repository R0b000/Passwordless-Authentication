-- ======================================================
-- 1. DROP ALL FOREIGN KEY CONSTRAINTS
-- ======================================================
DECLARE @SqlFk NVARCHAR(MAX) = N'';

SELECT @SqlFk += N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) 
    + N'.' + QUOTENAME(OBJECT_NAME(parent_object_id)) 
    + N' DROP CONSTRAINT ' + QUOTENAME(name) + N';' + CHAR(13)
FROM sys.foreign_keys;

EXEC sp_executesql @SqlFk;

-- ======================================================
-- 2. DROP ALL USER TABLES
-- ======================================================
DECLARE @SqlTables NVARCHAR(MAX) = N'';

SELECT @SqlTables += N'DROP TABLE ' + QUOTENAME(TABLE_SCHEMA) 
    + N'.' + QUOTENAME(TABLE_NAME) + N';' + CHAR(13)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';

EXEC sp_executesql @SqlTables;

-- ======================================================
-- 3. DROP ALL STORED PROCEDURES
-- ======================================================
DECLARE @SqlProcs NVARCHAR(MAX) = N'';

SELECT @SqlProcs += N'DROP PROCEDURE ' + QUOTENAME(ROUTINE_SCHEMA) 
    + N'.' + QUOTENAME(ROUTINE_NAME) + N';' + CHAR(13)
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE' 
  AND ROUTINE_NAME NOT LIKE 'sp_%'; -- Exclude system stored procedures

EXEC sp_executesql @SqlProcs;