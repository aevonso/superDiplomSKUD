using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Login",
                table: "twoFactorCodes");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "twoFactorCodes",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "twoFactorCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiry",
                table: "twoFactorCodes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "twoFactorCodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_twoFactorCodes_EmployeeId",
                table: "twoFactorCodes",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_twoFactorCodes_Employees_EmployeeId",
                table: "twoFactorCodes",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_twoFactorCodes_Employees_EmployeeId",
                table: "twoFactorCodes");

            migrationBuilder.DropIndex(
                name: "IX_twoFactorCodes_EmployeeId",
                table: "twoFactorCodes");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "twoFactorCodes");

            migrationBuilder.DropColumn(
                name: "Expiry",
                table: "twoFactorCodes");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "twoFactorCodes");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "twoFactorCodes",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "twoFactorCodes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
