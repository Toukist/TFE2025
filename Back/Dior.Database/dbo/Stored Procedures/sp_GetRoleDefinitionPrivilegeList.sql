
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Retourne la liste complète des associations rôle/privilege
-- =============================================
CREATE PROCEDURE dbo.sp_GetRoleDefinitionPrivilegeList
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
    ORDER BY id;
END