IF OBJECT_ID('sp_AddUserAccessCompetency', 'P') IS NOT NULL DROP PROCEDURE sp_AddUserAccessCompetency
GO
CREATE PROCEDURE sp_AddUserAccessCompetency
    @userId INT,
    @accessCompetencyId INT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL,
    @lastEditAt DATETIME = NULL,
    @lastEditBy NVARCHAR(100) = NULL
AS
BEGIN
    INSERT INTO USER_ACCESS_COMPETENCY (userId, accessCompetencyId, createdAt, createdBy, lastEditAt, lastEditBy)
    VALUES (@userId, @accessCompetencyId, @createdAt, @createdBy, @lastEditAt, @lastEditBy)
END
GO

IF OBJECT_ID('sp_GetUserAccessCompetencyById', 'P') IS NOT NULL DROP PROCEDURE sp_GetUserAccessCompetencyById
GO
CREATE PROCEDURE sp_GetUserAccessCompetencyById
    @id INT
AS
BEGIN
    SELECT * FROM USER_ACCESS_COMPETENCY WHERE id = @id
END
GO

IF OBJECT_ID('sp_UpdateUserAccessCompetency', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateUserAccessCompetency
GO
CREATE PROCEDURE sp_UpdateUserAccessCompetency
    @id INT,
    @userId INT,
    @accessCompetencyId INT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL,
    @lastEditAt DATETIME = NULL,
    @lastEditBy NVARCHAR(100) = NULL
AS
BEGIN
    UPDATE USER_ACCESS_COMPETENCY
    SET userId = @userId,
        accessCompetencyId = @accessCompetencyId,
        createdAt = @createdAt,
        createdBy = @createdBy,
        lastEditAt = @lastEditAt,
        lastEditBy = @lastEditBy
    WHERE id = @id
END
GO

IF OBJECT_ID('sp_DeleteUserAccessCompetency', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteUserAccessCompetency
GO
CREATE PROCEDURE sp_DeleteUserAccessCompetency
    @id INT
AS
BEGIN
    DELETE FROM USER_ACCESS_COMPETENCY WHERE id = @id
END
GO
