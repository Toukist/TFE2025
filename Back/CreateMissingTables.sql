-- =====================================================
-- Script de cr�ation des tables manquantes pour 
-- Dior Enterprise Management System
-- Version finale avec gestion d'erreurs
-- =====================================================

USE [Dior.Database];
GO

PRINT '?? D�but du script de cr�ation des tables pour Dior Enterprise System';
PRINT '================================================================';

-- Table Messages pour le syst�me de messagerie
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Messages')
BEGIN
    BEGIN TRY
        CREATE TABLE [dbo].[Messages] (
            [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            [SenderId] BIGINT NOT NULL,
            [RecipientUserId] BIGINT NULL,
            [RecipientTeamId] INT NULL,
            [Subject] NVARCHAR(200) NOT NULL,
            [Content] NVARCHAR(MAX) NOT NULL,
            [Priority] NVARCHAR(20) DEFAULT 'Normal',
            [MessageType] NVARCHAR(50) DEFAULT 'General',
            [IsRead] BIT DEFAULT 0,
            [ReadAt] DATETIME NULL,
            [SentAt] DATETIME DEFAULT GETDATE()
        );
        
        -- Ajouter les contraintes FK si les tables existent
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'USER')
        BEGIN
            ALTER TABLE [dbo].[Messages] ADD CONSTRAINT FK_Messages_Sender 
                FOREIGN KEY ([SenderId]) REFERENCES [USER]([ID]);
            ALTER TABLE [dbo].[Messages] ADD CONSTRAINT FK_Messages_RecipientUser 
                FOREIGN KEY ([RecipientUserId]) REFERENCES [USER]([ID]);
        END
        
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Team')
        BEGIN
            ALTER TABLE [dbo].[Messages] ADD CONSTRAINT FK_Messages_RecipientTeam 
                FOREIGN KEY ([RecipientTeamId]) REFERENCES [Team]([Id]);
        END
        
        PRINT '? Table Messages cr��e avec succ�s';
    END TRY
    BEGIN CATCH
        PRINT '? Erreur lors de la cr�ation de Messages: ' + ERROR_MESSAGE();
    END CATCH
END
ELSE
BEGIN
    PRINT '?? Table Messages existe d�j�';
END

-- Table Payslips pour les fiches de paie
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payslips')
BEGIN
    BEGIN TRY
        CREATE TABLE [dbo].[Payslips] (
            [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            [UserId] BIGINT NOT NULL,
            [Month] INT NOT NULL CHECK ([Month] >= 1 AND [Month] <= 12),
            [Year] INT NOT NULL CHECK ([Year] >= 2020 AND [Year] <= 2050),
            [GrossSalary] DECIMAL(18,2) NOT NULL CHECK ([GrossSalary] >= 0),
            [NetSalary] DECIMAL(18,2) NOT NULL CHECK ([NetSalary] >= 0),
            [Deductions] DECIMAL(18,2) DEFAULT 0 CHECK ([Deductions] >= 0),
            [Bonus] DECIMAL(18,2) DEFAULT 0 CHECK ([Bonus] >= 0),
            [FileUrl] NVARCHAR(500) DEFAULT '',
            [IsSent] BIT DEFAULT 0,
            [SentDate] DATETIME NULL,
            [GeneratedAt] DATETIME DEFAULT GETDATE(),
            [GeneratedBy] NVARCHAR(100) DEFAULT 'System',
            CONSTRAINT UQ_Payslip_Period UNIQUE ([UserId], [Month], [Year])
        );
        
        -- FK vers USER si elle existe
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'USER')
        BEGIN
            ALTER TABLE [dbo].[Payslips] ADD CONSTRAINT FK_Payslips_User 
                FOREIGN KEY ([UserId]) REFERENCES [USER]([ID]);
        END
        
        PRINT '? Table Payslips cr��e avec succ�s';
    END TRY
    BEGIN CATCH
        PRINT '? Erreur lors de la cr�ation de Payslips: ' + ERROR_MESSAGE();
    END CATCH
END
ELSE
BEGIN
    PRINT '?? Table Payslips existe d�j�';
END

-- Am�lioration de la table Contract
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Contract')
BEGIN
    PRINT '?? Am�lioration de la table Contract...';
    
    -- Ajouter les colonnes manquantes une par une
    DECLARE @columns TABLE (ColumnName NVARCHAR(128), DataType NVARCHAR(128), DefaultValue NVARCHAR(128))
    INSERT INTO @columns VALUES 
        ('ContractType', 'NVARCHAR(50)', '''CDI'''),
        ('StartDate', 'DATETIME', 'NULL'),
        ('EndDate', 'DATETIME', 'NULL'),
        ('Salary', 'DECIMAL(18,2)', '0'),
        ('Currency', 'NVARCHAR(10)', '''EUR'''),
        ('PaymentFrequency', 'NVARCHAR(50)', '''Mensuel'''),
        ('Status', 'NVARCHAR(50)', '''Actif'''),
        ('LastEditAt', 'DATETIME', 'NULL'),
        ('LastEditBy', 'NVARCHAR(100)', 'NULL');
    
    DECLARE @colName NVARCHAR(128), @dataType NVARCHAR(128), @defaultVal NVARCHAR(128);
    DECLARE col_cursor CURSOR FOR SELECT ColumnName, DataType, DefaultValue FROM @columns;
    OPEN col_cursor;
    FETCH NEXT FROM col_cursor INTO @colName, @dataType, @defaultVal;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Contract') AND name = @colName)
        BEGIN
            BEGIN TRY
                DECLARE @sql NVARCHAR(MAX) = 'ALTER TABLE [dbo].[Contract] ADD [' + @colName + '] ' + @dataType;
                IF @defaultVal != 'NULL'
                    SET @sql = @sql + ' DEFAULT ' + @defaultVal;
                EXEC sp_executesql @sql;
                PRINT '  ? Colonne ' + @colName + ' ajout�e';
            END TRY
            BEGIN CATCH
                PRINT '  ? Erreur ajout colonne ' + @colName + ': ' + ERROR_MESSAGE();
            END CATCH
        END
        FETCH NEXT FROM col_cursor INTO @colName, @dataType, @defaultVal;
    END
    
    CLOSE col_cursor;
    DEALLOCATE col_cursor;
END

-- Am�lioration de la table Projet
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Projet')
BEGIN
    PRINT '?? Am�lioration de la table Projet...';
    
    -- Ajouter ManagerId
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Projet') AND name = 'ManagerId')
    BEGIN
        BEGIN TRY
            ALTER TABLE [dbo].[Projet] ADD [ManagerId] BIGINT;
            IF EXISTS (SELECT * FROM sys.tables WHERE name = 'USER')
            BEGIN
                ALTER TABLE [dbo].[Projet] ADD CONSTRAINT FK_Projet_Manager 
                    FOREIGN KEY ([ManagerId]) REFERENCES [USER]([ID]);
            END
            PRINT '  ? Colonne ManagerId ajout�e';
        END TRY
        BEGIN CATCH
            PRINT '  ? Erreur ajout ManagerId: ' + ERROR_MESSAGE();
        END CATCH
    END

    -- Ajouter Type
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Projet') AND name = 'Type')
    BEGIN
        BEGIN TRY
            ALTER TABLE [dbo].[Projet] ADD [Type] NVARCHAR(50) DEFAULT 'Equipe';
            PRINT '  ? Colonne Type ajout�e';
        END TRY
        BEGIN CATCH
            PRINT '  ? Erreur ajout Type: ' + ERROR_MESSAGE();
        END CATCH
    END

    -- Ajouter Progress
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Projet') AND name = 'Progress')
    BEGIN
        BEGIN TRY
            ALTER TABLE [dbo].[Projet] ADD [Progress] INT DEFAULT 0 CHECK ([Progress] >= 0 AND [Progress] <= 100);
            PRINT '  ? Colonne Progress ajout�e';
        END TRY
        BEGIN CATCH
            PRINT '  ? Erreur ajout Progress: ' + ERROR_MESSAGE();
        END CATCH
    END
END

-- Cr�ation des index pour les performances
PRINT '?? Cr�ation des index de performance...';

-- Index sur Messages
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Messages')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Messages_SenderId')
        CREATE INDEX IX_Messages_SenderId ON Messages(SenderId);
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Messages_RecipientUserId')
        CREATE INDEX IX_Messages_RecipientUserId ON Messages(RecipientUserId) WHERE RecipientUserId IS NOT NULL;
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Messages_RecipientTeamId')
        CREATE INDEX IX_Messages_RecipientTeamId ON Messages(RecipientTeamId) WHERE RecipientTeamId IS NOT NULL;
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Messages_SentAt')
        CREATE INDEX IX_Messages_SentAt ON Messages(SentAt DESC);
        
    PRINT '  ? Index Messages cr��s';
END

-- Index sur Payslips
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Payslips')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Payslips_UserId')
        CREATE INDEX IX_Payslips_UserId ON Payslips(UserId);
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Payslips_Period')
        CREATE INDEX IX_Payslips_Period ON Payslips(Year, Month);
        
    PRINT '  ? Index Payslips cr��s';
END

-- Index sur Contract
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Contract')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Contract_UserId')
        CREATE INDEX IX_Contract_UserId ON Contract(UserId);
        
    PRINT '  ? Index Contract cr��s';
END

-- Donn�es de test (optionnel)
PRINT '?? V�rification des donn�es de base...';

-- V�rifier si on a des utilisateurs
DECLARE @userCount INT = 0;
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'USER')
    SELECT @userCount = COUNT(*) FROM [USER];

PRINT '  Utilisateurs dans le syst�me: ' + CAST(@userCount AS NVARCHAR(10));

-- V�rifier si on a des �quipes
DECLARE @teamCount INT = 0;
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Team')
    SELECT @teamCount = COUNT(*) FROM Team;

PRINT '  �quipes dans le syst�me: ' + CAST(@teamCount AS NVARCHAR(10));

PRINT '================================================================';
PRINT '?? Script termin� avec succ�s!';
PRINT '';
PRINT '?? R�SUM�:';
PRINT '? Tables cr��es/mises � jour:';
PRINT '   - Messages (syst�me de messagerie)';
PRINT '   - Payslips (fiches de paie)';
PRINT '   - Contract (colonnes �tendues)';
PRINT '   - Projet (colonnes Manager et Progress)';
PRINT '? Index de performance cr��s';
PRINT '';
PRINT '?? Le syst�me Dior Enterprise est pr�t!';
PRINT '   API: https://localhost:7201';
PRINT '   Swagger: https://localhost:7201/swagger';

GO