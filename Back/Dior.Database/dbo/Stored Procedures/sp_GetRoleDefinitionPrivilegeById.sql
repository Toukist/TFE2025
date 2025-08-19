
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Récupère une association rôle-privilege par son identifiant
-- =============================================
CREATE PROCEDURE dbo.sp_GetRoleDefinitionPrivilegeById
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        roleDefinitionId,
        privilegeId,
        createdAt,
        createdBy,
        lastEditAt,
        lastEditBy
    FROM dbo.ROLE_DEFINITION_PRIVILEGE
    WHERE id = @id;
END