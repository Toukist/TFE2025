CREATE PROCEDURE dbo.sp_UpdateRoleDefinition
    @id             INT,
    @Name           NVARCHAR(100),
    @Description    NVARCHAR(MAX)   = NULL,
    @ParentRoleId   INT             = NULL,
    @IsActive       BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ROLE_DEFINITION
    SET 
        [Name]         = @Name,
        [Description]  = @Description,
        parentRoleId   = @ParentRoleId,
        isActive       = @IsActive,
        lastEditAt     = GETDATE(),       -- date/heure serveur
        lastEditBy     = SUSER_SNAME()    -- login SQL Server courant
    WHERE id = @id;
END