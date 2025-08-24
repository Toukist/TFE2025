-- ================================
-- USER
-- ================================
CREATE   PROCEDURE dbo.sp_GetUserById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT u.Id, u.IsActive, u.UserName, u.FirstName, u.LastName, u.Email, u.Phone,
           u.TeamId, t.[Name] AS TeamName, u.LastEditAt, u.LastEditBy
    FROM dbo.[User] u
    LEFT JOIN dbo.[Team] t ON t.Id = u.TeamId
    WHERE u.Id = @Id;

    SELECT rd.Id AS RoleId, rd.[Name] AS RoleName
    FROM dbo.[User_Role] ur
    JOIN dbo.[RoleDefinition] rd ON rd.Id = ur.RoleDefinitionID
    WHERE ur.UserID = @Id;
END