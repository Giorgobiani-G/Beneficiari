using Microsoft.EntityFrameworkCore.Migrations;

namespace Test.Migrations
{
    public partial class Init : Migration
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
                    Misamarti = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaris", x => x.Benid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficiaris");
        }
    }
}
