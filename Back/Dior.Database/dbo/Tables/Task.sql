CREATE TABLE [dbo].[Task] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [Title]            NVARCHAR (255) NOT NULL,
    [Description]      NVARCHAR (MAX) NULL,
    [Status]           NVARCHAR (50)  DEFAULT ('En attente') NOT NULL,
    [AssignedToUserId] BIGINT         NOT NULL,
    [CreatedByUserId]  BIGINT         NOT NULL,
    [CreatedAt]        DATETIME       DEFAULT (getdate()) NOT NULL,
    [DueDate]          DATETIME       NULL,
    [LastEditAt]       DATETIME       NULL,
    [LastEditBy]       NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([AssignedToUserId]) REFERENCES [dbo].[USER] ([ID]),
    FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[USER] ([ID])
);

