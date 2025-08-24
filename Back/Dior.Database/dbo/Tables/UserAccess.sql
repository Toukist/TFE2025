CREATE TABLE [dbo].[UserAccess] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     BIGINT         NOT NULL,
    [AccessId]   INT            NOT NULL,
    [CreatedAt]  DATETIME       DEFAULT (getdate()) NOT NULL,
    [CreatedBy]  NVARCHAR (100) NULL,
    [LastEditAt] DATETIME       NULL,
    [LastEditBy] NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserAccess_Access] FOREIGN KEY ([AccessId]) REFERENCES [dbo].[ACCESS] ([id]),
    CONSTRAINT [FK_UserAccess_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[USER] ([ID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [idx_user_access_unique]
    ON [dbo].[UserAccess]([UserId] ASC, [AccessId] ASC);

