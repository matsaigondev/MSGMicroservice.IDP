using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSGMicroservice.IDP.Persistence.Migrations
{
    public partial class UpdateTblUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                schema: "Identity",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_RoleId_Function_Command",
                schema: "Identity",
                table: "Permissions");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "161055e7-25d0-4521-a9ac-a088b2730e83");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "be5d9587-a30a-4fba-a0dd-82d760ee8e8b");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                schema: "Identity",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                schema: "Identity",
                table: "Permissions",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0c3e649d-125b-44ee-a024-5736a529fa86", "5d684db6-6bef-40b3-8147-643ddc465e4b", "Administration", "ADMINISTRATION" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5af0778a-4bdd-4213-8149-490b5149b81b", "92d58b54-6420-451f-9546-bfb5fbc2b36f", "Customer", "CUSTOMER" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId_Function_Command",
                schema: "Identity",
                table: "Permissions",
                columns: new[] { "RoleId", "Function", "Command" },
                unique: true,
                filter: "[Function] IS NOT NULL AND [Command] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                schema: "Identity",
                table: "Permissions",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                schema: "Identity",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_RoleId_Function_Command",
                schema: "Identity",
                table: "Permissions");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "0c3e649d-125b-44ee-a024-5736a529fa86");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "5af0778a-4bdd-4213-8149-490b5149b81b");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                schema: "Identity",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                schema: "Identity",
                table: "Permissions",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "161055e7-25d0-4521-a9ac-a088b2730e83", "028fda1e-e37c-43df-b8f2-d0438cac49c5", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "be5d9587-a30a-4fba-a0dd-82d760ee8e8b", "1fb9c4d8-1df1-4704-88a8-aca081597a68", "Administration", "ADMINISTRATION" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId_Function_Command",
                schema: "Identity",
                table: "Permissions",
                columns: new[] { "RoleId", "Function", "Command" },
                unique: true,
                filter: "[RoleId] IS NOT NULL AND [Function] IS NOT NULL AND [Command] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                schema: "Identity",
                table: "Permissions",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "Roles",
                principalColumn: "Id");
        }
    }
}
