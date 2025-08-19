CREATE TABLE [dbo].[Messages] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [SenderId]        BIGINT         NOT NULL,
    [RecipientUserId] BIGINT         NULL,
    [RecipientTeamId] INT            NULL,
    [Subject]         NVARCHAR (200) NOT NULL,
    [Content]         NVARCHAR (MAX) NOT NULL,
    [Priority]        NVARCHAR (20)  DEFAULT ('Normal') NULL,
    [MessageType]     NVARCHAR (50)  DEFAULT ('General') NULL,
    [IsRead]          BIT            DEFAULT ((0)) NULL,
    [ReadAt]          DATETIME       NULL,
    [SentAt]          DATETIME       DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Messages_RecipientTeam] FOREIGN KEY ([RecipientTeamId]) REFERENCES [dbo].[Team] ([Id]),
    CONSTRAINT [FK_Messages_RecipientUser] FOREIGN KEY ([RecipientUserId]) REFERENCES [dbo].[USER] ([ID]),
    CONSTRAINT [FK_Messages_Sender] FOREIGN KEY ([SenderId]) REFERENCES [dbo].[USER] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_SentAt]
    ON [dbo].[Messages]([SentAt] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_RecipientTeamId]
    ON [dbo].[Messages]([RecipientTeamId] ASC) WHERE ([RecipientTeamId] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_RecipientUserId]
    ON [dbo].[Messages]([RecipientUserId] ASC) WHERE ([RecipientUserId] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_SenderId]
    ON [dbo].[Messages]([SenderId] ASC);

