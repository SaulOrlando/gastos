using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AdjustSystemCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] IN (N'Mensualidad', N'Transporte');

                IF NOT EXISTS (SELECT 1 FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] = N'Comida')
                    INSERT INTO [ExpenseCategories] ([UserId], [Name], [Icon], [IsSystem], [CreatedAt])
                    VALUES (NULL, N'Comida', N'bi-tag', 1, SYSUTCDATETIME());

                IF NOT EXISTS (SELECT 1 FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] = N'Entretenimiento')
                    INSERT INTO [ExpenseCategories] ([UserId], [Name], [Icon], [IsSystem], [CreatedAt])
                    VALUES (NULL, N'Entretenimiento', N'bi-tag', 1, SYSUTCDATETIME());

                IF NOT EXISTS (SELECT 1 FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] = N'Otras')
                    INSERT INTO [ExpenseCategories] ([UserId], [Name], [Icon], [IsSystem], [CreatedAt])
                    VALUES (NULL, N'Otras', N'bi-tag', 1, SYSUTCDATETIME());
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] = N'Otras';

                IF NOT EXISTS (SELECT 1 FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] = N'Mensualidad')
                    INSERT INTO [ExpenseCategories] ([UserId], [Name], [Icon], [IsSystem], [CreatedAt])
                    VALUES (NULL, N'Mensualidad', N'bi-mortarboard', 1, SYSUTCDATETIME());

                IF NOT EXISTS (SELECT 1 FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] = N'Transporte')
                    INSERT INTO [ExpenseCategories] ([UserId], [Name], [Icon], [IsSystem], [CreatedAt])
                    VALUES (NULL, N'Transporte', N'bi-bus-front', 1, SYSUTCDATETIME());
                """);
        }
    }
}
