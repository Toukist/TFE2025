<<<<<<< Updated upstream
﻿CREATE TABLE [dbo].[AUDITLOGS] (
=======
﻿CREATE TABLE [dbo].[AUDIT_LOGS] (
>>>>>>> Stashed changes
    [id]        INT            IDENTITY (1, 1) NOT NULL,
    [userId]    BIGINT         NOT NULL,
    [action]    NVARCHAR (100) NOT NULL,
    [tableName] NVARCHAR (100) NOT NULL,
    [recordId]  INT            NOT NULL,
    [details]   NVARCHAR (MAX) NULL,
    [timestamp] DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_AuditLogs_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[USER] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [idx_audit_user_timestamp]
<<<<<<< Updated upstream
    ON [dbo].[AUDITLOGS]([userId] ASC, [timestamp] ASC);
=======
    ON [dbo].[AUDIT_LOGS]([userId] ASC, [timestamp] ASC);

>>>>>>> Stashed changes
