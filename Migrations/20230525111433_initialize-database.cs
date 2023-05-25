using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatApp.Migrations
{
    public partial class initializedatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7897c3c9-a6d0-4c24-9e87-a19ea2faab8c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b2439408-d003-4266-95d6-20fda3753128");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "09553870-7d5d-496b-a295-b055f842f56a", "09553870-7d5d-496b-a295-b055f842f56a", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6933684e-b8ea-48df-afd4-ded15cdd78ff", "6933684e-b8ea-48df-afd4-ded15cdd78ff", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "09553870-7d5d-496b-a295-b055f842f56a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6933684e-b8ea-48df-afd4-ded15cdd78ff");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b2439408-d003-4266-95d6-20fda3753128", "b2439408-d003-4266-95d6-20fda3753128", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7897c3c9-a6d0-4c24-9e87-a19ea2faab8c", "7897c3c9-a6d0-4c24-9e87-a19ea2faab8c", "Admin", "ADMIN" });
        }
    }
}
