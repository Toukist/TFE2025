IF OBJECT_ID('sp_AddAccess', 'P') IS NOT NULL DROP PROCEDURE sp_AddAccess
GO
CREATE PROCEDURE sp_AddAccess
    @badgePhysicalNumber NVARCHAR(100),
    @isActive BIT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL
AS
BEGIN
    INSERT INTO ACCESS (badgePhysicalNumber, isActive, createdAt, createdBy)
    VALUES (@badgePhysicalNumber, @isActive, @createdAt, @createdBy)
END
GO

IF OBJECT_ID('sp_GetAccessById', 'P') IS NOT NULL DROP PROCEDURE sp_GetAccessById
GO
CREATE PROCEDURE sp_GetAccessById
    @id INT
AS
BEGIN
    SELECT * FROM ACCESS WHERE id = @id
END
GO

IF OBJECT_ID('sp_UpdateAccess', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateAccess
GO
CREATE PROCEDURE sp_UpdateAccess
    @id INT,
    @badgePhysicalNumber NVARCHAR(100),
    @isActive BIT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL
AS
BEGIN
    UPDATE ACCESS
    SET badgePhysicalNumber = @badgePhysicalNumber,
        isActive = @isActive,
        createdAt = @createdAt,
        createdBy = @createdBy
    WHERE id = @id
END
GO

IF OBJECT_ID('sp_DeleteAccess', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteAccess
GO
CREATE PROCEDURE sp_DeleteAccess
    @id INT
AS
BEGIN
    DELETE FROM ACCESS WHERE id = @id
END
GO
