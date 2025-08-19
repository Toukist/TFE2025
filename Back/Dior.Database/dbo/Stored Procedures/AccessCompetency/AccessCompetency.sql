IF OBJECT_ID('sp_AddAccessCompetency', 'P') IS NOT NULL DROP PROCEDURE sp_AddAccessCompetency
GO
CREATE PROCEDURE sp_AddAccessCompetency
    @name NVARCHAR(100),
    @parentId INT = NULL,
    @isActive BIT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL,
    @lastEditAt DATETIME = NULL,
    @lastEditBy NVARCHAR(100) = NULL
AS
BEGIN
    INSERT INTO ACCESS_COMPETENCY (name, parentId, isActive, createdAt, createdBy, lastEditAt, lastEditBy)
    VALUES (@name, @parentId, @isActive, @createdAt, @createdBy, @lastEditAt, @lastEditBy)
END
GO

IF OBJECT_ID('sp_GetAccessCompetencyById', 'P') IS NOT NULL DROP PROCEDURE sp_GetAccessCompetencyById
GO
CREATE PROCEDURE sp_GetAccessCompetencyById
    @id INT
AS
BEGIN
    SELECT * FROM ACCESS_COMPETENCY WHERE id = @id
END
GO

IF OBJECT_ID('sp_UpdateAccessCompetency', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateAccessCompetency
GO
CREATE PROCEDURE sp_UpdateAccessCompetency
    @id INT,
    @name NVARCHAR(100),
    @parentId INT = NULL,
    @isActive BIT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL,
    @lastEditAt DATETIME = NULL,
    @lastEditBy NVARCHAR(100) = NULL
AS
BEGIN
    UPDATE ACCESS_COMPETENCY
    SET name = @name,
        parentId = @parentId,
        isActive = @isActive,
        createdAt = @createdAt,
        createdBy = @createdBy,
        lastEditAt = @lastEditAt,
        lastEditBy = @lastEditBy
    WHERE id = @id
END
GO

IF OBJECT_ID('sp_DeleteAccessCompetency', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteAccessCompetency
GO
CREATE PROCEDURE sp_DeleteAccessCompetency
    @id INT
AS
BEGIN
    DELETE FROM ACCESS_COMPETENCY WHERE id = @id
END
GO
