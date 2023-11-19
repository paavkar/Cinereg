using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cinereg.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "638c9ad1-ef81-4476-a1b1-9a77bdff8e4b", "53a1fb4c-de68-4c02-97c1-7bfd7d829ede", "User", "USER" },
                    { "e2985af6-1797-446f-b5d8-1e237c0a956c", "bc2ab4e6-09fb-45dd-b868-1b16e51efb67", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "638c9ad1-ef81-4476-a1b1-9a77bdff8e4b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e2985af6-1797-446f-b5d8-1e237c0a956c");
        }
    }
}
