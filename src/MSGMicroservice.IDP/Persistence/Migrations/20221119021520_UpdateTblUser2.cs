using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSGMicroservice.IDP.Persistence.Migrations
{
    public partial class UpdateTblUser2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "2753c21b-b857-418c-aca4-0f933b0e041e");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e3382454-3c4d-4d78-bd6a-4671f3288819");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150);

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6f080876-1615-4abf-97b6-842bf2648629", "9b8494ae-a9bf-4fb4-94e7-f3f1ba749513", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bfb218ad-779a-40dd-a2b4-84732d4423bd", "712ec5c7-781d-44a0-bc95-5bef43b71fba", "Administration", "ADMINISTRATION" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "6f080876-1615-4abf-97b6-842bf2648629");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "bfb218ad-779a-40dd-a2b4-84732d4423bd");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                schema: "Identity",
                table: "Users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "Identity",
                table: "Users",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2753c21b-b857-418c-aca4-0f933b0e041e", "a2bcc0d0-e569-4ea7-8faf-4f96d9e022f1", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e3382454-3c4d-4d78-bd6a-4671f3288819", "494fc6d8-9352-4538-9389-30502f1dee02", "Administration", "ADMINISTRATION" });
        }
    }
}
