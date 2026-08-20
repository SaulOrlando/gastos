-- ============================================================
-- Script 3: Procedimientos Almacenados (12 SPs)
-- Tablas: Expenses y SavingsGoals
-- Operaciones: CRUD + Buscar
-- ============================================================

USE [FinanzAppDB]
GO

-- ============================================================
-- ================== TABLA: EXPENSES ========================
-- ============================================================

-- 1. CREAR GASTO
IF OBJECT_ID('sp_Expenses_Crear', 'P') IS NOT NULL
    DROP PROCEDURE sp_Expenses_Crear
GO
CREATE PROCEDURE sp_Expenses_Crear
    @UserId     NVARCHAR(128),
    @Amount     DECIMAL(18,2),
    @Category   INT,
    @Date       DATETIME2,
    @Note       NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Amount <= 0
    BEGIN
        RAISERROR('El monto debe ser mayor a cero.', 16, 1)
        RETURN
    END

    IF @Category NOT BETWEEN 0 AND 3
    BEGIN
        RAISERROR('Categoría inválida. Use: 0=Mensualidad, 1=Transporte, 2=Comida, 3=Entretenimiento', 16, 1)
        RETURN
    END

    INSERT INTO Expenses (UserId, Amount, Category, Date, Note, CreatedAt)
    VALUES (@UserId, @Amount, @Category, @Date, @Note, GETDATE())

    SELECT SCOPE_IDENTITY() AS NuevoId
END
GO

-- 2. MODIFICAR GASTO
IF OBJECT_ID('sp_Expenses_Modificar', 'P') IS NOT NULL
    DROP PROCEDURE sp_Expenses_Modificar
GO
CREATE PROCEDURE sp_Expenses_Modificar
    @Id         INT,
    @UserId     NVARCHAR(128),
    @Amount     DECIMAL(18,2),
    @Category   INT,
    @Date       DATETIME2,
    @Note       NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Expenses WHERE Id = @Id AND UserId = @UserId)
    BEGIN
        RAISERROR('Gasto no encontrado o no pertenece al usuario.', 16, 1)
        RETURN
    END

    IF @Amount <= 0
    BEGIN
        RAISERROR('El monto debe ser mayor a cero.', 16, 1)
        RETURN
    END

    UPDATE Expenses
    SET Amount   = @Amount,
        Category = @Category,
        Date     = @Date,
        Note     = @Note,
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND UserId = @UserId

    SELECT @Id AS IdModificado
END
GO

-- 3. ELIMINAR GASTO
IF OBJECT_ID('sp_Expenses_Eliminar', 'P') IS NOT NULL
    DROP PROCEDURE sp_Expenses_Eliminar
GO
CREATE PROCEDURE sp_Expenses_Eliminar
    @Id     INT,
    @UserId NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Expenses WHERE Id = @Id AND UserId = @UserId)
    BEGIN
        RAISERROR('Gasto no encontrado o no pertenece al usuario.', 16, 1)
        RETURN
    END

    DELETE FROM Expenses WHERE Id = @Id AND UserId = @UserId

    SELECT @Id AS IdEliminado
END
GO

-- 4. OBTENER GASTO POR ID
IF OBJECT_ID('sp_Expenses_ObtenerPorId', 'P') IS NOT NULL
    DROP PROCEDURE sp_Expenses_ObtenerPorId
GO
CREATE PROCEDURE sp_Expenses_ObtenerPorId
    @Id     INT,
    @UserId NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT e.Id, e.UserId, e.Amount, e.Category, e.Date, e.Note,
           e.CreatedAt, e.UpdatedAt,
           CASE e.Category
               WHEN 0 THEN 'Mensualidad'
               WHEN 1 THEN 'Transporte'
               WHEN 2 THEN 'Comida'
               WHEN 3 THEN 'Entretenimiento'
           END AS CategoryName
    FROM Expenses e
    WHERE e.Id = @Id AND e.UserId = @UserId
END
GO

-- 5. OBTENER TODOS LOS GASTOS DE UN USUARIO
IF OBJECT_ID('sp_Expenses_ObtenerTodos', 'P') IS NOT NULL
    DROP PROCEDURE sp_Expenses_ObtenerTodos
GO
CREATE PROCEDURE sp_Expenses_ObtenerTodos
    @UserId NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT e.Id, e.Amount, e.Category, e.Date, e.Note,
           e.CreatedAt, e.UpdatedAt,
           CASE e.Category
               WHEN 0 THEN 'Mensualidad'
               WHEN 1 THEN 'Transporte'
               WHEN 2 THEN 'Comida'
               WHEN 3 THEN 'Entretenimiento'
           END AS CategoryName
    FROM Expenses e
    WHERE e.UserId = @UserId
    ORDER BY e.Date DESC
END
GO

-- 6. BUSCAR GASTOS POR MONTO, CATEGORÍA O FECHA
IF OBJECT_ID('sp_Expenses_Buscar', 'P') IS NOT NULL
    DROP PROCEDURE sp_Expenses_Buscar
GO
CREATE PROCEDURE sp_Expenses_Buscar
    @UserId         NVARCHAR(128),
    @MontoMinimo    DECIMAL(18,2) = NULL,
    @MontoMaximo    DECIMAL(18,2) = NULL,
    @Categoria      INT           = NULL,
    @FechaInicio    DATETIME2     = NULL,
    @FechaFin       DATETIME2     = NULL,
    @TextoNota      NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT e.Id, e.Amount, e.Category, e.Date, e.Note,
           CASE e.Category
               WHEN 0 THEN 'Mensualidad'
               WHEN 1 THEN 'Transporte'
               WHEN 2 THEN 'Comida'
               WHEN 3 THEN 'Entretenimiento'
           END AS CategoryName
    FROM Expenses e
    WHERE e.UserId = @UserId
      AND (@MontoMinimo IS NULL OR e.Amount >= @MontoMinimo)
      AND (@MontoMaximo IS NULL OR e.Amount <= @MontoMaximo)
      AND (@Categoria   IS NULL OR e.Category = @Categoria)
      AND (@FechaInicio IS NULL OR e.Date >= @FechaInicio)
      AND (@FechaFin    IS NULL OR e.Date <= @FechaFin)
      AND (@TextoNota   IS NULL OR e.Note LIKE '%' + @TextoNota + '%')
    ORDER BY e.Date DESC
END
GO

-- ============================================================
-- ================ TABLA: SAVINGSGOALS ======================
-- ============================================================

-- 7. CREAR META DE AHORRO
IF OBJECT_ID('sp_SavingsGoals_Crear', 'P') IS NOT NULL
    DROP PROCEDURE sp_SavingsGoals_Crear
GO
CREATE PROCEDURE sp_SavingsGoals_Crear
    @UserId         NVARCHAR(128),
    @Name           NVARCHAR(100),
    @TargetAmount   DECIMAL(18,2),
    @Deadline       DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    IF @TargetAmount <= 0
    BEGIN
        RAISERROR('El monto objetivo debe ser mayor a cero.', 16, 1)
        RETURN
    END

    IF @Deadline <= GETDATE()
    BEGIN
        RAISERROR('La fecha límite debe ser futura.', 16, 1)
        RETURN
    END

    INSERT INTO SavingsGoals (UserId, Name, TargetAmount, Deadline, CreatedAt, IsCompleted)
    VALUES (@UserId, @Name, @TargetAmount, @Deadline, GETDATE(), 0)

    SELECT SCOPE_IDENTITY() AS NuevaMetaId
END
GO

-- 8. MODIFICAR META DE AHORRO
IF OBJECT_ID('sp_SavingsGoals_Modificar', 'P') IS NOT NULL
    DROP PROCEDURE sp_SavingsGoals_Modificar
GO
CREATE PROCEDURE sp_SavingsGoals_Modificar
    @Id             INT,
    @UserId         NVARCHAR(128),
    @Name           NVARCHAR(100),
    @TargetAmount   DECIMAL(18,2),
    @Deadline       DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM SavingsGoals WHERE Id = @Id AND UserId = @UserId)
    BEGIN
        RAISERROR('Meta no encontrada o no pertenece al usuario.', 16, 1)
        RETURN
    END

    IF @TargetAmount <= 0
    BEGIN
        RAISERROR('El monto objetivo debe ser mayor a cero.', 16, 1)
        RETURN
    END

    UPDATE SavingsGoals
    SET Name         = @Name,
        TargetAmount = @TargetAmount,
        Deadline     = @Deadline
    WHERE Id = @Id AND UserId = @UserId

    SELECT @Id AS MetaModificada
END
GO

-- 9. ELIMINAR META DE AHORRO
IF OBJECT_ID('sp_SavingsGoals_Eliminar', 'P') IS NOT NULL
    DROP PROCEDURE sp_SavingsGoals_Eliminar
GO
CREATE PROCEDURE sp_SavingsGoals_Eliminar
    @Id     INT,
    @UserId NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM SavingsGoals WHERE Id = @Id AND UserId = @UserId)
    BEGIN
        RAISERROR('Meta no encontrada o no pertenece al usuario.', 16, 1)
        RETURN
    END

    -- CASCADE DELETE eliminará las SavingsEntries asociadas
    DELETE FROM SavingsGoals WHERE Id = @Id AND UserId = @UserId

    SELECT @Id AS MetaEliminada
END
GO

-- 10. OBTENER META POR ID (con total ahorrado y proyección)
IF OBJECT_ID('sp_SavingsGoals_ObtenerPorId', 'P') IS NOT NULL
    DROP PROCEDURE sp_SavingsGoals_ObtenerPorId
GO
CREATE PROCEDURE sp_SavingsGoals_ObtenerPorId
    @Id     INT,
    @UserId NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sg.Id, sg.UserId, sg.Name, sg.TargetAmount, sg.Deadline,
           sg.CreatedAt, sg.IsCompleted,
           ISNULL(total.AhorroTotal, 0) AS AhorroTotal,
           sg.TargetAmount - ISNULL(total.AhorroTotal, 0) AS Falta,
           CASE
               WHEN sg.TargetAmount > 0
               THEN ROUND((ISNULL(total.AhorroTotal, 0) / sg.TargetAmount) * 100, 1)
               ELSE 0
           END AS PorcentajeProgreso,
           DATEDIFF(MONTH, GETDATE(), sg.Deadline) AS MesesRestantes
    FROM SavingsGoals sg
    LEFT JOIN (
        SELECT GoalId, SUM(Amount) AS AhorroTotal
        FROM SavingsEntries
        GROUP BY GoalId
    ) total ON sg.Id = total.GoalId
    WHERE sg.Id = @Id AND sg.UserId = @UserId
END
GO

-- 11. OBTENER TODAS LAS METAS DE UN USUARIO
IF OBJECT_ID('sp_SavingsGoals_ObtenerTodos', 'P') IS NOT NULL
    DROP PROCEDURE sp_SavingsGoals_ObtenerTodos
GO
CREATE PROCEDURE sp_SavingsGoals_ObtenerTodos
    @UserId NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sg.Id, sg.Name, sg.TargetAmount, sg.Deadline, sg.IsCompleted,
           ISNULL(total.AhorroTotal, 0) AS AhorroTotal,
           CASE
               WHEN sg.TargetAmount > 0
               THEN ROUND((ISNULL(total.AhorroTotal, 0) / sg.TargetAmount) * 100, 1)
               ELSE 0
           END AS PorcentajeProgreso,
           DATEDIFF(MONTH, GETDATE(), sg.Deadline) AS MesesRestantes
    FROM SavingsGoals sg
    LEFT JOIN (
        SELECT GoalId, SUM(Amount) AS AhorroTotal
        FROM SavingsEntries
        GROUP BY GoalId
    ) total ON sg.Id = total.GoalId
    WHERE sg.UserId = @UserId
    ORDER BY sg.Deadline ASC
END
GO

-- 12. BUSCAR METAS POR NOMBRE O MONTO
IF OBJECT_ID('sp_SavingsGoals_Buscar', 'P') IS NOT NULL
    DROP PROCEDURE sp_SavingsGoals_Buscar
GO
CREATE PROCEDURE sp_SavingsGoals_Buscar
    @UserId             NVARCHAR(128),
    @Nombre             NVARCHAR(100) = NULL,
    @MontoMinimo        DECIMAL(18,2) = NULL,
    @MontoMaximo        DECIMAL(18,2) = NULL,
    @SoloCompletadas    BIT           = NULL,
    @FechaLimiteDesde   DATETIME2     = NULL,
    @FechaLimiteHasta   DATETIME2     = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT sg.Id, sg.Name, sg.TargetAmount, sg.Deadline, sg.IsCompleted,
           ISNULL(total.AhorroTotal, 0) AS AhorroTotal,
           CASE
               WHEN sg.TargetAmount > 0
               THEN ROUND((ISNULL(total.AhorroTotal, 0) / sg.TargetAmount) * 100, 1)
               ELSE 0
           END AS PorcentajeProgreso
    FROM SavingsGoals sg
    LEFT JOIN (
        SELECT GoalId, SUM(Amount) AS AhorroTotal
        FROM SavingsEntries
        GROUP BY GoalId
    ) total ON sg.Id = total.GoalId
    WHERE sg.UserId = @UserId
      AND (@Nombre          IS NULL OR sg.Name LIKE '%' + @Nombre + '%')
      AND (@MontoMinimo     IS NULL OR sg.TargetAmount >= @MontoMinimo)
      AND (@MontoMaximo     IS NULL OR sg.TargetAmount <= @MontoMaximo)
      AND (@SoloCompletadas IS NULL OR sg.IsCompleted = @SoloCompletadas)
      AND (@FechaLimiteDesde IS NULL OR sg.Deadline >= @FechaLimiteDesde)
      AND (@FechaLimiteHasta IS NULL OR sg.Deadline <= @FechaLimiteHasta)
    ORDER BY sg.Deadline ASC
END
GO

PRINT '============================================================'
PRINT '12 procedimientos almacenados creados exitosamente.'
PRINT '  Expenses (6): Crear, Modificar, Eliminar, ObtenerPorId, ObtenerTodos, Buscar'
PRINT '  SavingsGoals (6): Crear, Modificar, Eliminar, ObtenerPorId, ObtenerTodos, Buscar'
PRINT '============================================================'
GO
