
CREATE   PROCEDURE dbo.sp_DeleteUserRole
  @UserId INT,
  @RoleDefinitionId INT
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM dbo.[User_Role]
  WHERE UserID=@UserId AND RoleDefinitionId=@RoleDefinitionId;
END