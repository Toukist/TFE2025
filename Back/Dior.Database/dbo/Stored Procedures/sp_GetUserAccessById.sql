
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Récupère un accès utilisateur par son identifiant
-- =============================================
CREATE PROCEDURE dbo.sp_GetUserAccessById
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        userId,
        accessId,
        createdAt,
        createdBy,
        lastEditAt,
        lastEditBy
    FROM dbo.USER_ACCESS
    WHERE id = @id;
END