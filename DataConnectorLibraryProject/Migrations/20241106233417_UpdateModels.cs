using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataConnectorLibraryProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerEmployee_Customers_CustomersCustomerId",
                table: "CustomerEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerEmployee_Employees_EmployeesEmployeeId",
                table: "CustomerEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePerformer_Employees_EmployeesEmployeeId",
                table: "EmployeePerformer");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePerformer_Performers_PerformersPerformerId",
                table: "EmployeePerformer");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "Vehicles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TypeVehicleId",
                table: "TypeVehicles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Services",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ProvidedServiceId",
                table: "ProvidedServices",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "Positions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PerformerId",
                table: "Performers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "Equipments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Employees",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PerformersPerformerId",
                table: "EmployeePerformer",
                newName: "PerformersId");

            migrationBuilder.RenameColumn(
                name: "EmployeesEmployeeId",
                table: "EmployeePerformer",
                newName: "EmployeesId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeePerformer_PerformersPerformerId",
                table: "EmployeePerformer",
                newName: "IX_EmployeePerformer_PerformersId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Customers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EmployeesEmployeeId",
                table: "CustomerEmployee",
                newName: "EmployeesId");

            migrationBuilder.RenameColumn(
                name: "CustomersCustomerId",
                table: "CustomerEmployee",
                newName: "CustomersId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerEmployee_EmployeesEmployeeId",
                table: "CustomerEmployee",
                newName: "IX_CustomerEmployee_EmployeesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerEmployee_Customers_CustomersId",
                table: "CustomerEmployee",
                column: "CustomersId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerEmployee_Employees_EmployeesId",
                table: "CustomerEmployee",
                column: "EmployeesId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePerformer_Employees_EmployeesId",
                table: "EmployeePerformer",
                column: "EmployeesId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePerformer_Performers_PerformersId",
                table: "EmployeePerformer",
                column: "PerformersId",
                principalTable: "Performers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerEmployee_Customers_CustomersId",
                table: "CustomerEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerEmployee_Employees_EmployeesId",
                table: "CustomerEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePerformer_Employees_EmployeesId",
                table: "EmployeePerformer");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeePerformer_Performers_PerformersId",
                table: "EmployeePerformer");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Vehicles",
                newName: "VehicleId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TypeVehicles",
                newName: "TypeVehicleId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Services",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProvidedServices",
                newName: "ProvidedServiceId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Positions",
                newName: "PositionId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Performers",
                newName: "PerformerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Equipments",
                newName: "EquipmentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Employees",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "PerformersId",
                table: "EmployeePerformer",
                newName: "PerformersPerformerId");

            migrationBuilder.RenameColumn(
                name: "EmployeesId",
                table: "EmployeePerformer",
                newName: "EmployeesEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeePerformer_PerformersId",
                table: "EmployeePerformer",
                newName: "IX_EmployeePerformer_PerformersPerformerId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Customers",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "EmployeesId",
                table: "CustomerEmployee",
                newName: "EmployeesEmployeeId");

            migrationBuilder.RenameColumn(
                name: "CustomersId",
                table: "CustomerEmployee",
                newName: "CustomersCustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerEmployee_EmployeesId",
                table: "CustomerEmployee",
                newName: "IX_CustomerEmployee_EmployeesEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerEmployee_Customers_CustomersCustomerId",
                table: "CustomerEmployee",
                column: "CustomersCustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerEmployee_Employees_EmployeesEmployeeId",
                table: "CustomerEmployee",
                column: "EmployeesEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePerformer_Employees_EmployeesEmployeeId",
                table: "EmployeePerformer",
                column: "EmployeesEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeePerformer_Performers_PerformersPerformerId",
                table: "EmployeePerformer",
                column: "PerformersPerformerId",
                principalTable: "Performers",
                principalColumn: "PerformerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
