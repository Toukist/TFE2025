CREATE OR ALTER PROCEDURE dbo.sp_UpdateUserAccess
  @id       BIGINT,
  @userId   BIGINT,
  @accessId BIGINT,
  @By       NVARCHAR(100)=NULL
AS
BEGIN
  SET NOCOUNT ON;
  IF @By IS NULL SET @By = SUSER_SNAME();

  UPDATE dbo.USER_ACCESS
  SET UserId = @userId,
      AccessId = @accessId,
      LastEditAt = GETDATE(),
      LastEditBy = @By
  WHERE Id = @id;

  DECLARE @details NVARCHAR(200) = N'AccessId=' + CAST(@accessId AS NVARCHAR(50));

  EXEC dbo.sp_AddAuditLog
       @userId = @userId, @action = N'UPDATE', @tableName = N'USER_ACCESS',
       @recordId = @id, @details = @details, @timestamp = NULL;
END
GO