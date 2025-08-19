CREATE TABLE [dbo].[Contract] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [UserId]           BIGINT          NOT NULL,
    [FileName]         NVARCHAR (255)  NOT NULL,
    [FileUrl]          NVARCHAR (1024) NOT NULL,
    [UploadDate]       DATETIME        DEFAULT (getdate()) NOT NULL,
    [Description]      NVARCHAR (MAX)  NULL,
    [UploadedBy]       NVARCHAR (100)  NULL,
    [LastEditAt]       DATETIME        NULL,
    [LastEditBy]       NVARCHAR (100)  NULL,
    [ContractType]     NVARCHAR (50)   DEFAULT ('CDI') NULL,
    [StartDate]        DATETIME        NULL,
    [EndDate]          DATETIME        NULL,
    [Salary]           DECIMAL (18, 2) DEFAULT ((0)) NULL,
    [Currency]         NVARCHAR (10)   DEFAULT ('EUR') NULL,
    [PaymentFrequency] NVARCHAR (50)   DEFAULT ('Mensuel') NULL,
    [Status]           NVARCHAR (50)   DEFAULT ('Actif') NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[USER] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Contract_UserId]
    ON [dbo].[Contract]([UserId] ASC);

