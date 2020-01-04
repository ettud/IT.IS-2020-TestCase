using System.Collections;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogBasePresenter.Migrations
{
    public partial class SubnetToBits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<BitArray>(
                name: "Broadcast",
                table: "Subnet",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<BitArray>(
                name: "Network",
                table: "Subnet",
                maxLength: 32,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Broadcast",
                table: "Subnet");

            migrationBuilder.DropColumn(
                name: "Network",
                table: "Subnet");
        }
    }
}
