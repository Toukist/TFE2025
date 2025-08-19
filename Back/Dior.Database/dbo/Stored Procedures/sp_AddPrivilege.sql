
CREATE PROCEDURE [dbo].[sp_AddPrivilege]
    @Name                    NVARCHAR(50),
    @Description             NVARCHAR(MAX) = NULL,
    @IsConfigurableRead      BIT,
    @IsConfigurableDelete    BIT,
    @IsConfigurableAdd       BIT,
    @IsConfigurableModify    BIT,
    @IsConfigurableStatus    BIT,
    @IsConfigurableExecution BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.PRIVILEGE
        ([Name],
         [Description],
         [IsConfigurableRead],
         [IsConfigurableDelete],
         [IsConfigurableAdd],
         [IsConfigurableModify],
         [IsConfigurableStatus],
         [IsConfigurableExecution],
         LastEditBy,
         LastEditAt)
    VALUES
        (
            @Name,
            @Description,
            @IsConfigurableRead,
            @IsConfigurableDelete,
            @IsConfigurableAdd,
            @IsConfigurableModify,
            @IsConfigurableStatus,
            @IsConfigurableExecution,
            SUSER_SNAME(),  -- utilisateur connecté
            GETDATE()       -- date/heure serveur
        );
END