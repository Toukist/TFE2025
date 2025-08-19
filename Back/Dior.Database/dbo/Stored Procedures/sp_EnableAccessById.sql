CREATE PROCEDURE sp_EnableAccessById
    @AccessId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM ACCESS WHERE ID = @AccessId)
    BEGIN
        RAISERROR('Accès introuvable.', 16, 1);
        RETURN;
    END

    UPDATE ACCESS
    SET isActive = 1
    WHERE ID = @AccessId;
END