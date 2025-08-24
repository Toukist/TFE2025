CREATE TABLE [dbo].[PASSWORD_HISTORY] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]       BIGINT         NOT NULL,
    [PasswordHash] NVARCHAR (200) NOT NULL,
    [CreatedAt]    DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PasswordHistory_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[USER] ([ID])
);

