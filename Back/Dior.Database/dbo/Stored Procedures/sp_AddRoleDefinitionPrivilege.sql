CREATE OR ALTER PROCEDURE dbo.sp_AddRoleDefinitionPrivilege
    @RoleDefinitionId BIGINT,
    @PrivilegeId      BIGINT,
    @By               NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @By IS NULL SET @By = SUSER_SNAME();

    INSERT INTO ROLE_DEFINITION_PRIVILEGE
        (RoleDefinitionId,
         PrivilegeId,
         CreatedAt,
         CreatedBy,
         LastEditAt,
         LastEditBy)
    VALUES
        (
            @RoleDefinitionId,
            @PrivilegeId,
            GETDATE(),
            @By,
            GETDATE(),
            @By
        );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS Id;
END
GO