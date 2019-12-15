using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogBasePresenter.Migrations
{
    public partial class LogRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogRecords",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RecordTime = table.Column<DateTime>(nullable: false),
                    Ip = table.Column<IPAddress>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    UrlParameters = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogRecords");
        }
    }
}
