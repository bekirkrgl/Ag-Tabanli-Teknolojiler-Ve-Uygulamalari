using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneRandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Specializations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Specializations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "IsActive" },
                values: new object[] { new DateTime(2025, 10, 17, 16, 37, 10, 138, DateTimeKind.Local).AddTicks(8820), true });

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "IsActive" },
                values: new object[] { new DateTime(2025, 10, 17, 16, 37, 10, 141, DateTimeKind.Local).AddTicks(1684), true });

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "IsActive" },
                values: new object[] { new DateTime(2025, 10, 17, 16, 37, 10, 141, DateTimeKind.Local).AddTicks(1702), true });

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "IsActive" },
                values: new object[] { new DateTime(2025, 10, 17, 16, 37, 10, 141, DateTimeKind.Local).AddTicks(1704), true });

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "IsActive" },
                values: new object[] { new DateTime(2025, 10, 17, 16, 37, 10, 141, DateTimeKind.Local).AddTicks(1705), true });

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "IsActive" },
                values: new object[] { new DateTime(2025, 10, 17, 16, 37, 10, 141, DateTimeKind.Local).AddTicks(1706), true });

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "IsActive" },
                values: new object[] { new DateTime(2025, 10, 17, 16, 37, 10, 141, DateTimeKind.Local).AddTicks(1707), true });

            migrationBuilder.UpdateData(
                table: "Specializations",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "IsActive" },
                values: new object[] { new DateTime(2025, 10, 17, 16, 37, 10, 141, DateTimeKind.Local).AddTicks(1708), true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Specializations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Specializations");
        }
    }
}
