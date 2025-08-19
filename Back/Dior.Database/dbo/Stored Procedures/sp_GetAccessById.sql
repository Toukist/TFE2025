
CREATE PROCEDURE [dbo].[sp_GetAccessById]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        id,
        badgePhysicalNumber,
        isActive,
        createdAt,
        createdBy
    FROM ACCESS
    WHERE id = @id;
END