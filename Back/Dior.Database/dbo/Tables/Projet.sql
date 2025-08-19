CREATE TABLE [dbo].[Projet] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Nom]         NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [DateDebut]   DATE           NULL,
    [DateFin]     DATE           NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [CreatedBy]   NVARCHAR (100) NULL,
    [LastEditAt]  DATETIME       NULL,
    [LastEditBy]  NVARCHAR (100) NULL,
    [TeamId]      INT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Projet_Team] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Team] ([Id])
);

