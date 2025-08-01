using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAS_Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddCellNumberToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CellNumber",
                table: "AspNetUsers",
                type: "nvarchar(18)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    BusinessID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cellnumber = table.Column<string>(type: "nvarchar(18)", nullable: false),
                    PricelistPreamble = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegisteredName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Industry = table.Column<byte>(type: "tinyint", nullable: false),
                    TradingName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    VATNumber = table.Column<long>(type: "bigint", nullable: true),
                    StreetAddress = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    PostalSameAsStreet = table.Column<bool>(type: "bit", nullable: true),
                    PostalAddress = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Facebook = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Twitter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TradingHours = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PayGateID = table.Column<long>(type: "bigint", nullable: true),
                    PayGatePassword = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Bank = table.Column<byte>(type: "tinyint", nullable: true),
                    BranchNumber = table.Column<int>(type: "int", nullable: true),
                    AccountNumber = table.Column<long>(type: "bigint", nullable: true),
                    AccountType = table.Column<byte>(type: "tinyint", nullable: true),
                    GPSLocation = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LocationLastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AverageRating = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.BusinessID);
                });

            migrationBuilder.CreateTable(
                name: "Catalogs",
                columns: table => new
                {
                    CatalogID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessID = table.Column<long>(type: "bigint", nullable: false),
                    CreatorUserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.CatalogID);
                    table.ForeignKey(
                        name: "FK_Catalogs_Businesses_BusinessID",
                        column: x => x.BusinessID,
                        principalTable: "Businesses",
                        principalColumn: "BusinessID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogItems",
                columns: table => new
                {
                    CatalogID = table.Column<long>(type: "bigint", nullable: false),
                    ItemID = table.Column<long>(type: "bigint", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItems", x => new { x.CatalogID, x.ItemID });
                    table.ForeignKey(
                        name: "FK_CatalogItems_Catalogs_CatalogID",
                        column: x => x.CatalogID,
                        principalTable: "Catalogs",
                        principalColumn: "CatalogID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_Cellnumber",
                table: "Businesses",
                column: "Cellnumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_BusinessID",
                table: "Catalogs",
                column: "BusinessID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogItems");

            migrationBuilder.DropTable(
                name: "Catalogs");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropColumn(
                name: "CellNumber",
                table: "AspNetUsers");
        }
    }
}
