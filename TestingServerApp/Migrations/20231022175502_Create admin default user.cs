using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections;
using System;

#nullable disable

namespace TestingServerApp.Migrations
{
    /// <inheritdoc />
    public partial class Createadmindefaultuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create administrator group
            migrationBuilder.Sql("INSERT INTO UserGroups (Name) VALUES ('Administrator')");

            // Create default administrator user with salt and hash that correspond to "admin" password
            byte[] passwordSaltArray = PasswordEncryption.GenerateSaltAsBytes();
            string passwordSaltHexString = Convert.ToHexString(passwordSaltArray);
            string passwordHash = PasswordEncryption.GetPasswordHash("admin", passwordSaltArray);

            migrationBuilder.Sql($"INSERT INTO Users " +
                $"(Login, Email, PasswordSalt, PasswordHash, UserGroupId) " +
                $"VALUES " +
                $"('admin','emailSample@emptyMail.com',X'{passwordSaltHexString}','{passwordHash}',1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Users");
            migrationBuilder.Sql("DELETE FROM UserGroups");
            // Reser identity column
            migrationBuilder.Sql("DELETE FROM sqlite_sequence WHERE name='Users'");
            migrationBuilder.Sql("DELETE FROM sqlite_sequence WHERE name='UserGroups'");
        }
    }
}
