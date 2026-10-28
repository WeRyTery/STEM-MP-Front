using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechTrend.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSnapshotSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrendSnapshots_TechnologyId_Period_CollectedAt",
                table: "TrendSnapshots");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "TrendSnapshots",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TrendSnapshots_TechnologyId_Source_Period_CollectedAt",
                table: "TrendSnapshots",
                columns: new[] { "TechnologyId", "Source", "Period", "CollectedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrendSnapshots_TechnologyId_Source_Period_CollectedAt",
                table: "TrendSnapshots");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "TrendSnapshots");

            migrationBuilder.CreateIndex(
                name: "IX_TrendSnapshots_TechnologyId_Period_CollectedAt",
                table: "TrendSnapshots",
                columns: new[] { "TechnologyId", "Period", "CollectedAt" });
        }
    }
}
