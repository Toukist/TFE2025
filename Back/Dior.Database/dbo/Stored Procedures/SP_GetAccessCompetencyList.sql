
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Retourne la liste complète des compétences d’accès
-- =============================================
CREATE PROCEDURE dbo.[SP_GetAccessCompetencyList]
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
    ORDER BY id;
END