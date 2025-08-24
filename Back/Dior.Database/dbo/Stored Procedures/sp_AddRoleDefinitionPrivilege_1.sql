
-- ================================
-- ROLE DEFINITION PRIVILEGE
-- ================================
CREATE   PROCEDURE dbo.sp_AddRoleDefinitionPrivilege
    @RoleDefinitionId BIGINT,
    @PrivilegeId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.RoleDefinition_Privilege
        WHERE RoleDefinitionId = @RoleDefinitionId AND PrivilegeId = @PrivilegeId
    )
    BEGIN
        INSERT INTO dbo.RoleDefinition_Privilege (RoleDefinitionId, PrivilegeId)
        VALUES (@RoleDefinitionId, @PrivilegeId);
    END
END