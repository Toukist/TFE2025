
CREATE   PROCEDURE dbo.sp_DeleteRoleDefinitionPrivilege
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.RoleDefinition_Privilege WHERE Id = @Id;
END