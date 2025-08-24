
-- ================================
-- AUDIT LOG
-- ================================
CREATE   PROCEDURE dbo.sp_GetAuditLogById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, UserId, Action, TableName, RecordId, Details, Timestamp
    FROM dbo.Audit_Log
    WHERE Id = @Id;
END