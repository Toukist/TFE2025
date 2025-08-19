IF OBJECT_ID('sp_AddRoleDefinitionPrivilege', 'P') IS NOT NULL DROP PROCEDURE sp_AddRoleDefinitionPrivilege
GO
CREATE PROCEDURE sp_AddRoleDefinitionPrivilege
    @roleDefinitionId INT,
    @privilegeId INT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL,
    @lastEditAt DATETIME = NULL,
    @lastEditBy NVARCHAR(100) = NULL
AS
BEGIN
    INSERT INTO ROLE_DEFINITION_PRIVILEGE (roleDefinitionId, privilegeId, createdAt, createdBy, lastEditAt, lastEditBy)
    VALUES (@roleDefinitionId, @privilegeId, @createdAt, @createdBy, @lastEditAt, @lastEditBy)
END
GO

IF OBJECT_ID('sp_GetRoleDefinitionPrivilegeById', 'P') IS NOT NULL DROP PROCEDURE sp_GetRoleDefinitionPrivilegeById
GO
CREATE PROCEDURE sp_GetRoleDefinitionPrivilegeById
    @id INT
AS
BEGIN
    SELECT * FROM ROLE_DEFINITION_PRIVILEGE WHERE id = @id
END
GO

IF OBJECT_ID('sp_UpdateRoleDefinitionPrivilege', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateRoleDefinitionPrivilege
GO
CREATE PROCEDURE sp_UpdateRoleDefinitionPrivilege
    @id INT,
    @roleDefinitionId INT,
    @privilegeId INT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL,
    @lastEditAt DATETIME = NULL,
    @lastEditBy NVARCHAR(100) = NULL
AS
BEGIN
    UPDATE ROLE_DEFINITION_PRIVILEGE
    SET roleDefinitionId = @roleDefinitionId,
        privilegeId = @privilegeId,
        createdAt = @createdAt,
        createdBy = @createdBy,
        lastEditAt = @lastEditAt,
        lastEditBy = @lastEditBy
    WHERE id = @id
END
GO

IF OBJECT_ID('sp_DeleteRoleDefinitionPrivilege', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteRoleDefinitionPrivilege
GO
CREATE PROCEDURE sp_DeleteRoleDefinitionPrivilege
    @id INT
AS
BEGIN
    DELETE FROM ROLE_DEFINITION_PRIVILEGE WHERE id = @id
END
GO
