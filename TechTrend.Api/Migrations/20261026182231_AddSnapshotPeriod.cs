using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechTrend.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSnapshotPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrendSnapshots_TechnologyId_CollectedAt",
                table: "TrendSnapshots");

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "TrendSnapshots",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TrendSnapshots_TechnologyId_Period_CollectedAt",
                table: "TrendSnapshots",
                columns: new[] { "TechnologyId", "Period", "CollectedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrendSnapshots_TechnologyId_Period_CollectedAt",
                table: "TrendSnapshots");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "TrendSnapshots");

            migrationBuilder.CreateIndex(
                name: "IX_TrendSnapshots_TechnologyId_CollectedAt",
                table: "TrendSnapshots",
                columns: new[] { "TechnologyId", "CollectedAt" });
        }
    }
}
