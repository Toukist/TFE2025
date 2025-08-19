IF OBJECT_ID('sp_AddRoleDefinition', 'P') IS NOT NULL DROP PROCEDURE sp_AddRoleDefinition
GO
CREATE PROCEDURE sp_AddRoleDefinition
    @name NVARCHAR(100),
    @description NVARCHAR(MAX) = NULL,
    @parentRoleId INT = NULL,
    @isActive BIT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL,
    @lastEditAt DATETIME = NULL,
    @lastEditBy NVARCHAR(100) = NULL
AS
BEGIN
    INSERT INTO ROLE_DEFINITION (name, description, parentRoleId, isActive, createdAt, createdBy, lastEditAt, lastEditBy)
    VALUES (@name, @description, @parentRoleId, @isActive, @createdAt, @createdBy, @lastEditAt, @lastEditBy)
END
GO

IF OBJECT_ID('sp_GetRoleDefinitionById', 'P') IS NOT NULL DROP PROCEDURE sp_GetRoleDefinitionById
GO
CREATE PROCEDURE sp_GetRoleDefinitionById
    @id INT
AS
BEGIN
    SELECT * FROM ROLE_DEFINITION WHERE id = @id
END
GO

IF OBJECT_ID('sp_UpdateRoleDefinition', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateRoleDefinition
GO
CREATE PROCEDURE sp_UpdateRoleDefinition
    @id INT,
    @name NVARCHAR(100),
    @description NVARCHAR(MAX) = NULL,
    @parentRoleId INT = NULL,
    @isActive BIT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL,
    @lastEditAt DATETIME = NULL,
    @lastEditBy NVARCHAR(100) = NULL
AS
BEGIN
    UPDATE ROLE_DEFINITION
    SET name = @name,
        description = @description,
        parentRoleId = @parentRoleId,
        isActive = @isActive,
        createdAt = @createdAt,
        createdBy = @createdBy,
        lastEditAt = @lastEditAt,
        lastEditBy = @lastEditBy
    WHERE id = @id
END
GO

IF OBJECT_ID('sp_DeleteRoleDefinition', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteRoleDefinition
GO
CREATE PROCEDURE sp_DeleteRoleDefinition
    @id INT
AS
BEGIN
    DELETE FROM ROLE_DEFINITION WHERE id = @id
END
GO
