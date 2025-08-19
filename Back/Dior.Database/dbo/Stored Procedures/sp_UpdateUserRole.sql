
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Met à jour une association utilisateur/rôle en renseignant automatiquement LastEditBy et LastEditAt
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateUserRole]
    @ID                 BIGINT,
    @RoleDefinitionID   BIGINT,
    @UserID             BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.USER_ROLE
        SET 
            RoleDefinitionID = @RoleDefinitionID,  -- colonne telle que définie en base
            UserID            = @UserID,
            LastEditBy        = SUSER_SNAME(),       -- utilisateur connecté
            LastEditAt        = GETDATE()            -- date/heure serveur
        WHERE ID = @ID;

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