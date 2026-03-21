using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asp_all.Migrations
{
    /// <inheritdoc />
    public partial class AvatarAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarFilename",
                table: "UserAccesses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "UserAccesses",
                keyColumn: "Id",
                keyValue: new Guid("f0e98ef0-917f-4bf7-90e9-cba9bbd86c04"),
                column: "AvatarFilename",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarFilename",
                table: "UserAccesses");
        }
    }
}
