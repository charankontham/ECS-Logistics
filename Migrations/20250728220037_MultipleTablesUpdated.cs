using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECS_Logistics.Migrations
{
    /// <inheritdoc />
    public partial class MultipleTablesUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "delivery_agent_id",
                table: "order_returns");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "order_returns",
                newName: "product_quantity");

            migrationBuilder.AlterColumn<string>(
                name: "order_tracking_id",
                table: "order_returns",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "brand_id",
                table: "order_returns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "order_returns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "product_id",
                table: "order_returns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "subcategory_id",
                table: "order_returns",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "brand_id",
                table: "order_returns");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "order_returns");

            migrationBuilder.DropColumn(
                name: "product_id",
                table: "order_returns");

            migrationBuilder.DropColumn(
                name: "subcategory_id",
                table: "order_returns");

            migrationBuilder.RenameColumn(
                name: "product_quantity",
                table: "order_returns",
                newName: "order_id");

            migrationBuilder.AlterColumn<int>(
                name: "order_tracking_id",
                table: "order_returns",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "delivery_agent_id",
                table: "order_returns",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
