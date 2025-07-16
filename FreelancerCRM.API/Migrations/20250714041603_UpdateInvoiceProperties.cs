using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelancerCRM.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvoiceProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsKDVMukellefi",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TCKN",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxOffice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EInvoiceType",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "EInvoiceUUID",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "GIBStatus",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "NetAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ResponseCode",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "SendDate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "StopajAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "StopajRate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsKDVMukellefi",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TaxOffice",
                table: "Clients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsKDVMukellefi",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TCKN",
                table: "Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxOffice",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EInvoiceType",
                table: "Invoices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EInvoiceUUID",
                table: "Invoices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GIBStatus",
                table: "Invoices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NetAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ResponseCode",
                table: "Invoices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SendDate",
                table: "Invoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StopajAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StopajRate",
                table: "Invoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsKDVMukellefi",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "Clients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxOffice",
                table: "Clients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
