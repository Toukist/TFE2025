CREATE PROCEDURE [dbo].[sp_GetUserAccessCompetencyList]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        id,
        userId,
        accessCompetencyId,
        createdAt,
        createdBy,
        lastEditAt,
        lastEditBy
    FROM [USER_ACCESS_COMPETENCY];
END