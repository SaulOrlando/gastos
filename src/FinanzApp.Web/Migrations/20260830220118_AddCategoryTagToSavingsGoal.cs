using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryTagToSavingsGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryTag",
                table: "SavingsGoals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryTag",
                table: "SavingsGoals");
        }
    }
}
