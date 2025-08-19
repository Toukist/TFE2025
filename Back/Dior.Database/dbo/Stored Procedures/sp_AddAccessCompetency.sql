
CREATE PROCEDURE [dbo].[sp_AddAccessCompetency]
    @name       NVARCHAR(100),
    @parentId   INT           = NULL,
    @isActive   BIT
AS
BEGIN
    INSERT INTO ACCESS_COMPETENCY
        (name, parentId, isActive, createdAt, createdBy, lastEditAt, lastEditBy)
    VALUES
        (
            @name,
            @parentId,
            @isActive,
            GETDATE(),         -- date/heure du serveur au moment de l’insertion
            SUSER_SNAME(),     -- login SQL Server de l’utilisateur courant
            GETDATE(),         -- date/heure du serveur au moment de l’insertion
            SUSER_SNAME()      -- login SQL Server de l’utilisateur courant
        );
END