
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Récupère une entrée du journal d’audit par son identifiant
-- =============================================
CREATE PROCEDURE dbo.sp_GetAuditLogById
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        userId,
        action,
        tableName,
        recordId,
        details,
        [timestamp]
    FROM dbo.AUDIT_LOG
    WHERE id = @id;
END