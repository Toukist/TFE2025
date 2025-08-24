
-- ================================
-- ACCESS
-- ================================
CREATE   PROCEDURE dbo.sp_GetAccessById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, badgePhysicalNumber, isActive, createdAt, createdBy
    FROM dbo.Access
    WHERE Id = @Id;
END