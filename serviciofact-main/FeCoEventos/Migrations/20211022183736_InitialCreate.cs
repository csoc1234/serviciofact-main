using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FeCoEventos.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invoice_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_enterprise = table.Column<int>(type: "int", nullable: false),
                    invoice_id = table.Column<int>(type: "int", nullable: false),
                    document_id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    invoice_uuid = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    invoice_uuid_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    invoice_issuedate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    supplier_type_identification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    supplier_identification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_type_identification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_identification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    event_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    event_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    event_uuid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    dian_status = table.Column<int>(type: "int", nullable: false),
                    active = table.Column<bool>(type: "bit", nullable: false),
                    dian_message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dian_response_datetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    path_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    namefile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    session_log = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    track_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_events", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_events");
        }
    }
}
