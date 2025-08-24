-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Récupère une entrée du journal d’audit par son identifiant
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_GetAuditLogById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        UserId,
        Action,
        TableName,
        RecordId,
        Details,
        [Timestamp]
    FROM dbo.AUDIT_LOG
    WHERE Id = @Id;
END
GO