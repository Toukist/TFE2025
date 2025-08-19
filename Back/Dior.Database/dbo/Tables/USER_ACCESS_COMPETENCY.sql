CREATE TABLE [dbo].[USER_ACCESS_COMPETENCY] (
    [id]                 INT            IDENTITY (1, 1) NOT NULL,
    [userId]             INT            NOT NULL,
    [accessCompetencyId] INT            NOT NULL,
    [createdAt]          DATETIME       DEFAULT (getdate()) NOT NULL,
    [createdBy]          NVARCHAR (100) NULL,
    [lastEditAt]         DATETIME       NULL,
    [lastEditBy]         NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_UserAccessCompetency_AccessCompetency] FOREIGN KEY ([accessCompetencyId]) REFERENCES [dbo].[ACCESS_COMPETENCY] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [idx_user_access_competency_unique]
    ON [dbo].[USER_ACCESS_COMPETENCY]([userId] ASC, [accessCompetencyId] ASC);

