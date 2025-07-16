using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelancerCRM.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTurkeySpecificFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountRate",
                table: "Invoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OutstandingAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Invoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "InvoiceItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "InvoiceItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "InvoiceItems",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "InvoiceItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Assignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_UserID",
                table: "Assignments",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_UserID",
                table: "Assignments",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_UserID",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_UserID",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DiscountRate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "OutstandingAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Assignments");
        }
    }
}
