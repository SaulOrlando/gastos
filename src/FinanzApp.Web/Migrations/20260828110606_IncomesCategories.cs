using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class IncomesCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Incomes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "IncomeCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeCategories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomeCategory_User_Name",
                table: "IncomeCategories",
                columns: new[] { "UserId", "Name" });

            migrationBuilder.Sql(@"
UPDATE [Incomes]
SET [Category] = CASE [Category]
    WHEN '0' THEN 'Beca'
    WHEN '1' THEN 'Mesada'
    WHEN '2' THEN 'Salario'
    WHEN '3' THEN 'Otras'
    ELSE 'Otras'
END;");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [IncomeCategories] WHERE [Name] = 'Beca' AND [UserId] IS NULL)
    INSERT INTO [IncomeCategories] ([UserId], [Name], [IsSystem], [CreatedAt]) VALUES (NULL, 'Beca', 1, SYSUTCDATETIME());
IF NOT EXISTS (SELECT 1 FROM [IncomeCategories] WHERE [Name] = 'Mesada' AND [UserId] IS NULL)
    INSERT INTO [IncomeCategories] ([UserId], [Name], [IsSystem], [CreatedAt]) VALUES (NULL, 'Mesada', 1, SYSUTCDATETIME());
IF NOT EXISTS (SELECT 1 FROM [IncomeCategories] WHERE [Name] = 'Salario' AND [UserId] IS NULL)
    INSERT INTO [IncomeCategories] ([UserId], [Name], [IsSystem], [CreatedAt]) VALUES (NULL, 'Salario', 1, SYSUTCDATETIME());
IF NOT EXISTS (SELECT 1 FROM [IncomeCategories] WHERE [Name] = 'Otras' AND [UserId] IS NULL)
    INSERT INTO [IncomeCategories] ([UserId], [Name], [IsSystem], [CreatedAt]) VALUES (NULL, 'Otras', 1, SYSUTCDATETIME());");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomeCategories");

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Incomes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
