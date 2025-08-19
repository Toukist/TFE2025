IF OBJECT_ID('sp_AddUserRole', 'P') IS NOT NULL DROP PROCEDURE sp_AddUserRole
GO
CREATE PROCEDURE sp_AddUserRole
    @RoleDefinitiionID BIGINT,
    @UserID BIGINT,
    @LastEditBy NVARCHAR(50),
    @LastEditAt DATETIME
AS
BEGIN
    INSERT INTO USER_ROLE (RoleDefinitiionID, UserID, LastEditBy, LastEditAt)
    VALUES (@RoleDefinitiionID, @UserID, @LastEditBy, @LastEditAt)
END
GO

IF OBJECT_ID('sp_GetUserRoleById', 'P') IS NOT NULL DROP PROCEDURE sp_GetUserRoleById
GO
CREATE PROCEDURE sp_GetUserRoleById
    @ID BIGINT
AS
BEGIN
    SELECT * FROM USER_ROLE WHERE ID = @ID
END
GO

IF OBJECT_ID('sp_UpdateUserRole', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateUserRole
GO
CREATE PROCEDURE sp_UpdateUserRole
    @ID BIGINT,
    @RoleDefinitiionID BIGINT,
    @UserID BIGINT,
    @LastEditBy NVARCHAR(50),
    @LastEditAt DATETIME
AS
BEGIN
    UPDATE USER_ROLE
    SET RoleDefinitiionID = @RoleDefinitiionID,
        UserID = @UserID,
        LastEditBy = @LastEditBy,
        LastEditAt = @LastEditAt
    WHERE ID = @ID
END
GO

IF OBJECT_ID('sp_DeleteUserRole', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteUserRole
GO
CREATE PROCEDURE sp_DeleteUserRole
    @ID BIGINT
AS
BEGIN
    DELETE FROM USER_ROLE WHERE ID = @ID
END
GO
