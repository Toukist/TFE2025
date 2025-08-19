-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Retourne la liste complète des utilisateurs
-- =============================================
CREATE PROCEDURE dbo.sp_GetUserList
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ID,
        IsActive,                       -- FIX: Changed from IsActivate to IsActive
        Username,                       -- FIX: Changed from Name to Username
        LastName,
        FirstName,
        Email,
        Phone,
        LastEditBy,
        LastEditAt
    FROM dbo.[USER]
    ORDER BY ID;
END -- FIX: replaced ENDEND