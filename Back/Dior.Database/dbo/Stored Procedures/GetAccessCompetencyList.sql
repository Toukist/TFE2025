CREATE OR ALTER PROCEDURE dbo.GetAccessCompetencyList
  @UserID BIGINT
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    AC.Id                        AS AccessCompetencyId,
    AC.Name                      AS CompetencyName,
    AC.ParentId                  AS ParentCompetencyId,
    AC.IsActive                  AS CompetencyIsActive,
    CASE WHEN UAC.UserId IS NULL THEN 0 ELSE 1 END AS HasAccess,
    UAC.LastEditBy               AS LastEditedBy,
    UAC.LastEditAt               AS LastEditedAt
  FROM dbo.ACCESS_COMPETENCY AS AC
  LEFT JOIN dbo.USER_ACCESS_COMPETENCY AS UAC
         ON AC.Id = UAC.AccessCompetencyId
        AND UAC.UserId = @UserID
  WHERE AC.IsActive = 1
  ORDER BY AC.Name;
END
GO