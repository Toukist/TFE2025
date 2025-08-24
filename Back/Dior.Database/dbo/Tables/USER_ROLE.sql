CREATE TABLE [dbo].[USER_ROLE] (
    [ID]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [RoleDefinitionID] BIGINT        NOT NULL,
    [UserID]           BIGINT        NOT NULL,
    [LastEditBy]       NVARCHAR (50) NOT NULL,
    [LastEditAt]       DATETIME      NOT NULL,
    CONSTRAINT [PK_User_Role] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_User_Role_User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[USER] ([ID]),
    CONSTRAINT [UQ_User_Role_RoleDefinitiionID_UserID] UNIQUE NONCLUSTERED ([RoleDefinitionID] ASC, [UserID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_UserRole_User_Role]
    ON [dbo].[USER_ROLE]([UserID] ASC, [RoleDefinitionID] ASC);

