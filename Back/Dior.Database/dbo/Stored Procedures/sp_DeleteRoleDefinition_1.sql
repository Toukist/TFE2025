
CREATE   PROCEDURE dbo.sp_DeleteRoleDefinition
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Role_Definition WHERE Id = @Id;
END