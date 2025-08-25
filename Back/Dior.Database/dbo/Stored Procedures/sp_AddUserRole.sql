/* =======================
   6) ROLE LINK HELPERS (optional)
   ======================= */
<<<<<<< Updated upstream
/* Ajout d'un lien User-Role si non existant */
CREATE OR ALTER PROCEDURE dbo.sp_AddUserRole
=======
CREATE OR ALTER   PROCEDURE dbo.sp_AddUserRole
>>>>>>> Stashed changes
  @UserId BIGINT,
  @RoleDefinitionId BIGINT
AS
BEGIN
  SET NOCOUNT ON;
<<<<<<< Updated upstream

  IF NOT EXISTS (
      SELECT 1
      FROM dbo.[User_Role]
      WHERE UserId = @UserId
        AND RoleDefinitionId = @RoleDefinitionId
  )
  BEGIN
    INSERT INTO dbo.[User_Role] (UserId, RoleDefinitionId)
    VALUES (@UserId, @RoleDefinitionId);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS Id;
  END
  ELSE
  BEGIN
    SELECT CAST(-1 AS BIGINT) AS Id;
  END
END
GO
=======
  IF NOT EXISTS (SELECT 1 FROM dbo.[User_Role] WHERE UserID=@UserId AND RoleDefinitionID=@RoleDefinitionId)
    INSERT INTO dbo.[User_Role](UserID, RoleDefinitionID) VALUES(@UserId, @RoleDefinitionId);
ENDEND
>>>>>>> Stashed changes
