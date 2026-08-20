-- ============================================================
-- Script 4: Triggers (Auditoría de eliminados + 1 extra)
-- Esquema: eliminados
-- Tablas: Expenses, SavingsGoals, AITips
-- ============================================================

USE [FinanzAppDB]
GO

-- ============================================================
-- TABLAS DE AUDITORÍA en esquema [eliminados]
-- ============================================================

-- Tabla para gastos eliminados
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eliminados].[Expenses]') AND type = 'U')
BEGIN
    CREATE TABLE [eliminados].[Expenses] (
        [OriginalId]    INT             NOT NULL,
        [UserId]        NVARCHAR(128)   NOT NULL,
        [Amount]        DECIMAL(18,2)   NOT NULL,
        [Category]      INT             NOT NULL,
        [Date]          DATETIME2       NOT NULL,
        [Note]          NVARCHAR(500)   NULL,
        [CreatedAt]     DATETIME2       NOT NULL,
        [DeletedAt]     DATETIME2       NOT NULL DEFAULT GETDATE(),
        [DeletedBy]     NVARCHAR(128)   NULL
    )
END
GO

-- Tabla para metas de ahorro eliminadas
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eliminados].[SavingsGoals]') AND type = 'U')
BEGIN
    CREATE TABLE [eliminados].[SavingsGoals] (
        [OriginalId]    INT             NOT NULL,
        [UserId]        NVARCHAR(128)   NOT NULL,
        [Name]          NVARCHAR(100)   NOT NULL,
        [TargetAmount]  DECIMAL(18,2)   NOT NULL,
        [Deadline]      DATETIME2       NOT NULL,
        [CreatedAt]     DATETIME2       NOT NULL,
        [IsCompleted]   BIT             NOT NULL,
        [DeletedAt]     DATETIME2       NOT NULL DEFAULT GETDATE(),
        [DeletedBy]     NVARCHAR(128)   NULL
    )
END
GO

-- Tabla para consejos de IA eliminados
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eliminados].[AITips]') AND type = 'U')
BEGIN
    CREATE TABLE [eliminados].[AITips] (
        [OriginalId]    INT             NOT NULL,
        [UserId]        NVARCHAR(128)   NOT NULL,
        [Content]       NVARCHAR(1000)  NOT NULL,
        [GeneratedAt]   DATETIME2       NOT NULL,
        [IsUseful]      BIT             NULL,
        [RatedAt]       DATETIME2       NULL,
        [DeletedAt]     DATETIME2       NOT NULL DEFAULT GETDATE(),
        [DeletedBy]     NVARCHAR(128)   NULL
    )
END
GO

-- ============================================================
-- TRIGGER 1: Registrar gastos eliminados
-- ============================================================
IF OBJECT_ID('trg_Expenses_AuditDelete', 'TR') IS NOT NULL
    DROP TRIGGER trg_Expenses_AuditDelete
GO
CREATE TRIGGER trg_Expenses_AuditDelete
ON Expenses
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [eliminados].[Expenses]
        (OriginalId, UserId, Amount, Category, Date, Note, CreatedAt, DeletedAt, DeletedBy)
    SELECT
        d.Id, d.UserId, d.Amount, d.Category, d.Date, d.Note, d.CreatedAt,
        GETDATE(), SYSTEM_USER
    FROM deleted d
END
GO

-- ============================================================
-- TRIGGER 2: Registrar metas de ahorro eliminadas
-- ============================================================
IF OBJECT_ID('trg_SavingsGoals_AuditDelete', 'TR') IS NOT NULL
    DROP TRIGGER trg_SavingsGoals_AuditDelete
GO
CREATE TRIGGER trg_SavingsGoals_AuditDelete
ON SavingsGoals
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [eliminados].[SavingsGoals]
        (OriginalId, UserId, Name, TargetAmount, Deadline, CreatedAt, IsCompleted, DeletedAt, DeletedBy)
    SELECT
        d.Id, d.UserId, d.Name, d.TargetAmount, d.Deadline, d.CreatedAt, d.IsCompleted,
        GETDATE(), SYSTEM_USER
    FROM deleted d
END
GO

-- ============================================================
-- TRIGGER 3: Registrar consejos de IA eliminados
-- ============================================================
IF OBJECT_ID('trg_AITips_AuditDelete', 'TR') IS NOT NULL
    DROP TRIGGER trg_AITips_AuditDelete
GO
CREATE TRIGGER trg_AITips_AuditDelete
ON AITips
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [eliminados].[AITips]
        (OriginalId, UserId, Content, GeneratedAt, IsUseful, RatedAt, DeletedAt, DeletedBy)
    SELECT
        d.Id, d.UserId, d.Content, d.GeneratedAt, d.IsUseful, d.RatedAt,
        GETDATE(), SYSTEM_USER
    FROM deleted d
END
GO

-- ============================================================
-- TRIGGER 4: Auto-completar meta cuando se alcanza el monto
-- (Propósito diferente a registrar eliminados)
-- ============================================================
IF OBJECT_ID('trg_SavingsGoals_AutoComplete', 'TR') IS NOT NULL
    DROP TRIGGER trg_SavingsGoals_AutoComplete
GO
CREATE TRIGGER trg_SavingsGoals_AutoComplete
ON SavingsEntries
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE sg
    SET sg.IsCompleted = 1
    FROM SavingsGoals sg
    INNER JOIN (
        SELECT i.GoalId, SUM(se.Amount) AS TotalAhorrado
        FROM inserted i
        INNER JOIN SavingsEntries se ON i.GoalId = se.GoalId
        GROUP BY i.GoalId
    ) totals ON sg.Id = totals.GoalId
    WHERE sg.IsCompleted = 0
      AND totals.TotalAhorrado >= sg.TargetAmount
END
GO

PRINT '============================================================'
PRINT 'Triggers creados exitosamente.'
PRINT '  - trg_Expenses_AuditDelete (gastos eliminados → eliminados.Expenses)'
PRINT '  - trg_SavingsGoals_AuditDelete (metas eliminadas → eliminados.SavingsGoals)'
PRINT '  - trg_AITips_AuditDelete (consejos eliminados → eliminados.AITips)'
PRINT '  - trg_SavingsGoals_AutoComplete (auto-completar meta al alcanzar monto)'
PRINT '============================================================'
GO
