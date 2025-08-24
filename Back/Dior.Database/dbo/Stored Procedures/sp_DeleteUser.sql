
CREATE   PROCEDURE dbo.sp_DeleteUser
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.[User] WHERE Id = @Id;
END