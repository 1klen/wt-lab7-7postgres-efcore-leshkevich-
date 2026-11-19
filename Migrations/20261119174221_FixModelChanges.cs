using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace wt_lab6_minimalapi_leshkevich.Migrations
{
    /// <inheritdoc />
    public partial class FixModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "Brand", "CreatedAt", "IsAvailable", "Model", "PricePerDay" },
                values: new object[,]
                {
                    { 1, "Mercedes-Benz", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "E 320 W211", 90m },
                    { 2, "BMW", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), true, "530i E60", 85m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
