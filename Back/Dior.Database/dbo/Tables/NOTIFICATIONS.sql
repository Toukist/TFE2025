CREATE TABLE [dbo].[NOTIFICATIONS] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [UserId]    BIGINT         NOT NULL,
    [Type]      NVARCHAR (50)  NOT NULL,
    [Message]   NVARCHAR (MAX) NULL,
    [IsRead]    BIT            DEFAULT ((0)) NOT NULL,
    [CreatedAt] DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NOTIFICATION_USER] FOREIGN KEY ([UserId]) REFERENCES [dbo].[USER] ([ID])
);

