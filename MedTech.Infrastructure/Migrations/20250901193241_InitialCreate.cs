using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegrationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcessingDuration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmrPatientId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MedTechPatientId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    EmrReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcedureType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcedureCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProcedureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Procedures_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationLogs_EventType_Status",
                table: "IntegrationLogs",
                columns: new[] { "EventType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationLogs_Timestamp",
                table: "IntegrationLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Email",
                table: "Patients",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_EmrPatientId",
                table: "Patients",
                column: "EmrPatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MedTechPatientId",
                table: "Patients",
                column: "MedTechPatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_EmrReferenceId",
                table: "Procedures",
                column: "EmrReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_PatientId_ProcedureDate",
                table: "Procedures",
                columns: new[] { "PatientId", "ProcedureDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationLogs");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
