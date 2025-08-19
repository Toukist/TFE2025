
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Met à jour une association rôle/privilege en renseignant automatiquement LastEditAt et LastEditBy
-- =============================================
CREATE PROCEDURE dbo.sp_UpdateRoleDefinitionPrivilege
    @id                INT,
    @roleDefinitionId  INT,
    @privilegeId       INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.ROLE_DEFINITION_PRIVILEGE
        SET 
            roleDefinitionId = @roleDefinitionId,
            privilegeId      = @privilegeId,
            lastEditAt       = GETDATE(),       -- date/heure serveur
            lastEditBy       = SUSER_SNAME()    -- utilisateur connecté
        WHERE id = @id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        DECLARE 
            @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE(),
            @ErrorSeverity INT           = ERROR_SEVERITY(),
            @ErrorState   INT            = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END