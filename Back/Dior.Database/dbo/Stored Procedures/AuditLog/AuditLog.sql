IF OBJECT_ID('sp_AddAuditLog', 'P') IS NOT NULL DROP PROCEDURE sp_AddAuditLog
GO
CREATE PROCEDURE sp_AddAuditLog
    @userId INT,
    @action NVARCHAR(100),
    @tableName NVARCHAR(100),
    @recordId INT,
    @details NVARCHAR(MAX) = NULL,
    @timestamp DATETIME
AS
BEGIN
    INSERT INTO AUDIT_LOG (userId, action, tableName, recordId, details, timestamp)
    VALUES (@userId, @action, @tableName, @recordId, @details, @timestamp)
END
GO

IF OBJECT_ID('sp_GetAuditLogById', 'P') IS NOT NULL DROP PROCEDURE sp_GetAuditLogById
GO
CREATE PROCEDURE sp_GetAuditLogById
    @id INT
AS
BEGIN
    SELECT * FROM AUDIT_LOG WHERE id = @id
END
GO

IF OBJECT_ID('sp_DeleteAuditLog', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteAuditLog
GO
CREATE PROCEDURE sp_DeleteAuditLog
    @id INT
AS
BEGIN
    DELETE FROM AUDIT_LOG WHERE id = @id
END
GO
