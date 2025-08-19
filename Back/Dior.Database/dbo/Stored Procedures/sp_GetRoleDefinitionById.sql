
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Récupère une définition de rôle par son identifiant
-- =============================================
CREATE PROCEDURE dbo.sp_GetRoleDefinitionById
    @id INT
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
    WHERE id = @id;
END