using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Test.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beneficiaris",
                columns: table => new
                {
                    Benid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Piradobisnomeri = table.Column<string>(nullable: true),
                    Saxeli = table.Column<string>(nullable: true),
                    Gvari = table.Column<string>(nullable: true),
                    Asaki = table.Column<int>(nullable: false),
                    Misamarti = table.Column<string>(nullable: true),
                    Telefoni = table.Column<int>(nullable: false),
                    Tarigi = table.Column<DateTime>(nullable: false),
                    DabTarigi = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaris", x => x.Benid);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    Vsid = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VistisTipi = table.Column<string>(nullable: true),
                    TarigiDro = table.Column<DateTime>(nullable: false),
                    Piradoba = table.Column<string>(nullable: true),
                    Saxeli = table.Column<string>(nullable: true),
                    Gvari = table.Column<string>(nullable: true),
                    Symptomi = table.Column<string>(nullable: true),
                    Currentuser = table.Column<string>(nullable: true),
                    Mdgomareoba = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Vsid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficiaris");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "Visits");
        }
    }
}
