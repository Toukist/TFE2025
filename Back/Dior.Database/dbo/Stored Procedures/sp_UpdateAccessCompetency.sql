
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Met à jour une compétence d’accès en renseignant automatiquement createdAt et createdBy
-- =============================================
CREATE PROCEDURE dbo.sp_UpdateAccessCompetency
    @id        INT,
    @name      NVARCHAR(100),
    @parentId  INT         = NULL,
    @isActive  BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.ACCESS_COMPETENCY
        SET
            name       = @name,
            parentId   = @parentId,
            isActive   = @isActive,
            createdAt  = GETDATE(),      -- date/heure du serveur
            createdBy  = SUSER_SNAME()   -- utilisateur connecté
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