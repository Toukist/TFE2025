CREATE PROCEDURE sp_AddAccess
    @badgePhysicalNumber NVARCHAR(100),
    @isActive BIT,
    @createdAt DATETIME,
    @createdBy NVARCHAR(100) = NULL
AS
BEGIN
    INSERT INTO ACCESS (badgePhysicalNumber, isActive, createdAt, createdBy)
    VALUES (@badgePhysicalNumber, @isActive, @createdAt, @createdBy)
END