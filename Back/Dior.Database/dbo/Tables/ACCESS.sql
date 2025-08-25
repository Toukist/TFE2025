CREATE TABLE [dbo].[Access] (
    [Id]                 INT IDENTITY (1, 1) NOT NULL,
    [BadgePhysicalNumber] NVARCHAR (100) NOT NULL,
    [IsActive]            BIT NOT NULL DEFAULT (1),
    [CreatedAt]           DATETIME NOT NULL DEFAULT (GETDATE()),
    [CreatedBy]           NVARCHAR (100) NULL,
    [LastEditAt]          DATETIME NULL,
    [LastEditBy]          NVARCHAR (100) NULL,
    CONSTRAINT [PK_Access] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Access_BadgePhysicalNumber] UNIQUE NONCLUSTERED ([BadgePhysicalNumber] ASC)
);

GO

-- Index unique supplémentaire sur BadgePhysicalNumber (utile si certains clients SQL Server 
-- ne considèrent pas la contrainte unique comme un index exploitable).
CREATE UNIQUE NONCLUSTERED INDEX [IX_Access_Badge]
    ON [dbo].[Access]([BadgePhysicalNumber] ASC) 
    WHERE ([BadgePhysicalNumber] IS NOT NULL);
