
/* =======================
   4) LIST USERS WITH ROLES
   ======================= */
CREATE   PROCEDURE dbo.sp_GetUsersWithRoles
  @IncludeInactive BIT = 0
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    u.Id            AS UserId,
    u.UserName,
    u.FirstName,
    u.LastName,
    u.Email,
    u.Phone,
    u.TeamId,
    t.[Name]        AS TeamName,
    u.IsActive,
    u.LastEditAt,
    u.LastEditBy,
    rd.Id           AS RoleId,
    rd.[Name]       AS RoleName
  FROM dbo.[User] u
  LEFT JOIN dbo.[User_Role] ur ON ur.UserID = u.Id
  LEFT JOIN dbo.[RoleDefinition] rd ON rd.Id = ur.RoleDefinitionID  -- assumes typo fixed
  LEFT JOIN dbo.[Team] t ON t.Id = u.TeamId
  WHERE (@IncludeInactive = 1) OR (u.IsActive = 1)
  ORDER BY u.LastName, u.FirstName, u.UserName;
END