CREATE PROCEDURE [dbo].[sp_GetPrivilegeList]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        id,
        name,
        description,
        isConfigurableRead,
        isConfigurableDelete,
        isConfigurableAdd,
        isConfigurableModify,
        isConfigurableStatus,
        isConfigurableExecution,
        lastEditBy,
        lastEditAt
    FROM [PRIVILEGE];
END