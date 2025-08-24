
CREATE   PROCEDURE dbo.sp_UpdateRoleDefinition
    @Id BIGINT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(255) = NULL,
    @ParentRoleId BIGINT = NULL,
    @IsActive BIT,
    @By NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @By IS NULL SET @By = SUSER_SNAME();

    UPDATE dbo.Role_Definition
    SET Name = @Name,
        Description = @Description,
        ParentRoleId = @ParentRoleId,
        IsActive = @IsActive,
        LastEditAt = GETDATE(),
        LastEditBy = @By
    WHERE Id = @Id;
END