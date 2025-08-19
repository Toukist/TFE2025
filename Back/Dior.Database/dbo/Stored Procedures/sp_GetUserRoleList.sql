
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Retourne la liste complète des associations utilisateur/rôle
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetUserRoleList]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ID,
        RoleDefinitionID,
        UserID,
        LastEditBy,
        LastEditAt
    FROM dbo.USER_ROLE
    ORDER BY ID;
END