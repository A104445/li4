using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MontagemPro.Migrations
{
    /// <inheritdoc />
    public partial class processControllerProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductsId",
                table: "ProductionProcess",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionProcess_ProductsId",
                table: "ProductionProcess",
                column: "ProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionProcess_Products_ProductsId",
                table: "ProductionProcess",
                column: "ProductsId",
                principalTable: "Products",
                principalColumn: "ProductsId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionProcess_Products_ProductsId",
                table: "ProductionProcess");

            migrationBuilder.DropIndex(
                name: "IX_ProductionProcess_ProductsId",
                table: "ProductionProcess");

            migrationBuilder.DropColumn(
                name: "ProductsId",
                table: "ProductionProcess");
        }
    }
}
