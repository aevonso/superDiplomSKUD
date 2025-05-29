using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessAttempts_PointOfPassages_PointOfPassageId",
                table: "AccessAttempts");

            migrationBuilder.AlterColumn<int>(
                name: "PointOfPassageId",
                table: "AccessAttempts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessAttempts_PointOfPassages_PointOfPassageId",
                table: "AccessAttempts",
                column: "PointOfPassageId",
                principalTable: "PointOfPassages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessAttempts_PointOfPassages_PointOfPassageId",
                table: "AccessAttempts");

            migrationBuilder.AlterColumn<int>(
                name: "PointOfPassageId",
                table: "AccessAttempts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessAttempts_PointOfPassages_PointOfPassageId",
                table: "AccessAttempts",
                column: "PointOfPassageId",
                principalTable: "PointOfPassages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
