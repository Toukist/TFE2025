CREATE OR ALTER PROCEDURE dbo.sp_UpdateRoleDefinition
    @Id             BIGINT,
    @Name           NVARCHAR(100),
    @Description    NVARCHAR(MAX)   = NULL,
    @ParentRoleId   BIGINT          = NULL,
    @IsActive       BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ROLE_DEFINITION
    SET 
        [Name]         = @Name,
        [Description]  = @Description,
        ParentRoleId   = @ParentRoleId,
        IsActive       = @IsActive,
        LastEditAt     = GETDATE(),
        LastEditBy     = SUSER_SNAME()
    WHERE Id = @Id;
END
GO