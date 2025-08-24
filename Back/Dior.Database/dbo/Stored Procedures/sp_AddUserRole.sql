/* =======================
   6) ROLE LINK HELPERS (optional)
   ======================= */
/* Ajout d'un lien User-Role si non existant */
CREATE OR ALTER PROCEDURE dbo.sp_AddUserRole
  @UserId BIGINT,
  @RoleDefinitionId BIGINT
AS
BEGIN
  SET NOCOUNT ON;

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