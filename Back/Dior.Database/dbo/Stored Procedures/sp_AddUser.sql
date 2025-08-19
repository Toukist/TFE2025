CREATE   PROCEDURE dbo.sp_AddUser
  @UserName     NVARCHAR(100),
  @FirstName    NVARCHAR(100),
  @LastName     NVARCHAR(100),
  @Email        NVARCHAR(255),
  @Phone        NVARCHAR(50)=NULL,
  @TeamId       INT=NULL,
  @PasswordHash NVARCHAR(200)=NULL,
  @By           NVARCHAR(100)=NULL
AS
BEGIN
  SET NOCOUNT ON;
  IF @By IS NULL SET @By = SUSER_SNAME();

  INSERT INTO dbo.[USER] (
    IsActive, Username, FirstName, LastName, Email, Phone, TeamId, passwordHash,
    CreatedAt, CreatedBy, LastEditAt, LastEditBy
  )
  VALUES (
    1, @UserName, @FirstName, @LastName, @Email, @Phone, @TeamId, @PasswordHash,
    GETDATE(), @By, GETDATE(), @By
  );

  SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS Id;
END