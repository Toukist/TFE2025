CREATE TABLE [dbo].[PRIVILEGE] (
    [ID]                      BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (50)  NOT NULL,
    [Description]             NVARCHAR (MAX) NULL,
    [IsConfigurableRead]      BIT            CONSTRAINT [DF_Privilege_IsConfigurableRead] DEFAULT ((0)) NOT NULL,
    [IsConfigurableDelete]    BIT            CONSTRAINT [DF_Privilege_IsConfigurableDelete] DEFAULT ((0)) NOT NULL,
    [IsConfigurableAdd]       BIT            CONSTRAINT [DF_Privilege_IsConfigurableAdd] DEFAULT ((0)) NOT NULL,
    [IsConfigurableModify]    BIT            CONSTRAINT [DF_Privilege_IsConfigurableModify] DEFAULT ((0)) NOT NULL,
    [IsConfigurableStatus]    BIT            CONSTRAINT [DF_Privilege_IsConfigurableStatus] DEFAULT ((0)) NOT NULL,
    [IsConfigurableExecution] BIT            CONSTRAINT [DF_Privilege_IsConfigurableExecution] DEFAULT ((0)) NOT NULL,
    [LastEditBy]              NVARCHAR (50)  NOT NULL,
    [LastEditAt]              DATETIME       CONSTRAINT [DF_Privilege_LastEditAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Privilege] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_Privilege_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

