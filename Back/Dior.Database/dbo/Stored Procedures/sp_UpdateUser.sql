
/* =======================
   2) UPDATE USER (password kept if NULL/empty)
   ======================= */
CREATE   PROCEDURE dbo.sp_UpdateUser
  @Id           INT,
  @IsActive     BIT,
  @UserName     NVARCHAR(100),
  @FirstName    NVARCHAR(100),
  @LastName     NVARCHAR(100),
  @Email        NVARCHAR(255),
  @Phone        NVARCHAR(50) = NULL,
  @TeamId       INT = NULL,
  @PasswordHash NVARCHAR(200) = NULL,
  @By           NVARCHAR(100) = NULL
AS
BEGIN
  SET NOCOUNT ON;
  IF @By IS NULL SET @By = SUSER_SNAME();

  UPDATE u
  SET
    u.IsActive     = @IsActive,
    u.UserName     = @UserName,
    u.FirstName    = @FirstName,
    u.LastName     = @LastName,
    u.Email        = @Email,
    u.Phone        = @Phone,
    u.TeamId       = @TeamId,
    u.PasswordHash = CASE WHEN NULLIF(LTRIM(RTRIM(@PasswordHash)), N'') IS NULL THEN u.PasswordHash ELSE @PasswordHash END,
    u.LastEditAt   = GETDATE(),
    u.LastEditBy   = @By
  FROM dbo.[User] u
  WHERE u.Id = @Id;
END
GO