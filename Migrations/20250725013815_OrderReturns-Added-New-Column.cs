using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECS_Logistics.Migrations
{
    /// <inheritdoc />
    public partial class OrderReturnsAddedNewColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "customer_id",
                table: "order_returns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "delivery_hub_address_id",
                table: "delivery_hubs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "order_returns");

            migrationBuilder.AlterColumn<int>(
                name: "delivery_hub_address_id",
                table: "delivery_hubs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
