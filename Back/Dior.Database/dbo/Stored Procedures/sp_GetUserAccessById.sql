
CREATE   PROCEDURE dbo.sp_GetUserAccessById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ua.Id, ua.UserId, ua.AccessId, ua.CreatedAt, ua.CreatedBy
    FROM dbo.User_Access ua
    WHERE ua.Id = @Id;
END