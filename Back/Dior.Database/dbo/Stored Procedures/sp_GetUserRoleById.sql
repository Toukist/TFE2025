
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Récupère une association utilisateur/rôle par son identifiant
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetUserRoleById]
    @ID BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ID,
        RoleDefinitionID,  -- tel que défini dans la table
        UserID,
        LastEditBy,
        LastEditAt
    FROM dbo.USER_ROLE
    WHERE ID = @ID;
END