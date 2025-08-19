CREATE TABLE [dbo].[ACCESS_COMPETENCY] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [name]       NVARCHAR (100) NOT NULL,
    [parentId]   INT            NULL,
    [isActive]   BIT            DEFAULT ((1)) NOT NULL,
    [createdAt]  DATETIME       DEFAULT (getdate()) NOT NULL,
    [createdBy]  NVARCHAR (100) NULL,
    [lastEditAt] DATETIME       NULL,
    [lastEditBy] NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_AccessCompetency_Parent] FOREIGN KEY ([parentId]) REFERENCES [dbo].[ACCESS_COMPETENCY] ([id]),
    CONSTRAINT [UQ_accessCompetency_name] UNIQUE NONCLUSTERED ([name] ASC)
);


GO
CREATE NONCLUSTERED INDEX [idx_access_competency_name]
    ON [dbo].[ACCESS_COMPETENCY]([name] ASC);

