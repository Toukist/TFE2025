
-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Met à jour un privilège et positionne automatiquement LastEditBy et LastEditAt
-- =============================================
CREATE PROCEDURE dbo.sp_UpdatePrivilege
    @ID                      BIGINT,
    @Name                    NVARCHAR(50),
    @Description             NVARCHAR(MAX) = NULL,
    @IsConfigurableRead      BIT,
    @IsConfigurableDelete    BIT,
    @IsConfigurableAdd       BIT,
    @IsConfigurableModify    BIT,
    @IsConfigurableStatus    BIT,
    @IsConfigurableExecution BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.PRIVILEGE
        SET 
            [Name]                    = @Name,
            [Description]             = @Description,
            IsConfigurableRead        = @IsConfigurableRead,
            IsConfigurableDelete      = @IsConfigurableDelete,
            IsConfigurableAdd         = @IsConfigurableAdd,
            IsConfigurableModify      = @IsConfigurableModify,
            IsConfigurableStatus      = @IsConfigurableStatus,
            IsConfigurableExecution   = @IsConfigurableExecution,
            LastEditBy                = SUSER_SNAME(),  -- utilisateur connecté
            LastEditAt                = GETDATE()       -- date/heure serveur
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