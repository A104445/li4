using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MontagemPro.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToProductComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductComponents",
                table: "ProductComponents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts");

            migrationBuilder.AddColumn<int>(
                name: "ProductComponentId",
                table: "ProductComponents",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "OrderProductId",
                table: "OrderProducts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductComponents",
                table: "ProductComponents",
                column: "ProductComponentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts",
                column: "OrderProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductComponents_ProductsId",
                table: "ProductComponents",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_ProductsId",
                table: "OrderProducts",
                column: "ProductsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductComponents",
                table: "ProductComponents");

            migrationBuilder.DropIndex(
                name: "IX_ProductComponents_ProductsId",
                table: "ProductComponents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_OrderProducts_ProductsId",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "ProductComponentId",
                table: "ProductComponents");

            migrationBuilder.DropColumn(
                name: "OrderProductId",
                table: "OrderProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductComponents",
                table: "ProductComponents",
                columns: new[] { "ProductsId", "ComponentsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts",
                columns: new[] { "ProductsId", "OrdersId" });
        }
    }
}
