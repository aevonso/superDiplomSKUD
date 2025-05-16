using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Divisions_DivisionId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "DivisionId",
                table: "Rooms",
                newName: "FloorId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_DivisionId",
                table: "Rooms",
                newName: "IX_Rooms_FloorId");

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_DivisionId",
                table: "Posts",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Divisions_DivisionId",
                table: "Posts",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Floors_FloorId",
                table: "Rooms",
                column: "FloorId",
                principalTable: "Floors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Divisions_DivisionId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Floors_FloorId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Floors");

            migrationBuilder.DropIndex(
                name: "IX_Posts_DivisionId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "FloorId",
                table: "Rooms",
                newName: "DivisionId");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_FloorId",
                table: "Rooms",
                newName: "IX_Rooms_DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Divisions_DivisionId",
                table: "Rooms",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
