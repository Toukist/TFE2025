CREATE TABLE [dbo].[Payslips] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [UserId]      BIGINT          NOT NULL,
    [Month]       INT             NOT NULL,
    [Year]        INT             NOT NULL,
    [GrossSalary] DECIMAL (18, 2) NOT NULL,
    [NetSalary]   DECIMAL (18, 2) NOT NULL,
    [Deductions]  DECIMAL (18, 2) DEFAULT ((0)) NULL,
    [Bonus]       DECIMAL (18, 2) DEFAULT ((0)) NULL,
    [FileUrl]     NVARCHAR (500)  DEFAULT ('') NULL,
    [IsSent]      BIT             DEFAULT ((0)) NULL,
    [SentDate]    DATETIME        NULL,
    [GeneratedAt] DATETIME        DEFAULT (getdate()) NULL,
    [GeneratedBy] NVARCHAR (100)  DEFAULT ('System') NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CHECK ([Bonus]>=(0)),
    CHECK ([Deductions]>=(0)),
    CHECK ([GrossSalary]>=(0)),
    CHECK ([Month]>=(1) AND [Month]<=(12)),
    CHECK ([NetSalary]>=(0)),
    CHECK ([Year]>=(2020) AND [Year]<=(2050)),
    CONSTRAINT [FK_Payslips_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[USER] ([ID]),
    CONSTRAINT [UQ_Payslip_Period] UNIQUE NONCLUSTERED ([UserId] ASC, [Month] ASC, [Year] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Payslips_Period]
    ON [dbo].[Payslips]([Year] ASC, [Month] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Payslips_UserId]
    ON [dbo].[Payslips]([UserId] ASC);

