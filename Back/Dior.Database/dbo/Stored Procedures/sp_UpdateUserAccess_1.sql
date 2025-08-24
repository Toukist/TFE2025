CREATE   PROCEDURE dbo.sp_UpdateUserAccess
  @id       INT,
  @userId   BIGINT,
  @accessId INT,
  @By       NVARCHAR(100)=NULL
AS
BEGIN
  SET NOCOUNT ON;
  IF @By IS NULL SET @By = SUSER_SNAME();

  UPDATE dbo.USER_ACCESS
  SET userId=@userId, accessId=@accessId,
      lastEditAt=GETDATE(), lastEditBy=@By
  WHERE id=@id;

  DECLARE @details NVARCHAR(200) = N'accessId=' + CAST(@accessId AS NVARCHAR(50));

  EXEC dbo.sp_AddAuditLog
       @userId=@userId, @action=N'UPDATE', @tableName=N'USER_ACCESS',
       @recordId=@id, @details=@details, @timestamp=NULL;
END