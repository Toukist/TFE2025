CREATE TABLE [dbo].[USER] (
    [ID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [IsActive]     BIT            CONSTRAINT [DF_User_IsActivate] DEFAULT ((1)) NOT NULL,
    [Username]     NVARCHAR (50)  NOT NULL,
    [LastName]     NVARCHAR (50)  NOT NULL,
    [FirstName]    NVARCHAR (50)  NOT NULL,
    [LastEditBy]   NVARCHAR (50)  NOT NULL,
    [LastEditAt]   DATETIME       NOT NULL,
    [passwordHash] NVARCHAR (200) NULL,
    [Email]        NVARCHAR (255) NULL,
    [Phone]        NVARCHAR (20)  NULL,
    [TeamId]       INT            NULL,
    [DeletedAt]    DATETIME       NULL,
    [DeletedBy]    NVARCHAR (100) NULL,
    [CreatedAt]    DATETIME       CONSTRAINT [DF_User_CreatedAt] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]    NVARCHAR (100) NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [CK_User_Email_Format] CHECK ([Email] IS NULL OR [Email] like '%_@_%._%'),
    CONSTRAINT [CK_User_Phone_Format] CHECK ([Phone] IS NULL OR NOT [Phone] like '%[^0-9+() -]%'),
    CONSTRAINT [FK_User_Team] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Team] ([Id]),
    CONSTRAINT [UQ_User_Name] UNIQUE NONCLUSTERED ([Username] ASC),
    CONSTRAINT [UQ_User_Username] UNIQUE NONCLUSTERED ([Username] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_User_UserName]
    ON [dbo].[USER]([Username] ASC);

