using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job_Web.Migrations
{
    /// <inheritdoc />
    public partial class newidd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AspNetUsers_EmployerId",
                table: "Jobs");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_AspNetUsers_EmployerId",
                table: "Jobs",
                column: "EmployerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AspNetUsers_EmployerId",
                table: "Jobs");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_AspNetUsers_EmployerId",
                table: "Jobs",
                column: "EmployerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
