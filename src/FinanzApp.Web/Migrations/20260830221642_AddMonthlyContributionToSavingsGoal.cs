using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyContributionToSavingsGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyContribution",
                table: "SavingsGoals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyContribution",
                table: "SavingsGoals");
        }
    }
}
