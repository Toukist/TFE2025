
CREATE   PROCEDURE dbo.sp_UpdateRoleDefinitionPrivilege
    @Id BIGINT,
    @RoleDefinitionId BIGINT,
    @PrivilegeId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.RoleDefinition_Privilege
    SET RoleDefinitionId = @RoleDefinitionId,
        PrivilegeId = @PrivilegeId
    WHERE Id = @Id;
END