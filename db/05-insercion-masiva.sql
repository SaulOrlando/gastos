-- ============================================================
-- Script 5: Inserción Masiva (10,000 registros × 2 tablas)
-- Usa WHILE para generar datos pseudo-aleatorios
-- Tablas: Expenses (10,000) y SavingsEntries (10,000)
-- ============================================================

USE [FinanzAppDB]
GO

-- ============================================================
-- Crear usuario de prueba si no existe
-- ============================================================
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
    PRINT 'Usuario de prueba creado: ' + @TestUserId
END
ELSE
    PRINT 'Usuario de prueba ya existe: ' + @TestUserId
GO

-- ============================================================
-- INSERTAR 10,000 REGISTROS EN Expenses (USANDO WHILE)
-- ============================================================
DECLARE @Counter INT = 0
DECLARE @MaxRows INT = 10000
DECLARE @UserId NVARCHAR(128) = 'test-user-001'
DECLARE @Amount DECIMAL(18,2)
DECLARE @Category INT
DECLARE @Date DATETIME2
DECLARE @Note NVARCHAR(500)
DECLARE @StartDate DATETIME2 = DATEADD(MONTH, -12, GETDATE())

PRINT 'Iniciando inserción de 10,000 registros en Expenses...'

WHILE @Counter < @MaxRows
BEGIN
    SET @Counter = @Counter + 1

    -- Monto pseudo-aleatorio entre 10 y 2000
    SET @Amount = ROUND(10 + (ABS(CHECKSUM(NEWID())) % 2000) + (ABS(CHECKSUM(NEWID())) % 100) / 100.0, 2)

    -- Categoría: 0-3
    SET @Category = ABS(CHECKSUM(NEWID())) % 4

    -- Fecha aleatoria en los últimos 12 meses
    SET @Date = DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 365, @StartDate)

    -- Notas según categoría
    SET @Note = CASE @Category
        WHEN 0 THEN 'Pago de mensualidad ciclo ' + CAST(1 + (@Counter % 6) AS NVARCHAR)
        WHEN 1 THEN 'Pasaje ' + CASE (@Counter % 3) WHEN 0 THEN 'Metro' WHEN 1 THEN 'Bus' ELSE 'Uber' END
        WHEN 2 THEN CASE (@Counter % 5) WHEN 0 THEN 'Cafeteria' WHEN 1 THEN 'Comida rapida' WHEN 2 THEN 'Restaurante' WHEN 3 THEN 'Supermercado' ELSE 'Snack' END
        WHEN 3 THEN CASE (@Counter % 4) WHEN 0 THEN 'Netflix' WHEN 1 THEN 'Spotify' WHEN 2 THEN 'Cine' ELSE 'Videojuegos' END
    END

    INSERT INTO Expenses (UserId, Amount, Category, Date, Note, CreatedAt)
    VALUES (@UserId, @Amount, @Category, @Date, @Note, GETDATE())
END

DECLARE @CountExpenses INT
SELECT @CountExpenses = COUNT(*) FROM Expenses WHERE UserId = @UserId

PRINT 'Expenses: 10,000 registros insertados exitosamente.'
PRINT 'Total Expenses: ' + CAST(@CountExpenses AS NVARCHAR)
GO

-- ============================================================
-- INSERTAR 10,000 REGISTROS EN SavingsEntries (USANDO WHILE)
-- ============================================================
DECLARE @GoalCounter INT = 0
DECLARE @EntryCounter INT = 0
DECLARE @GoalId INT
DECLARE @UserId NVARCHAR(128) = 'test-user-001'
DECLARE @GoalName NVARCHAR(100)
DECLARE @TargetAmount DECIMAL(18,2)
DECLARE @Deadline DATETIME2
DECLARE @EntryAmount DECIMAL(18,2)
DECLARE @EntryDate DATETIME2
DECLARE @GoalsToCreate INT = 10
DECLARE @EntriesPerGoal INT = 1000  -- 10 metas × 1000 = 10,000

-- Crear 10 metas de ahorro
PRINT 'Creando 10 metas de ahorro...'

WHILE @GoalCounter < @GoalsToCreate
BEGIN
    SET @GoalCounter = @GoalCounter + 1

    SET @GoalName = 'Meta de prueba #' + CAST(@GoalCounter AS NVARCHAR)
    SET @TargetAmount = 5000 + (@GoalCounter * 1000)
    SET @Deadline = DATEADD(MONTH, 3 + @GoalCounter, GETDATE())

    INSERT INTO SavingsGoals (UserId, Name, TargetAmount, Deadline, CreatedAt, IsCompleted)
    VALUES (@UserId, @GoalName, @TargetAmount, @Deadline, GETDATE(), 0)

    SET @GoalId = SCOPE_IDENTITY()
    PRINT '  Meta #' + CAST(@GoalCounter AS NVARCHAR) + ' creada (Id: ' + CAST(@GoalId AS NVARCHAR) + ')'

    -- Insertar 1,000 ahorros por cada meta
    SET @EntryCounter = 0

    WHILE @EntryCounter < @EntriesPerGoal
    BEGIN
        SET @EntryCounter = @EntryCounter + 1

        -- Monto entre 50 y 500
        SET @EntryAmount = ROUND(50 + (ABS(CHECKSUM(NEWID())) % 450) + (ABS(CHECKSUM(NEWID())) % 100) / 100.0, 2)

        -- Fecha aleatoria en los últimos 6 meses
        SET @EntryDate = DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 180, DATEADD(MONTH, -6, GETDATE()))

        INSERT INTO SavingsEntries (GoalId, Amount, Date, CreatedAt)
        VALUES (@GoalId, @EntryAmount, @EntryDate, GETDATE())
    END
END

DECLARE @CountGoals INT
DECLARE @CountEntries INT
SELECT @CountGoals = COUNT(*) FROM SavingsGoals WHERE UserId = @UserId
SELECT @CountEntries = COUNT(*) FROM SavingsEntries

PRINT ''
PRINT 'SavingsEntries: 10,000 registros insertados exitosamente (10 metas × 1,000 ahorros).'
PRINT 'Total SavingsGoals: ' + CAST(@CountGoals AS NVARCHAR)
PRINT 'Total SavingsEntries: ' + CAST(@CountEntries AS NVARCHAR)
PRINT ''
PRINT '============================================================'
PRINT 'Inserción masiva completada.'
PRINT '  Expenses:        10,000 registros'
PRINT '  SavingsEntries:  10,000 registros (10 metas × 1,000)'
PRINT '============================================================'
GO
