
CREATE PROCEDURE sp_DisableAccessById
    @AccessId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [ACCESS]
    SET isActive = 0
    WHERE ID = @AccessId;

    -- Optionnel : vérifie si quelque chose a été modifié
    IF @@ROWCOUNT = 0
        THROW 50001, 'Aucun badge trouvé avec cet ID', 1;
END;