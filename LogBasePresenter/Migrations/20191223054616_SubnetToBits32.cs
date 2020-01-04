using System.Collections;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogBasePresenter.Migrations
{
    public partial class SubnetToBits32 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<BitArray>(
                name: "Network",
                table: "Subnet",
                type: "bit(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(BitArray),
                oldType: "bit varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<BitArray>(
                name: "Broadcast",
                table: "Subnet",
                type: "bit(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(BitArray),
                oldType: "bit varying(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<BitArray>(
                name: "Network",
                table: "Subnet",
                type: "bit varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(BitArray),
                oldType: "bit(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<BitArray>(
                name: "Broadcast",
                table: "Subnet",
                type: "bit varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(BitArray),
                oldType: "bit(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
