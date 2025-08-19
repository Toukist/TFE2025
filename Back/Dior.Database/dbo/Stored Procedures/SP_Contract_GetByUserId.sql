CREATE PROCEDURE SP_Contract_GetByUserId
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        FileName,
        FileUrl,         -- ✅ inclus ici maintenant
        UploadDate,
        Description,
        UploadedBy,
        LastEditAt,
        LastEditBy
    FROM dbo.Contract
    WHERE UserId = @UserId
    ORDER BY UploadDate DESC;
END