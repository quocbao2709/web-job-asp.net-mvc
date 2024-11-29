using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job_Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIsApproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cập nhật tất cả giá trị NULL hiện tại trong bảng thành FALSE
            migrationBuilder.Sql("UPDATE AspNetUsers SET IsApproved = 0 WHERE IsApproved IS NULL");

            // Thay đổi cột để không cho phép NULL và đặt giá trị mặc định là FALSE
            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "AspNetUsers",
                type: "bit",
                nullable: false, // Không cho phép NULL
                defaultValue: false); // Giá trị mặc định là FALSE
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Khôi phục lại cột để cho phép NULL (rollback)
            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "AspNetUsers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
