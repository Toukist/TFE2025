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
        a.badgePhysicalNumber  AS BadgePhysicalNumber, -- Corrigé
        t.Name                 AS TeamName,
        STRING_AGG(r.name, ', ') AS Roles,
        u.IsActive,
        u.LastEditAt,
        u.LastEditBy
    FROM [dbo].[User] u -- Corrigé
    LEFT JOIN [dbo].[UserAccess] ua ON ua.UserId = u.ID -- Corrigé
    LEFT JOIN [dbo].[Access] a ON a.Id = ua.AccessId -- Corrigé
    LEFT JOIN [dbo].[Team] t ON t.Id = u.TeamId -- Corrigé
    LEFT JOIN [dbo].[UserRole] ur ON ur.UserId = u.ID -- Corrigé
    LEFT JOIN [dbo].[RoleDefinition] r ON r.Id = ur.RoleDefinitionId -- Corrigé
    GROUP BY
        u.ID,
        u.TeamId,
        u.Username,
        u.FirstName,
        u.LastName,
        u.Email,
        u.Phone,
        a.badgePhysicalNumber,
        t.Name,
        u.IsActive,
        u.LastEditAt,
        u.LastEditBy;
END