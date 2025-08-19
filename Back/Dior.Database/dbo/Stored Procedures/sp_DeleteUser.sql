CREATE   PROCEDURE dbo.sp_DeleteUser
  @Id BIGINT,
  @By NVARCHAR(100)=NULL
AS
BEGIN
  SET NOCOUNT ON;
  IF @By IS NULL SET @By = SUSER_SNAME();

  UPDATE dbo.[USER]
  SET IsActive   = 0,
      DeletedAt  = GETDATE(),
      DeletedBy  = @By,
      LastEditAt = GETDATE(),
      LastEditBy = @By
  WHERE ID = @Id;
END