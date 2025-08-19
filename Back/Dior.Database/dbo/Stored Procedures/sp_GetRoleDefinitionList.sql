
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Retourne la liste complète des définitions de rôle
-- =============================================
CREATE PROCEDURE dbo.sp_GetRoleDefinitionList
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        name,
        description,
        parentRoleId,
        isActive,
        createdAt,
        createdBy,
        lastEditAt,
        lastEditBy
    FROM dbo.ROLE_DEFINITION
    ORDER BY id;
END