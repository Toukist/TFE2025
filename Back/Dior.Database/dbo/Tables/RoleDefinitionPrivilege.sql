CREATE TABLE [dbo].[RoleDefinitionPrivilege] (
    [id]               INT            IDENTITY (1, 1) NOT NULL,
    [roleDefinitionId] INT            NOT NULL,
    [PrivilegeId]      BIGINT         NOT NULL,
    [createdAt]        DATETIME       DEFAULT (getdate()) NOT NULL,
    [createdBy]        NVARCHAR (100) NULL,
    [lastEditAt]       DATETIME       NULL,
    [lastEditBy]       NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_RoleDefinitionPrivilege_Privilege] FOREIGN KEY ([PrivilegeId]) REFERENCES [dbo].[PRIVILEGE] ([ID]),
    CONSTRAINT [FK_RoleDefinitionPrivilege_RoleDefinition] FOREIGN KEY ([roleDefinitionId]) REFERENCES [dbo].[ROLE_DEFINITION] ([id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [idx_role_privilege_unique]
    ON [dbo].[RoleDefinitionPrivilege]([roleDefinitionId] ASC, [PrivilegeId] ASC);

