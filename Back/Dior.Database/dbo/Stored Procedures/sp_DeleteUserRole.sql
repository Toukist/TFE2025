CREATE OR ALTER PROCEDURE dbo.sp_DeleteUserRole
  @UserId BIGINT,
  @RoleDefinitionId BIGINT
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM dbo.[User_Role]
  WHERE UserId = @UserId AND RoleDefinitionId = @RoleDefinitionId;
END
GO