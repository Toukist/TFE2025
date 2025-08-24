
/* =======================
   6) ROLE LINK HELPERS (optional)
   ======================= */
CREATE   PROCEDURE dbo.sp_AddUserRole
  @UserId INT,
  @RoleDefinitionId INT
AS
BEGIN
  SET NOCOUNT ON;
  IF NOT EXISTS (SELECT 1 FROM dbo.[User_Role] WHERE UserID=@UserId AND RoleDefinitionID=@RoleDefinitionId)
    INSERT INTO dbo.[User_Role](UserID, RoleDefinitionID) VALUES(@UserId, @RoleDefinitionId);
END