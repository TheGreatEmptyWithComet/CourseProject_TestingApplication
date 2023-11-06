using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingServerApp.Migrations
{
    /// <inheritdoc />
    public partial class imagetypechanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Answers");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Tests",
                type: "TEXT",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Questions",
                type: "TEXT",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Answers",
                type: "TEXT",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_Name",
                table: "UserGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestCategories_Name",
                table: "TestCategories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Login",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserGroups_Name",
                table: "UserGroups");

            migrationBuilder.DropIndex(
                name: "IX_TestCategories_Name",
                table: "TestCategories");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Answers");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Tests",
                type: "BLOB",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Questions",
                type: "BLOB",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Answers",
                type: "BLOB",
                maxLength: 1024,
                nullable: true);
        }
    }
}
