CREATE   PROCEDURE dbo.sp_AddUserAccess
  @userId   BIGINT,
  @accessId INT,
  @By       NVARCHAR(100)=NULL
AS
BEGIN
  SET NOCOUNT ON;
  IF @By IS NULL SET @By = SUSER_SNAME();

  INSERT INTO dbo.USER_ACCESS (userId, accessId, createdAt, createdBy, lastEditAt, lastEditBy)
  VALUES (@userId, @accessId, GETDATE(), @By, GETDATE(), @By);

  DECLARE @newId INT = SCOPE_IDENTITY();
  DECLARE @details NVARCHAR(200) = N'accessId=' + CAST(@accessId AS NVARCHAR(50));

  EXEC dbo.sp_AddAuditLog
       @userId=@userId, @action=N'ADD', @tableName=N'USER_ACCESS',
       @recordId=@newId, @details=@details, @timestamp=NULL;
END