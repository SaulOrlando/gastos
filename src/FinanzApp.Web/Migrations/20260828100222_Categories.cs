using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class Categories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Expenses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Mapea los valores numéricos del enum antiguo (0-3) a nombres legibles.
            migrationBuilder.Sql(@"
                UPDATE [Expenses]
                SET [Category] = CASE [Category]
                    WHEN '0' THEN 'Mensualidad'
                    WHEN '1' THEN 'Transporte'
                    WHEN '2' THEN 'Comida'
                    WHEN '3' THEN 'Entretenimiento'
                    ELSE 'Otro'
                END;
            ");

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseCategories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_User_Name",
                table: "ExpenseCategories",
                columns: new[] { "UserId", "Name" });

            // Categorías del sistema (sin usuario) disponibles para todos.
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] = N'Mensualidad')
                    INSERT INTO [ExpenseCategories] ([UserId], [Name], [Icon], [IsSystem], [CreatedAt])
                    VALUES (NULL, N'Mensualidad', N'bi-mortarboard', 1, SYSUTCDATETIME());

                IF NOT EXISTS (SELECT 1 FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] = N'Transporte')
                    INSERT INTO [ExpenseCategories] ([UserId], [Name], [Icon], [IsSystem], [CreatedAt])
                    VALUES (NULL, N'Transporte', N'bi-bus-front', 1, SYSUTCDATETIME());

                IF NOT EXISTS (SELECT 1 FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] = N'Comida')
                    INSERT INTO [ExpenseCategories] ([UserId], [Name], [Icon], [IsSystem], [CreatedAt])
                    VALUES (NULL, N'Comida', N'bi-utensils', 1, SYSUTCDATETIME());

                IF NOT EXISTS (SELECT 1 FROM [ExpenseCategories] WHERE [UserId] IS NULL AND [Name] = N'Entretenimiento')
                    INSERT INTO [ExpenseCategories] ([UserId], [Name], [Icon], [IsSystem], [CreatedAt])
                    VALUES (NULL, N'Entretenimiento', N'bi-controller', 1, SYSUTCDATETIME());
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Devuelve los nombres a valores numéricos antes de revertir la columna.
            migrationBuilder.Sql(@"
                UPDATE [Expenses]
                SET [Category] = CASE [Category]
                    WHEN 'Mensualidad' THEN '0'
                    WHEN 'Transporte' THEN '1'
                    WHEN 'Comida' THEN '2'
                    WHEN 'Entretenimiento' THEN '3'
                    ELSE '0'
                END;
            ");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Expenses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
