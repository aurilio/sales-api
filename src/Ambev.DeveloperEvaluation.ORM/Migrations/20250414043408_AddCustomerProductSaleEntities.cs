using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations;

/// <inheritdoc />
public partial class AddCustomerProductSaleEntities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "Users",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Price = table.Column<decimal>(type: "numeric", nullable: false),
                Description = table.Column<string>(type: "text", nullable: false),
                Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Rating_Rate = table.Column<decimal>(type: "numeric", nullable: false),
                Rating_Count = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Sales",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                SaleNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                SaleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                CustomerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                Branch = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                IsCancelled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sales", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SaleItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                SaleId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductId = table.Column<int>(type: "integer", nullable: false),
                ProductName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false),
                UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                Discount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SaleItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_SaleItems_Sales_SaleId",
                    column: x => x.SaleId,
                    principalTable: "Sales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_SaleItems_SaleId",
            table: "SaleItems",
            column: "SaleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Customers");

        migrationBuilder.DropTable(
            name: "Products");

        migrationBuilder.DropTable(
            name: "SaleItems");

        migrationBuilder.DropTable(
            name: "Sales");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "Users");
    }
}
