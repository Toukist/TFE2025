
-- ================================
-- ROLE DEFINITION
-- ================================
CREATE   PROCEDURE dbo.sp_GetRoleDefinitionById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Description, ParentRoleId, IsActive,
           CreatedAt, CreatedBy, LastEditAt, LastEditBy
    FROM dbo.Role_Definition
    WHERE Id = @Id;
END