
CREATE PROCEDURE [dbo].[sp_DeleteRoleDefinitionPrivilege]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM ROLE_DEFINITION_PRIVILEGE
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

        -- Réémettre l’erreur
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END