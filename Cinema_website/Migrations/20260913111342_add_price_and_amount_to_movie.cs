using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema_website.Migrations
{
    /// <inheritdoc />
    public partial class add_price_and_amount_to_movie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Movies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Movies");
        }
    }
}
