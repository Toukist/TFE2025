
CREATE PROCEDURE [dbo].[sp_AddRoleDefinitionPrivilege]
    @roleDefinitionId INT,
    @privilegeId      INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ROLE_DEFINITION_PRIVILEGE
        (roleDefinitionId,
         privilegeId,
         createdAt,
         createdBy,
         lastEditAt,
         lastEditBy)
    VALUES
        (
            @roleDefinitionId,
            @privilegeId,
            GETDATE(),        -- date/heure du serveur au moment de l’insertion
            SUSER_SNAME(),    -- utilisateur connecté
            GETDATE(),        -- date/heure du serveur au moment de l’insertion
            SUSER_SNAME()     -- utilisateur connecté
        );
END