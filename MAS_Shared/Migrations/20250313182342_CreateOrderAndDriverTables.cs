using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAS_Shared.Migrations
{
    /// <inheritdoc />
    public partial class CreateOrderAndDriverTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    LastOnline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GPSLat = table.Column<double>(type: "float", nullable: true),
                    GPSLong = table.Column<double>(type: "float", nullable: true),
                    LocationLastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Drivers_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    OrderItems = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DttmInitiated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderTotal = table.Column<decimal>(type: "numeric(12,0)", nullable: true),
                    DriverID = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IsBeingCollected = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsEnrouteToCust = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsArrivedAtCust = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DttmDelivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisputedReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DttmClosed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Drivers_DriverID",
                        column: x => x.DriverID,
                        principalTable: "Drivers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DriverID",
                table: "Orders",
                column: "DriverID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserID",
                table: "Orders",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Drivers");
        }
    }
}
