using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MontagemPro.Migrations
{
    /// <inheritdoc />
    public partial class ordersQuantidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completada",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Quantidade",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completada",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "Orders");
        }
    }
}
