CREATE PROCEDURE SP_UserAccessCompetency_GetList
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        uac.Id,
        uac.UserId,
        uac.AccessCompetencyId,
        uac.CreatedAt,
        uac.CreatedBy,
        uac.LastEditAt,
        uac.LastEditBy
    FROM 
        USER_ACCESS_COMPETENCY uac
END