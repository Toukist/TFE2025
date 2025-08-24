
CREATE   PROCEDURE dbo.sp_GetRoleDefinitionPrivilegeById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, RoleDefinitionId, PrivilegeId
    FROM dbo.RoleDefinition_Privilege
    WHERE Id = @Id;
END