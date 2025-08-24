-- =============================================
-- Author:    VotreNom
-- Create date: 2025-06-11
-- Description: Met à jour une association rôle/privilege en renseignant automatiquement LastEditAt et LastEditBy
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_UpdateRoleDefinitionPrivilege
    @Id               BIGINT,
    @RoleDefinitionId BIGINT,
    @PrivilegeId      BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.ROLE_DEFINITION_PRIVILEGE
        SET 
            RoleDefinitionId = @RoleDefinitionId,
            PrivilegeId      = @PrivilegeId,
            LastEditAt       = GETDATE(),
            LastEditBy       = SUSER_SNAME()
        WHERE Id = @Id;

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
GO