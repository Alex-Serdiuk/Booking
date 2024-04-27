using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingServer.Migrations
{
    public partial class ChangeCheapestPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheapestPrice",
                table: "Hotels");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CheapestPrice",
                table: "Hotels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
