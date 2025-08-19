CREATE TABLE [dbo].[Contract] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [UserId]      BIGINT          NOT NULL,
    [FileName]    NVARCHAR (255)  NOT NULL,
    [FileUrl]     NVARCHAR (1024) NOT NULL,
    [UploadDate]  DATETIME        DEFAULT (getdate()) NOT NULL,
    [Description] NVARCHAR (MAX)  NULL,
    [UploadedBy]  NVARCHAR (100)  NULL,
    [LastEditAt]  DATETIME        NULL,
    [LastEditBy]  NVARCHAR (100)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[USER] ([ID])
);

