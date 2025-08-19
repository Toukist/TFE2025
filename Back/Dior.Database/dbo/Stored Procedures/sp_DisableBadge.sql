CREATE PROCEDURE sp_DisableBadge
    @UserAccessId INT
AS
BEGIN
    UPDATE Access
    SET isActive = 0
    WHERE id = (SELECT accessId FROM UserAccess WHERE id = @UserAccessId)
END