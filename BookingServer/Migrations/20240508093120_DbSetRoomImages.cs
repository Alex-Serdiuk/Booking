using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingServer.Migrations
{
    public partial class DbSetRoomImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomImage_Rooms_RoomId",
                table: "RoomImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomImage",
                table: "RoomImage");

            migrationBuilder.RenameTable(
                name: "RoomImage",
                newName: "RoomImages");

            migrationBuilder.RenameIndex(
                name: "IX_RoomImage_RoomId",
                table: "RoomImages",
                newName: "IX_RoomImages_RoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomImages",
                table: "RoomImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomImages_Rooms_RoomId",
                table: "RoomImages",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomImages_Rooms_RoomId",
                table: "RoomImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomImages",
                table: "RoomImages");

            migrationBuilder.RenameTable(
                name: "RoomImages",
                newName: "RoomImage");

            migrationBuilder.RenameIndex(
                name: "IX_RoomImages_RoomId",
                table: "RoomImage",
                newName: "IX_RoomImage_RoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomImage",
                table: "RoomImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomImage_Rooms_RoomId",
                table: "RoomImage",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
