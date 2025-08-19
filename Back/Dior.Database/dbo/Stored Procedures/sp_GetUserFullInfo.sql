CREATE PROCEDURE [dbo].[sp_GetUserFullInfo]
AS
BEGIN
    SELECT 
        u.ID                   AS UserId,
        u.TeamId,
        u.Username,
        u.FirstName,
        u.LastName,
        u.Email,
        u.Phone,
        a.BadgePhysicalNumber,
        t.Name                  AS TeamName,
        STRING_AGG(r.Name, ', ') AS Roles,
        u.IsActive              AS IsActive,
        u.LastEditAt,
        u.LastEditBy
    FROM [USER] u
    LEFT JOIN [USER_ACCESS] ua ON ua.UserId = u.ID
    LEFT JOIN [ACCESS] a ON a.ID = ua.AccessId
    LEFT JOIN [TEAM] t ON t.ID = u.TeamId
    LEFT JOIN [USER_ROLE] ur ON ur.UserId = u.ID
    LEFT JOIN [ROLE_DEFINITION] r ON r.ID = ur.RoleDefinitionId
    GROUP BY 
        u.ID,
        u.TeamId,
        u.Username,
        u.FirstName,
        u.LastName,
        u.Email,
        u.Phone,
        a.BadgePhysicalNumber,
        t.Name,
        u.IsActive,
        u.LastEditAt,
        u.LastEditBy;
END 