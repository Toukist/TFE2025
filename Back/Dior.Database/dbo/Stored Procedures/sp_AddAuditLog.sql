CREATE   PROCEDURE dbo.sp_AddAuditLog
  @userId    BIGINT,
  @action    NVARCHAR(100),
  @tableName NVARCHAR(100),
  @recordId  INT,
  @details   NVARCHAR(MAX)=NULL,
  @timestamp DATETIME=NULL
AS
BEGIN
  SET NOCOUNT ON;
  IF @timestamp IS NULL SET @timestamp = GETDATE();
  INSERT INTO dbo.AUDIT_LOGS (userId, action, tableName, recordId, details, [timestamp])
  VALUES (@userId, @action, @tableName, @recordId, @details, @timestamp);
END