CREATE TABLE [dbo].[ACCESS] (
    [id]                  INT            IDENTITY (1, 1) NOT NULL,
    [badgePhysicalNumber] NVARCHAR (100) NOT NULL,
    [isActive]            BIT            DEFAULT ((1)) NOT NULL,
    [createdAt]           DATETIME       DEFAULT (getdate()) NOT NULL,
    [createdBy]           NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [UQ_access_badgePhysicalNumber] UNIQUE NONCLUSTERED ([badgePhysicalNumber] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Access_Badge]
    ON [dbo].[ACCESS]([badgePhysicalNumber] ASC) WHERE ([badgePhysicalNumber] IS NOT NULL);

