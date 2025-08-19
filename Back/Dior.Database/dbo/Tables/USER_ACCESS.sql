CREATE TABLE [dbo].[USER_ACCESS] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [userId]     BIGINT         NOT NULL,
    [accessId]   INT            NOT NULL,
    [createdAt]  DATETIME       DEFAULT (getdate()) NOT NULL,
    [createdBy]  NVARCHAR (100) NULL,
    [lastEditAt] DATETIME       NULL,
    [lastEditBy] NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_UserAccess_Access] FOREIGN KEY ([accessId]) REFERENCES [dbo].[ACCESS] ([id]),
    CONSTRAINT [FK_UserAccess_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[USER] ([ID])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [idx_user_access_unique]
    ON [dbo].[USER_ACCESS]([userId] ASC, [accessId] ASC);

