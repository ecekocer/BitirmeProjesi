using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitirmeProjesi.Migrations
{
    /// <inheritdoc />
    public partial class AddYearField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "PollutionDatas",
                type: "float(53)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "PollutionDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "PollutionDatas",
                type: "float(53)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "PollutionDatas",
                type: "float(53)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "PollutionDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "PollutionDatas",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(53)");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "PollutionDatas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "PollutionDatas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(53)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "PollutionDatas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float(53)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "PollutionDatas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
