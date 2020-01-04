using System.Collections;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogBasePresenter.Migrations
{
    public partial class ForTests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<BitArray>(
                name: "Mask",
                table: "Subnet",
                type: "bit(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<BitArray>(
                name: "IpBit",
                table: "LogRecords",
                type: "bit(32)",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mask",
                table: "Subnet");

            migrationBuilder.DropColumn(
                name: "IpBit",
                table: "LogRecords");
        }
    }
}
