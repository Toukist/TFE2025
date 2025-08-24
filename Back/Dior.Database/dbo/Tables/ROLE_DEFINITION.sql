CREATE TABLE [dbo].[ROLE_DEFINITION] (
    [id]           INT            IDENTITY (1, 1) NOT NULL,
    [name]         NVARCHAR (100) NOT NULL,
    [description]  NVARCHAR (MAX) NULL,
    [parentRoleId] INT            NULL,
    [isActive]     BIT            DEFAULT ((1)) NOT NULL,
    [createdAt]    DATETIME       DEFAULT (getdate()) NOT NULL,
    [createdBy]    NVARCHAR (100) NULL,
    [lastEditAt]   DATETIME       NULL,
    [lastEditBy]   NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_RoleDefinition_Parent] FOREIGN KEY ([parentRoleId]) REFERENCES [dbo].[ROLE_DEFINITION] ([id]),
    CONSTRAINT [UQ_roleDefinition_name] UNIQUE NONCLUSTERED ([name] ASC)
);


GO
CREATE NONCLUSTERED INDEX [idx_role_definition_name]
    ON [dbo].[ROLE_DEFINITION]([name] ASC);

