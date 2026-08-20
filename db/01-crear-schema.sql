-- ============================================================
-- Script 1: Creación de la Base de Datos y Tablas
-- FinanzApp Estudiantil
-- ============================================================
-- Ejecutar este script primero para crear la estructura base.
-- ============================================================

USE [master]
GO

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'FinanzAppDB')
BEGIN
    CREATE DATABASE FinanzAppDB
END
GO

USE [FinanzAppDB]
GO

-- ============================================================
-- Schema eliminados (para triggers de auditoría)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'eliminados')
BEGIN
    EXEC('CREATE SCHEMA eliminados')
END
GO

-- ============================================================
-- Tabla: AspNetUsers (extiende IdentityUser)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AspNetUsers] (
        [Id]                NVARCHAR(128)   NOT NULL,
        [UserName]          NVARCHAR(256)   NULL,
        [Email]             NVARCHAR(256)   NULL,
        [EmailConfirmed]    BIT             NOT NULL DEFAULT 0,
        [PasswordHash]      NVARCHAR(MAX)   NULL,
        [SecurityStamp]     NVARCHAR(MAX)   NULL,
        [PhoneNumber]       NVARCHAR(MAX)   NULL,
        [PhoneNumberConfirmed] BIT          NOT NULL DEFAULT 0,
        [TwoFactorEnabled]  BIT             NOT NULL DEFAULT 0,
        [LockoutEnd]        DATETIMEOFFSET  NULL,
        [LockoutEnabled]    BIT             NOT NULL DEFAULT 0,
        [AccessFailedCount] INT             NOT NULL DEFAULT 0,
        -- Campos adicionales FinanzApp
        [FullName]          NVARCHAR(100)   NOT NULL,
        [Currency]          NVARCHAR(3)     NOT NULL DEFAULT 'MXN',
        [MonthlyBudget]     DECIMAL(18,2)   NULL,
        [BudgetMonth]       INT             NULL,
        [BudgetYear]        INT             NULL,
        [CreatedAt]         DATETIME2       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- ============================================================
-- Tabla: Expenses
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Expenses]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Expenses] (
        [Id]            INT             IDENTITY(1,1) NOT NULL,
        [UserId]        NVARCHAR(128)   NOT NULL,
        [Amount]        DECIMAL(18,2)   NOT NULL,
        [Category]      INT             NOT NULL,  -- Enum: 0=Mensualidad, 1=Transporte, 2=Comida, 3=Entretenimiento
        [Date]          DATETIME2       NOT NULL DEFAULT GETDATE(),
        [Note]          NVARCHAR(500)   NULL,
        [CreatedAt]     DATETIME2       NOT NULL DEFAULT GETDATE(),
        [UpdatedAt]     DATETIME2       NULL,
        CONSTRAINT [PK_Expenses] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Expenses_AspNetUsers] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [CK_Expenses_Amount] CHECK ([Amount] > 0)
    )

    -- Índices para consultas del dashboard
    CREATE NONCLUSTERED INDEX [IX_Expenses_User_Date]
        ON [dbo].[Expenses] ([UserId] ASC, [Date] ASC)

    CREATE NONCLUSTERED INDEX [IX_Expenses_User_Category]
        ON [dbo].[Expenses] ([UserId] ASC, [Category] ASC)
END
GO

-- ============================================================
-- Tabla: SavingsGoals
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SavingsGoals]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[SavingsGoals] (
        [Id]            INT             IDENTITY(1,1) NOT NULL,
        [UserId]        NVARCHAR(128)   NOT NULL,
        [Name]          NVARCHAR(100)   NOT NULL,
        [TargetAmount]  DECIMAL(18,2)   NOT NULL,
        [Deadline]      DATETIME2       NOT NULL,
        [CreatedAt]     DATETIME2       NOT NULL DEFAULT GETDATE(),
        [IsCompleted]   BIT             NOT NULL DEFAULT 0,
        CONSTRAINT [PK_SavingsGoals] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_SavingsGoals_AspNetUsers] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [CK_SavingsGoals_TargetAmount] CHECK ([TargetAmount] > 0)
    )
END
GO

-- ============================================================
-- Tabla: SavingsEntries
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SavingsEntries]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[SavingsEntries] (
        [Id]            INT             IDENTITY(1,1) NOT NULL,
        [GoalId]        INT             NOT NULL,
        [Amount]        DECIMAL(18,2)   NOT NULL,
        [Date]          DATETIME2       NOT NULL DEFAULT GETDATE(),
        [CreatedAt]     DATETIME2       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_SavingsEntries] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_SavingsEntries_SavingsGoals] FOREIGN KEY ([GoalId])
            REFERENCES [dbo].[SavingsGoals] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [CK_SavingsEntries_Amount] CHECK ([Amount] > 0)
    )

    CREATE NONCLUSTERED INDEX [IX_SavingsEntries_Goal_Date]
        ON [dbo].[SavingsEntries] ([GoalId] ASC, [Date] ASC)
END
GO

-- ============================================================
-- Tabla: AITips
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AITips]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AITips] (
        [Id]            INT             IDENTITY(1,1) NOT NULL,
        [UserId]        NVARCHAR(128)   NOT NULL,
        [Content]       NVARCHAR(1000)  NOT NULL,
        [GeneratedAt]   DATETIME2       NOT NULL DEFAULT GETDATE(),
        [IsUseful]      BIT             NULL,
        [RatedAt]       DATETIME2       NULL,
        CONSTRAINT [PK_AITips] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_AITips_AspNetUsers] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    )

    CREATE NONCLUSTERED INDEX [IX_AITips_User_GeneratedAt]
        ON [dbo].[AITips] ([UserId] ASC, [GeneratedAt] ASC)
END
GO

PRINT '============================================================'
PRINT 'Base de datos y tablas creadas exitosamente.'
PRINT '============================================================'
GO
