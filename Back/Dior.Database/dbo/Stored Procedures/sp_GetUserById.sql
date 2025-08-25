/* =======================
<<<<<<< Updated upstream
   GET USER BY ID (2 result sets: user + roles)
   ======================= */
CREATE OR ALTER PROCEDURE dbo.sp_GetUserById
=======
   5) GET USER BY ID (2 result sets: user + roles)
   ======================= */
CREATE OR ALTER  PROCEDURE dbo.sp_GetUserById
>>>>>>> Stashed changes
  @Id BIGINT
AS
BEGIN
  SET NOCOUNT ON;

  -- User details
  SELECT
    u.Id, u.IsActive, u.UserName, u.FirstName, u.LastName,
    u.Email, u.Phone, u.TeamId, t.[Name] AS TeamName,
    u.LastEditAt, u.LastEditBy
  FROM dbo.[User] u
  LEFT JOIN dbo.[Team] t ON t.Id = u.TeamId
  WHERE u.Id = @Id;

  -- Roles
  SELECT rd.Id AS RoleId, rd.[Name] AS RoleName
  FROM dbo.[User_Role] ur
<<<<<<< Updated upstream
  JOIN dbo.[RoleDefinition] rd ON rd.Id = ur.RoleDefinitionId
  WHERE ur.UserId = @Id;
END
GO
=======
  JOIN dbo.[RoleDefinition] rd ON rd.Id = ur.RoleDefinitionID
  WHERE ur.UserID = @Id;
ENDEND
>>>>>>> Stashed changes
