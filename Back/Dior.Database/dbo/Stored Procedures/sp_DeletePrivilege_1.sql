
CREATE   PROCEDURE dbo.sp_DeletePrivilege
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Privilege WHERE Id = @Id;
END