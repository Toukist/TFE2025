-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Récupère une association rôle-privilege par son identifiant
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_GetRoleDefinitionPrivilegeById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        RoleDefinitionId,
        PrivilegeId,
        CreatedAt,
        CreatedBy,
        LastEditAt,
        LastEditBy
    FROM dbo.ROLE_DEFINITION_PRIVILEGE
    WHERE Id = @Id;
END
GO