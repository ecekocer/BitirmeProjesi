using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitirmeProjesi.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EnteredById",
                table: "PollutionDatas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PollutionDatas_EnteredById",
                table: "PollutionDatas",
                column: "EnteredById");

            migrationBuilder.AddForeignKey(
                name: "FK_PollutionDatas_AspNetUsers_EnteredById",
                table: "PollutionDatas",
                column: "EnteredById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollutionDatas_AspNetUsers_EnteredById",
                table: "PollutionDatas");

            migrationBuilder.DropIndex(
                name: "IX_PollutionDatas_EnteredById",
                table: "PollutionDatas");

            migrationBuilder.AlterColumn<string>(
                name: "EnteredById",
                table: "PollutionDatas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
