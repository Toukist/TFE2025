CREATE TABLE [dbo].[ROLE_DEFINITION_PRIVILEGE] (
    [id]               INT            IDENTITY (1, 1) NOT NULL,
    [roleDefinitionId] INT            NOT NULL,
    [privilegeId]      INT            NOT NULL,
    [createdAt]        DATETIME       DEFAULT (getdate()) NOT NULL,
    [createdBy]        NVARCHAR (100) NULL,
    [lastEditAt]       DATETIME       NULL,
    [lastEditBy]       NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_RoleDefinitionPrivilege_RoleDefinition] FOREIGN KEY ([roleDefinitionId]) REFERENCES [dbo].[ROLE_DEFINITION] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [idx_role_privilege_unique]
    ON [dbo].[ROLE_DEFINITION_PRIVILEGE]([roleDefinitionId] ASC, [privilegeId] ASC);

