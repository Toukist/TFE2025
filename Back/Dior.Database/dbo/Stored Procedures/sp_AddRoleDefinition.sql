
CREATE   PROCEDURE dbo.sp_AddRoleDefinition
    @Name NVARCHAR(100),
    @Description NVARCHAR(255) = NULL,
    @ParentRoleId BIGINT = NULL,
    @IsActive BIT = 1,
    @By NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @By IS NULL SET @By = SUSER_SNAME();

    INSERT INTO dbo.Role_Definition (Name, Description, ParentRoleId, IsActive, CreatedAt, CreatedBy, LastEditAt, LastEditBy)
    VALUES (@Name, @Description, @ParentRoleId, @IsActive, GETDATE(), @By, GETDATE(), @By);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS Id;
END