
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Retourne la liste complète des accès
-- =============================================
CREATE PROCEDURE dbo.sp_GetAccessList
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        badgePhysicalNumber,
        isActive,
        createdAt,
        createdBy
    FROM dbo.ACCESS
    ORDER BY id;
END