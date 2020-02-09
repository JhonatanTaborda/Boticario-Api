using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Boticario.Api.Migrations
{
    public partial class logger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_AspNetUsers_IdApplicationUser",
                table: "Sales");

            migrationBuilder.AlterColumn<string>(
                name: "IdApplicationUser",
                table: "Sales",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(nullable: true),
                    LogLevel = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_AspNetUsers_IdApplicationUser",
                table: "Sales",
                column: "IdApplicationUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_AspNetUsers_IdApplicationUser",
                table: "Sales");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.AlterColumn<string>(
                name: "IdApplicationUser",
                table: "Sales",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_AspNetUsers_IdApplicationUser",
                table: "Sales",
                column: "IdApplicationUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
