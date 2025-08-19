IF OBJECT_ID('sp_AddUser', 'P') IS NOT NULL DROP PROCEDURE sp_AddUser
GO
CREATE PROCEDURE sp_AddUser
    @IsAd BIT,
    @IsActivate BIT,
    @Name NVARCHAR(50),
    @LastName NVARCHAR(50),
    @FirstName NVARCHAR(50),
    @LastEditBy NVARCHAR(50),
    @LastEditAt DATETIME
AS
BEGIN
    INSERT INTO [USER] (IsAd, IsActivate, Name, LastName, FirstName, LastEditBy, LastEditAt)
    VALUES (@IsAd, @IsActivate, @Name, @LastName, @FirstName, @LastEditBy, @LastEditAt)
END
GO

IF OBJECT_ID('sp_GetUserById', 'P') IS NOT NULL DROP PROCEDURE sp_GetUserById
GO
CREATE PROCEDURE sp_GetUserById
    @ID BIGINT
AS
BEGIN
    SELECT * FROM [USER] WHERE ID = @ID
END
GO

IF OBJECT_ID('sp_UpdateUser', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateUser
GO
CREATE PROCEDURE sp_UpdateUser
    @ID BIGINT,
    @IsAd BIT,
    @IsActivate BIT,
    @Name NVARCHAR(50),
    @LastName NVARCHAR(50),
    @FirstName NVARCHAR(50),
    @LastEditBy NVARCHAR(50),
    @LastEditAt DATETIME
AS
BEGIN
    UPDATE [USER]
    SET IsAd = @IsAd,
        IsActivate = @IsActivate,
        Name = @Name,
        LastName = @LastName,
        FirstName = @FirstName,
        LastEditBy = @LastEditBy,
        LastEditAt = @LastEditAt
    WHERE ID = @ID
END
GO

IF OBJECT_ID('sp_DeleteUser', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteUser
GO
CREATE PROCEDURE sp_DeleteUser
    @ID BIGINT
AS
BEGIN
    DELETE FROM [USER] WHERE ID = @ID
END
GO
