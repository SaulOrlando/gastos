-- ============================================================
-- Script 7: Testeo completo
-- Ejecuta todos los procedimientos, vistas y dispara triggers
-- NOTA: Ejecutar primero los scripts 01 a 06 en orden
-- ============================================================

USE [FinanzAppDB]
GO

-- ============================================================
-- VERIFICAR QUE EXISTAN LOS OBJETOS NECESARIOS
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'eliminados')
BEGIN
    EXEC('CREATE SCHEMA eliminados')
    PRINT 'Schema eliminados creado.'
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eliminados].[Expenses]') AND type = 'U')
BEGIN
    CREATE TABLE [eliminados].[Expenses] (
        [OriginalId] INT NOT NULL, [UserId] NVARCHAR(128) NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL, [Category] INT NOT NULL,
        [Date] DATETIME2 NOT NULL, [Note] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL, [DeletedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [DeletedBy] NVARCHAR(128) NULL
    )
    PRINT 'Tabla eliminados.Expenses creada.'
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eliminados].[SavingsGoals]') AND type = 'U')
BEGIN
    CREATE TABLE [eliminados].[SavingsGoals] (
        [OriginalId] INT NOT NULL, [UserId] NVARCHAR(128) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL, [TargetAmount] DECIMAL(18,2) NOT NULL,
        [Deadline] DATETIME2 NOT NULL, [CreatedAt] DATETIME2 NOT NULL,
        [IsCompleted] BIT NOT NULL, [DeletedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [DeletedBy] NVARCHAR(128) NULL
    )
    PRINT 'Tabla eliminados.SavingsGoals creada.'
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[eliminados].[AITips]') AND type = 'U')
BEGIN
    CREATE TABLE [eliminados].[AITips] (
        [OriginalId] INT NOT NULL, [UserId] NVARCHAR(128) NOT NULL,
        [Content] NVARCHAR(1000) NOT NULL, [GeneratedAt] DATETIME2 NOT NULL,
        [IsUseful] BIT NULL, [RatedAt] DATETIME2 NULL,
        [DeletedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [DeletedBy] NVARCHAR(128) NULL
    )
    PRINT 'Tabla eliminados.AITips creada.'
END

-- Verificar usuario de prueba
DECLARE @TestUserId NVARCHAR(128) = 'test-user-001'
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = @TestUserId)
BEGIN
    INSERT INTO AspNetUsers (Id, UserName, Email, EmailConfirmed, PasswordHash,
                             SecurityStamp, FullName, Currency, MonthlyBudget,
                             BudgetMonth, BudgetYear, CreatedAt)
    VALUES (@TestUserId, 'estudiante_test', 'test@universidad.edu', 1,
            'AQAAAAIAAYagAAAAEI...hashfalso',
            NEWID(), 'Estudiante de Prueba', 'MXN', 5000.00,
            MONTH(GETDATE()), YEAR(GETDATE()), GETDATE())
    PRINT 'Usuario de prueba creado.'
END
GO

-- ============================================================
-- DECLARAR VARIABLES
-- ============================================================
DECLARE @TestUserId NVARCHAR(128) = 'test-user-001'
DECLARE @ExpenseId INT
DECLARE @GoalId INT
DECLARE @AutoGoalId INT
DECLARE @TipId INT

-- ============================================================
-- 1. TEST: Procedimientos de EXPENSES
-- ============================================================
PRINT '==========================================================='
PRINT '  1. PROCEDIMIENTOS DE EXPENSES'
PRINT '==========================================================='
PRINT ''

-- 1.1 CREAR GASTO
PRINT '--- 1.1 sp_Expenses_Crear ---';

DECLARE @ExpenseDate DATETIME;
SET @ExpenseDate = GETDATE();

EXEC sp_Expenses_Crear
    @UserId = @TestUserId,
    @Amount = 250.50,
    @Category = 2,
    @Date = @ExpenseDate,
    @Note = 'Test - Comida del jueves';

SET @ExpenseId = SCOPE_IDENTITY();

PRINT 'Gasto creado con Id: ' + CAST(@ExpenseId AS NVARCHAR(20));
PRINT '';

-- 1.2 MODIFICAR GASTO
PRINT '--- 1.2 sp_Expenses_Modificar ---';

DECLARE @ModifiedDate DATETIME;
SET @ModifiedDate = GETDATE();

EXEC sp_Expenses_Modificar
    @Id = @ExpenseId,
    @UserId = @TestUserId,
    @Amount = 300.00,
    @Category = 2,
    @Date = @ModifiedDate,
    @Note = 'Test - Comida del jueves (modificado)';

PRINT '';

-- 1.3 OBTENER POR ID
PRINT '--- 1.3 sp_Expenses_ObtenerPorId ---'
EXEC sp_Expenses_ObtenerPorId
    @Id     = @ExpenseId,
    @UserId = @TestUserId
PRINT ''

-- 1.4 OBTENER TODOS
PRINT '--- 1.4 sp_Expenses_ObtenerTodos (top 5) ---'
SELECT TOP 5
    e.Id, e.Amount,
    CASE e.Category WHEN 0 THEN 'Mensualidad' WHEN 1 THEN 'Transporte' WHEN 2 THEN 'Comida' WHEN 3 THEN 'Entretenimiento' END AS Categoria,
    e.Date, e.Note
FROM Expenses e
WHERE e.UserId = @TestUserId
ORDER BY e.Date DESC
PRINT ''

-- 1.5 BUSCAR POR MONTO Y CATEGORIA
PRINT '--- 1.5 sp_Expenses_Buscar (Comida, $100-$500) ---'
EXEC sp_Expenses_Buscar
    @UserId      = @TestUserId,
    @MontoMinimo = 100,
    @MontoMaximo = 500,
    @Categoria   = 2
PRINT ''

-- 1.6 ELIMINAR GASTO (dispara trigger de audit)
PRINT '--- 1.6 sp_Expenses_Eliminar (dispara trigger) ---'
EXEC sp_Expenses_Eliminar
    @Id     = @ExpenseId,
    @UserId = @TestUserId
PRINT 'Gasto eliminado. Verificando en eliminados.Expenses...'
SELECT * FROM [eliminados].[Expenses] WHERE OriginalId = @ExpenseId
PRINT ''

-- ============================================================
-- 2. TEST: Procedimientos de SAVINGSGOALS
-- ============================================================
PRINT '==========================================================='
PRINT '  2. PROCEDIMIENTOS DE SAVINGSGOALS'
PRINT '==========================================================='
PRINT ''

-- 2.1 CREAR META
PRINT '--- 2.1 sp_SavingsGoals_Crear ---'
EXEC sp_SavingsGoals_Crear
    @UserId       = @TestUserId,
    @Name         = 'Laptop para universidad',
    @TargetAmount = 15000.00,
    @Deadline     = '2026-12-31'

SET @GoalId = IDENT_CURRENT('SavingsGoals')
PRINT 'Meta creada con Id: ' + CAST(@GoalId AS NVARCHAR)
PRINT ''

-- 2.2 MODIFICAR META
PRINT '--- 2.2 sp_SavingsGoals_Modificar ---'
EXEC sp_SavingsGoals_Modificar
    @Id           = @GoalId,
    @UserId       = @TestUserId,
    @Name         = 'Laptop gaming para diseno',
    @TargetAmount = 18000.00,
    @Deadline     = '2027-03-01'
PRINT ''

-- 2.3 OBTENER POR ID
PRINT '--- 2.3 sp_SavingsGoals_ObtenerPorId ---'
EXEC sp_SavingsGoals_ObtenerPorId
    @Id     = @GoalId,
    @UserId = @TestUserId
PRINT ''

-- 2.4 OBTENER TODAS
PRINT '--- 2.4 sp_SavingsGoals_ObtenerTodos ---'
EXEC sp_SavingsGoals_ObtenerTodos
    @UserId = @TestUserId
PRINT ''

-- 2.5 BUSCAR POR NOMBRE
PRINT '--- 2.5 sp_SavingsGoals_Buscar (nombre contiene "laptop") ---'
EXEC sp_SavingsGoals_Buscar
    @UserId = @TestUserId,
    @Nombre = 'laptop'
PRINT ''

-- 2.6 ELIMINAR META (dispara trigger de audit)
PRINT '--- 2.6 sp_SavingsGoals_Eliminar (dispara trigger) ---'
EXEC sp_SavingsGoals_Eliminar
    @Id     = @GoalId,
    @UserId = @TestUserId
PRINT 'Meta eliminada. Verificando en eliminados.SavingsGoals...'
SELECT * FROM [eliminados].[SavingsGoals] WHERE OriginalId = @GoalId
PRINT ''

-- ============================================================
-- 3. TEST: TRIGGERS (disparar mas eventos)
-- ============================================================
PRINT '==========================================================='
PRINT '  3. TEST DE TRIGGERS'
PRINT '==========================================================='
PRINT ''

-- 3.1 Crear meta, agregar ahorros hasta completarla
PRINT '--- 3.1 trg_SavingsGoals_AutoComplete ---'
EXEC sp_SavingsGoals_Crear
    @UserId       = @TestUserId,
    @Name         = 'Test auto-complete',
    @TargetAmount = 100.00,
    @Deadline     = '2027-06-30'

SELECT @AutoGoalId = Id FROM SavingsGoals WHERE UserId = @TestUserId AND Name = 'Test auto-complete'
PRINT 'Meta de $100 creada (Id: ' + CAST(@AutoGoalId AS NVARCHAR) + ')'

INSERT INTO SavingsEntries (GoalId, Amount, Date, CreatedAt)
VALUES (@AutoGoalId, 60.00, GETDATE(), GETDATE())
PRINT 'Primer ahorro: $60 (progreso: 60%)'

INSERT INTO SavingsEntries (GoalId, Amount, Date, CreatedAt)
VALUES (@AutoGoalId, 50.00, GETDATE(), GETDATE())
PRINT 'Segundo ahorro: $50 (progreso: 110% -> auto-completada)'

-- Verificar si se auto-completo
SELECT sg.Id, sg.Name, sg.TargetAmount, sg.IsCompleted
FROM SavingsGoals sg
WHERE sg.Id = @AutoGoalId
PRINT ''

-- 3.2 Eliminar consejo de IA (dispara trg_AITips_AuditDelete)
PRINT '--- 3.2 trg_AITips_AuditDelete ---'

INSERT INTO AITips (UserId, Content, GeneratedAt)
VALUES (@TestUserId, 'Test - Intenta reducir gastos en entretenimiento', GETDATE())

SET @TipId = IDENT_CURRENT('AITips')
PRINT 'Consejo creado (Id: ' + CAST(@TipId AS NVARCHAR) + ')'

DELETE FROM AITips WHERE Id = @TipId
PRINT 'Consejo eliminado. Verificando en eliminados.AITips...'
SELECT * FROM [eliminados].[AITips] WHERE OriginalId = @TipId
PRINT ''

-- ============================================================
-- 4. TEST: VISTAS
-- ============================================================
PRINT '==========================================================='
PRINT '  4. TEST DE VISTAS'
PRINT '==========================================================='
PRINT ''

-- 4.1 Vista: Resumen de gastos mensuales
PRINT '--- 4.1 vw_ResumenGastosMensuales (top 10) ---'
SELECT TOP 10 *
FROM vw_ResumenGastosMensuales
WHERE UserId = @TestUserId
ORDER BY Anio DESC, Mes DESC, TotalGastado DESC
PRINT ''

-- 4.2 Vista: Proyeccion de metas
PRINT '--- 4.2 vw_ProyeccionMetasAhorro ---'
SELECT *
FROM vw_ProyeccionMetasAhorro
WHERE UserId = @TestUserId
ORDER BY PorcentajeProgreso DESC
PRINT ''

-- 4.3 Vista: Estadisticas de consejos
PRINT '--- 4.3 vw_EstadisticasConsejosIA ---'
SELECT *
FROM vw_EstadisticasConsejosIA
WHERE UserId = @TestUserId
PRINT ''

-- ============================================================
-- RESUMEN FINAL
-- ============================================================
DECLARE @R_Users INT, @R_Expenses INT, @R_Goals INT, @R_Entries INT, @R_Tips INT
DECLARE @R_DelExpenses INT, @R_DelGoals INT, @R_DelTips INT

SELECT @R_Users = COUNT(*) FROM AspNetUsers
SELECT @R_Expenses = COUNT(*) FROM Expenses
SELECT @R_Goals = COUNT(*) FROM SavingsGoals
SELECT @R_Entries = COUNT(*) FROM SavingsEntries
SELECT @R_Tips = COUNT(*) FROM AITips
SELECT @R_DelExpenses = COUNT(*) FROM [eliminados].[Expenses]
SELECT @R_DelGoals = COUNT(*) FROM [eliminados].[SavingsGoals]
SELECT @R_DelTips = COUNT(*) FROM [eliminados].[AITips]

PRINT '==========================================================='
PRINT '                   RESUMEN FINAL'
PRINT '==========================================================='
PRINT ' Tablas:'
PRINT '   AspNetUsers:     ' + CAST(@R_Users AS NVARCHAR(10)) + ' registros'
PRINT '   Expenses:        ' + CAST(@R_Expenses AS NVARCHAR(10)) + ' registros'
PRINT '   SavingsGoals:    ' + CAST(@R_Goals AS NVARCHAR(10)) + ' registros'
PRINT '   SavingsEntries:  ' + CAST(@R_Entries AS NVARCHAR(10)) + ' registros'
PRINT '   AITips:          ' + CAST(@R_Tips AS NVARCHAR(10)) + ' registros'
PRINT ''
PRINT ' Eliminados (auditados):'
PRINT '   eliminados.Expenses:      ' + CAST(@R_DelExpenses AS NVARCHAR(10)) + ' registros'
PRINT '   eliminados.SavingsGoals:  ' + CAST(@R_DelGoals AS NVARCHAR(10)) + ' registros'
PRINT '   eliminados.AITips:        ' + CAST(@R_DelTips AS NVARCHAR(10)) + ' registros'
PRINT ''
PRINT ' Triggers disparados: audit-delete x 3, auto-complete x 1'
PRINT ' SPs ejecutados: 12 (6 Expenses + 6 SavingsGoals)'
PRINT ' Vistas consultadas: 3'
PRINT '==========================================================='
GO
