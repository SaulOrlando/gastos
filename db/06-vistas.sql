-- ============================================================
-- Script 6: Vistas (3 vistas para 3 consultas en 3 tablas)
-- ============================================================

USE [FinanzAppDB]
GO

-- ============================================================
-- VISTA 1: Resumen mensual de gastos por categoría
-- Tabla: Expenses
-- Consulta: Total gastado por categoría en cada mes
-- ============================================================
IF OBJECT_ID('vw_ResumenGastosMensuales', 'V') IS NOT NULL
    DROP VIEW vw_ResumenGastosMensuales
GO
CREATE VIEW vw_ResumenGastosMensuales
AS
SELECT
    e.UserId,
    u.FullName AS NombreUsuario,
    u.Currency AS Moneda,
    YEAR(e.Date) AS Anio,
    MONTH(e.Date) AS Mes,
    CASE e.Category
        WHEN 0 THEN 'Mensualidad'
        WHEN 1 THEN 'Transporte'
        WHEN 2 THEN 'Comida'
        WHEN 3 THEN 'Entretenimiento'
    END AS Categoria,
    COUNT(*) AS CantidadTransacciones,
    SUM(e.Amount) AS TotalGastado,
    ROUND(AVG(e.Amount), 2) AS PromedioPorGasto,
    MIN(e.Amount) AS GastoMinimo,
    MAX(e.Amount) AS GastoMaximo,
    u.MonthlyBudget AS PresupuestoMensual,
    CASE
        WHEN u.MonthlyBudget > 0
        THEN ROUND((SUM(e.Amount) / u.MonthlyBudget) * 100, 1)
        ELSE NULL
    END AS PorcentajePresupuesto
FROM Expenses e
INNER JOIN AspNetUsers u ON e.UserId = u.Id
GROUP BY e.UserId, u.FullName, u.Currency, YEAR(e.Date), MONTH(e.Date),
         e.Category, u.MonthlyBudget
GO

-- ============================================================
-- VISTA 2: Progreso de metas de ahorro con proyección
-- Tabla: SavingsGoals + SavingsEntries
-- Consulta: Estado de cada meta con progreso y proyección
-- ============================================================
IF OBJECT_ID('vw_ProyeccionMetasAhorro', 'V') IS NOT NULL
    DROP VIEW vw_ProyeccionMetasAhorro
GO
CREATE VIEW vw_ProyeccionMetasAhorro
AS
SELECT
    sg.UserId,
    u.FullName AS NombreUsuario,
    sg.Id AS MetaId,
    sg.Name AS NombreMeta,
    sg.TargetAmount AS MontoObjetivo,
    sg.Deadline AS FechaLimite,
    ISNULL(total.AhorroTotal, 0) AS TotalAhorrado,
    sg.TargetAmount - ISNULL(total.AhorroTotal, 0) AS MontoFaltante,
    CASE
        WHEN sg.TargetAmount > 0
        THEN ROUND((ISNULL(total.AhorroTotal, 0) / sg.TargetAmount) * 100, 1)
        ELSE 0
    END AS PorcentajeProgreso,
    DATEDIFF(MONTH, GETDATE(), sg.Deadline) AS MesesRestantes,
    CASE
        WHEN DATEDIFF(MONTH, GETDATE(), sg.Deadline) > 0
        THEN ROUND(
            (sg.TargetAmount - ISNULL(total.AhorroTotal, 0)) /
            CAST(DATEDIFF(MONTH, GETDATE(), sg.Deadline) AS DECIMAL(18,2)),
            2
        )
        ELSE 0
    END AS AhorroMensualNecesario,
    CASE
        WHEN sg.IsCompleted = 1 THEN 'Completada'
        WHEN sg.Deadline < GETDATE() THEN 'Vencida'
        WHEN ISNULL(total.AhorroTotal, 0) >= sg.TargetAmount THEN 'Alcanzada'
        ELSE 'En progreso'
    END AS EstadoMeta,
    sg.IsCompleted AS Completada
FROM SavingsGoals sg
INNER JOIN AspNetUsers u ON sg.UserId = u.Id
LEFT JOIN (
    SELECT GoalId, SUM(Amount) AS AhorroTotal, COUNT(*) AS NumAhorros
    FROM SavingsEntries
    GROUP BY GoalId
) total ON sg.Id = total.GoalId
GO

-- ============================================================
-- VISTA 3: Historial completo de ahorros con detalle de meta
-- Tabla: AITips
-- Consulta: Resumen de consejos generados y su aceptación
-- ============================================================
IF OBJECT_ID('vw_EstadisticasConsejosIA', 'V') IS NOT NULL
    DROP VIEW vw_EstadisticasConsejosIA
GO
CREATE VIEW vw_EstadisticasConsejosIA
AS
SELECT
    at.UserId,
    u.FullName AS NombreUsuario,
    u.Currency AS Moneda,
    COUNT(*) AS TotalConsejosGenerados,
    SUM(CASE WHEN at.IsUseful = 1 THEN 1 ELSE 0 END) AS ConsejosUtiles,
    SUM(CASE WHEN at.IsUseful = 0 THEN 1 ELSE 0 END) AS ConsejosNoUtiles,
    SUM(CASE WHEN at.IsUseful IS NULL THEN 1 ELSE 0 END) AS SinCalificar,
    CASE
        WHEN COUNT(*) > 0
        THEN ROUND(
            (CAST(SUM(CASE WHEN at.IsUseful = 1 THEN 1 ELSE 0 END) AS DECIMAL(18,2))
            / COUNT(*)) * 100, 1
        )
        ELSE 0
    END AS PorcentajeAceptacion,
    MIN(at.GeneratedAt) AS PrimerConsejo,
    MAX(at.GeneratedAt) AS UltimoConsejo,
    -- Tasa de respuesta: cuántos han sido calificados
    CASE
        WHEN COUNT(*) > 0
        THEN ROUND(
            (CAST(SUM(CASE WHEN at.IsUseful IS NOT NULL THEN 1 ELSE 0 END) AS DECIMAL(18,2))
            / COUNT(*)) * 100, 1
        )
        ELSE 0
    END AS TasaRespuesta
FROM AITips at
INNER JOIN AspNetUsers u ON at.UserId = u.Id
GROUP BY at.UserId, u.FullName, u.Currency
GO

PRINT '============================================================'
PRINT 'Vistas creadas exitosamente.'
PRINT '  - vw_ResumenGastosMensuales (gastos por categoría/mes)'
PRINT '  - vw_ProyeccionMetasAhorro (progreso y proyección de metas)'
PRINT '  - vw_EstadisticasConsejosIA (estadísticas de consejos IA)'
PRINT '============================================================'
GO
