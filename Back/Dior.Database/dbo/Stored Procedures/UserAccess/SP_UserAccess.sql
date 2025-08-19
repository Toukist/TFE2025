IF OBJECT_ID('sp_AddUserAccess', 'P') IS NOT NULL DROP PROCEDURE sp_AddUserAccess
GO
CREATE PROCEDURE sp_AddUserAccess
    @userId INT,
    @accessId INT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL,
    @lastEditAt DATETIME = NULL,
    @lastEditBy NVARCHAR(100) = NULL
AS
BEGIN
    INSERT INTO USER_ACCESS (userId, accessId, createdAt, createdBy, lastEditAt, lastEditBy)
    VALUES (@userId, @accessId, @createdAt, @createdBy, @lastEditAt, @lastEditBy)
END
GO

IF OBJECT_ID('sp_GetUserAccessById', 'P') IS NOT NULL DROP PROCEDURE sp_GetUserAccessById
GO
CREATE PROCEDURE sp_GetUserAccessById
    @id INT
AS
BEGIN
    SELECT * FROM USER_ACCESS WHERE id = @id
END
GO

IF OBJECT_ID('sp_UpdateUserAccess', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateUserAccess
GO
CREATE PROCEDURE sp_UpdateUserAccess
    @id INT,
    @userId INT,
    @accessId INT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL,
    @lastEditAt DATETIME = NULL,
    @lastEditBy NVARCHAR(100) = NULL
AS
BEGIN
    UPDATE USER_ACCESS
    SET userId = @userId,
        accessId = @accessId,
        createdAt = @createdAt,
        createdBy = @createdBy,
        lastEditAt = @lastEditAt,
        lastEditBy = @lastEditBy
    WHERE id = @id
END
GO

IF OBJECT_ID('sp_DeleteUserAccess', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteUserAccess
GO
CREATE PROCEDURE sp_DeleteUserAccess
    @id INT
AS
BEGIN
    DELETE FROM USER_ACCESS WHERE id = @id
END
GO
