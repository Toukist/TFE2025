IF OBJECT_ID('sp_AddPrivilege', 'P') IS NOT NULL DROP PROCEDURE sp_AddPrivilege
GO
CREATE PROCEDURE sp_AddPrivilege
    @Name NVARCHAR(50),
    @Description NVARCHAR(MAX) = NULL,
    @IsConfigurableRead BIT,
    @IsConfigurableDelete BIT,
    @IsConfigurableAdd BIT,
    @IsConfigurableModify BIT,
    @IsConfigurableStatus BIT,
    @IsConfigurableExecution BIT,
    @LastEditBy NVARCHAR(50),
    @LastEditAt DATETIME
AS
BEGIN
    INSERT INTO PRIVILEGE (Name, Description, IsConfigurableRead, IsConfigurableDelete, IsConfigurableAdd, IsConfigurableModify, IsConfigurableStatus, IsConfigurableExecution, LastEditBy, LastEditAt)
    VALUES (@Name, @Description, @IsConfigurableRead, @IsConfigurableDelete, @IsConfigurableAdd, @IsConfigurableModify, @IsConfigurableStatus, @IsConfigurableExecution, @LastEditBy, @LastEditAt)
END
GO

IF OBJECT_ID('sp_GetPrivilegeById', 'P') IS NOT NULL DROP PROCEDURE sp_GetPrivilegeById
GO
CREATE PROCEDURE sp_GetPrivilegeById
    @ID BIGINT
AS
BEGIN
    SELECT * FROM PRIVILEGE WHERE ID = @ID
END
GO

IF OBJECT_ID('sp_UpdatePrivilege', 'P') IS NOT NULL DROP PROCEDURE sp_UpdatePrivilege
GO
CREATE PROCEDURE sp_UpdatePrivilege
    @ID BIGINT,
    @Name NVARCHAR(50),
    @Description NVARCHAR(MAX) = NULL,
    @IsConfigurableRead BIT,
    @IsConfigurableDelete BIT,
    @IsConfigurableAdd BIT,
    @IsConfigurableModify BIT,
    @IsConfigurableStatus BIT,
    @IsConfigurableExecution BIT,
    @LastEditBy NVARCHAR(50),
    @LastEditAt DATETIME
AS
BEGIN
    UPDATE PRIVILEGE
    SET Name = @Name,
        Description = @Description,
        IsConfigurableRead = @IsConfigurableRead,
        IsConfigurableDelete = @IsConfigurableDelete,
        IsConfigurableAdd = @IsConfigurableAdd,
        IsConfigurableModify = @IsConfigurableModify,
        IsConfigurableStatus = @IsConfigurableStatus,
        IsConfigurableExecution = @IsConfigurableExecution,
        LastEditBy = @LastEditBy,
        LastEditAt = @LastEditAt
    WHERE ID = @ID
END
GO

IF OBJECT_ID('sp_DeletePrivilege', 'P') IS NOT NULL DROP PROCEDURE sp_DeletePrivilege
GO
CREATE PROCEDURE sp_DeletePrivilege
    @ID BIGINT
AS
BEGIN
    DELETE FROM PRIVILEGE WHERE ID = @ID
END
GO
