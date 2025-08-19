
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Retourne la liste complète des accès utilisateurs
-- =============================================
CREATE PROCEDURE dbo.sp_GetUserAccessList
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        userId,
        accessId,
        createdAt,
        createdBy,
        lastEditAt,
        lastEditBy
    FROM dbo.USER_ACCESS
    ORDER BY id;
END