
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Récupère une compétence d’accès par son identifiant
-- =============================================
CREATE PROCEDURE dbo.[SP_GetAccessCompetency]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        name,
        parentId,
        isActive,
        createdAt,
        createdBy,
        lastEditAt,
        lastEditBy
    FROM dbo.ACCESS_COMPETENCY
    WHERE id = @id;
END