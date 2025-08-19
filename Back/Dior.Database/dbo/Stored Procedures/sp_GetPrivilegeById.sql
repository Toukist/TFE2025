
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Récupère un privilège par son identifiant
-- =============================================
CREATE PROCEDURE dbo.sp_GetPrivilegeById
    @ID BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ID,
        [Name],
        [Description],
        IsConfigurableRead,
        IsConfigurableDelete,
        IsConfigurableAdd,
        IsConfigurableModify,
        IsConfigurableStatus,
        IsConfigurableExecution,
        LastEditBy,
        LastEditAt
    FROM dbo.PRIVILEGE
    WHERE ID = @ID;
END