using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataConnectorLibraryProject.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Performers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PerfomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EdpouCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Performers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServicePrice = table.Column<double>(type: "float", nullable: false),
                    ServicePriceWithPdv = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeVehicles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeVehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EdpouCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patronymic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patronymic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpnCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandVehicle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StateNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeVehicleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vehicles_TypeVehicles_TypeVehicleId",
                        column: x => x.TypeVehicleId,
                        principalTable: "TypeVehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerEmployee",
                columns: table => new
                {
                    CustomersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerEmployee", x => new { x.CustomersId, x.EmployeesId });
                    table.ForeignKey(
                        name: "FK_CustomerEmployee_Customers_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerEmployee_Employees_EmployeesId",
                        column: x => x.EmployeesId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePerformer",
                columns: table => new
                {
                    EmployeesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PerformersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePerformer", x => new { x.EmployeesId, x.PerformersId });
                    table.ForeignKey(
                        name: "FK_EmployeePerformer_Employees_EmployeesId",
                        column: x => x.EmployeesId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeePerformer_Performers_PerformersId",
                        column: x => x.PerformersId,
                        principalTable: "Performers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceEquipment = table.Column<double>(type: "float", nullable: false),
                    PriceEquipmentWithPdv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IMEIPT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IMEIDRP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelDRP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelRT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProvidedServices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PerformerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VehicleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvidedServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvidedServices_Performers_PerformerId",
                        column: x => x.PerformerId,
                        principalTable: "Performers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProvidedServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProvidedServices_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerEmployee_EmployeesId",
                table: "CustomerEmployee",
                column: "EmployeesId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PositionId",
                table: "Customers",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePerformer_PerformersId",
                table: "EmployeePerformer",
                column: "PerformersId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PositionId",
                table: "Employees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_VehicleId",
                table: "Equipments",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvidedServices_PerformerId",
                table: "ProvidedServices",
                column: "PerformerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvidedServices_ServiceId",
                table: "ProvidedServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvidedServices_VehicleId",
                table: "ProvidedServices",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CustomerId",
                table: "Vehicles",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TypeVehicleId",
                table: "Vehicles",
                column: "TypeVehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerEmployee");

            migrationBuilder.DropTable(
                name: "EmployeePerformer");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "ProvidedServices");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Performers");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "TypeVehicles");

            migrationBuilder.DropTable(
                name: "Positions");
        }
    }
}
