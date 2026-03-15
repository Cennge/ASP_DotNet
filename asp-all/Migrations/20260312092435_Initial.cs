using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace asp_all.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateLevel = table.Column<int>(type: "int", nullable: false),
                    ReadLevel = table.Column<int>(type: "int", nullable: false),
                    UpdateLevel = table.Column<int>(type: "int", nullable: false),
                    DeleteLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsersData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birthdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccesses_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccesses_UsersData_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreateLevel", "DeleteLevel", "Description", "Name", "ReadLevel", "UpdateLevel" },
                values: new object[,]
                {
                    { new Guid("56d473ba-ed6b-4695-aebf-439e2102f2c3"), -1, -1, "Користувач з максимальним доступом, через якого вводяться інші ролі та доступи", "Root Administrator", -1, -1 },
                    { new Guid("bc84c3aa-f62f-44c6-b822-ae954f450a53"), 0, 0, "Користувачі, що самі зареєструвались на сайті. Мінімальні права доступу", "Self Registered", 0, 0 }
                });

            migrationBuilder.InsertData(
                table: "UsersData",
                columns: new[] { "Id", "Birthdate", "DeletedAt", "Email", "Name" },
                values: new object[] { new Guid("41e5ed40-ab13-4b5c-b1d0-3722023ea5c7"), new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "admin@change.me", "Default Administrator" });

            migrationBuilder.InsertData(
                table: "UserAccesses",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Dk", "Login", "Salt", "UserId", "UserRoleId" },
                values: new object[] { new Guid("f0e98ef0-917f-4bf7-90e9-cba9bbd86c04"), new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "BA8C615C65B7A66750C12605DF7602FDB037A12C", "DefaultAdministrator", "4009BA69-7EFC-4E4F-A9AF-FEC77B759BC6", new Guid("41e5ed40-ab13-4b5c-b1d0-3722023ea5c7"), new Guid("56d473ba-ed6b-4695-aebf-439e2102f2c3") });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_Login",
                table: "UserAccesses",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_UserId",
                table: "UserAccesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_UserRoleId",
                table: "UserAccesses",
                column: "UserRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAccesses");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UsersData");
        }
    }
}
