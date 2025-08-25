<<<<<<< Updated upstream
﻿CREATE OR ALTER PROCEDURE dbo.sp_DeleteUserRole
=======
﻿CREATE OR ALTER   PROCEDURE dbo.sp_DeleteUserRole
>>>>>>> Stashed changes
  @UserId BIGINT,
  @RoleDefinitionId BIGINT
AS
BEGIN
  SET NOCOUNT ON;
  DELETE FROM dbo.[User_Role]
<<<<<<< Updated upstream
  WHERE UserId = @UserId AND RoleDefinitionId = @RoleDefinitionId;
END
GO
=======
  WHERE UserID=@UserId AND RoleDefinitionId=@RoleDefinitionId;
ENDEND
>>>>>>> Stashed changes
