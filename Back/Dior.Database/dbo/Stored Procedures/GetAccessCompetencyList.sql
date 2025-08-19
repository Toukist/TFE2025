CREATE   PROCEDURE dbo.GetAccessCompetencyList
  @UserID BIGINT
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    AC.ID                       AS AccessCompetencyID,
    AC.Name                     AS CompetencyName,
    AC.ParentId                 AS ParentCompetencyId,
    AC.IsActive                 AS CompetencyIsActive,
    CASE WHEN UAC.UserID IS NULL THEN 0 ELSE 1 END AS HasAccess,
    UAC.LastEditBy              AS LastEditedBy,
    UAC.LastEditAt              AS LastEditedAt
  FROM dbo.ACCESS_COMPETENCY AS AC
  LEFT JOIN dbo.USER_ACCESS_COMPETENCY AS UAC
         ON AC.ID = UAC.AccessCompetencyID
        AND UAC.UserID = @UserID
  WHERE AC.IsActive = 1
  ORDER BY AC.Name;
END