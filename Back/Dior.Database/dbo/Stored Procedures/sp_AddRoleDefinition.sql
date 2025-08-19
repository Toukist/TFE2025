
CREATE PROCEDURE [dbo].[sp_AddRoleDefinition]
    @name          NVARCHAR(100),
    @description   NVARCHAR(MAX) = NULL,
    @parentRoleId  INT           = NULL,
    @isActive      BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ROLE_DEFINITION
        (name,
         description,
         parentRoleId,
         isActive,
         createdAt,
         createdBy,
         lastEditAt,
         lastEditBy)
    VALUES
        (
            @name,
            @description,
            @parentRoleId,
            @isActive,
            GETDATE(),        -- date/heure du serveur au moment de l’insertion
            SUSER_SNAME(),    -- utilisateur connecté
            GETDATE(),        -- date/heure du serveur au moment de l’insertion
            SUSER_SNAME()     -- utilisateur connecté
        );
END