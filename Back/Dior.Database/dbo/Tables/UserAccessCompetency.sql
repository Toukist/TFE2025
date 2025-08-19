CREATE TABLE [dbo].[UserAccessCompetency] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [UserId]             BIGINT         NOT NULL,
    [AccessCompetencyId] INT            NOT NULL,
    [CreatedAt]          DATETIME       DEFAULT (getdate()) NOT NULL,
    [CreatedBy]          NVARCHAR (100) NULL,
    [LastEditAt]         DATETIME       NULL,
    [LastEditBy]         NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserAccessCompetency_AccessCompetency] FOREIGN KEY ([AccessCompetencyId]) REFERENCES [dbo].[ACCESS_COMPETENCY] ([id]),
    CONSTRAINT [FK_UserAccessCompetency_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[USER] ([ID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [idx_user_access_competency_unique]
    ON [dbo].[UserAccessCompetency]([UserId] ASC, [AccessCompetencyId] ASC);

