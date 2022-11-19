using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSGMicroservice.IDP.Persistence.Migrations
{
    public partial class UpdateTblUser1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "889a07bb-14fe-447b-b142-578af4a2a4b8", "2d8689b1-0a80-4627-a3c2-c69f623de6e1", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9f7b5a0e-b188-40ca-86f8-0ae3d04200ce", "f385cc66-6049-4351-949d-25bae15e8a2a", "Administration", "ADMINISTRATION" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "889a07bb-14fe-447b-b142-578af4a2a4b8");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "9f7b5a0e-b188-40ca-86f8-0ae3d04200ce");

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
        }
    }
}
