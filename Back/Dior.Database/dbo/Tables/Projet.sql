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
    [ManagerId]   BIGINT         NULL,
    [Type]        NVARCHAR (50)  DEFAULT ('Equipe') NULL,
    [Progress]    INT            DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CHECK ([Progress]>=(0) AND [Progress]<=(100)),
    CONSTRAINT [FK_Projet_Manager] FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[USER] ([ID]),
    CONSTRAINT [FK_Projet_Team] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Team] ([Id])
);

