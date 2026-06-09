using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrbitalGuardian.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConjunctionEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PrimaryObjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SecondaryObjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PredictedTcaUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MissDistanceKm = table.Column<double>(type: "REAL", nullable: false),
                    TcaSeconds = table.Column<double>(type: "REAL", nullable: false),
                    CollisionProbabilityValue = table.Column<double>(type: "REAL", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConjunctionEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpaceObjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NoradId = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    LaunchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OE_Inclination = table.Column<double>(type: "REAL", nullable: false),
                    OE_Eccentricity = table.Column<double>(type: "REAL", nullable: false),
                    OE_MeanMotion = table.Column<double>(type: "REAL", nullable: false),
                    OE_RightAscension = table.Column<double>(type: "REAL", nullable: false),
                    OE_ArgumentOfPerigee = table.Column<double>(type: "REAL", nullable: false),
                    OE_MeanAnomaly = table.Column<double>(type: "REAL", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Operator = table.Column<string>(type: "TEXT", nullable: true),
                    MissionType = table.Column<string>(type: "TEXT", nullable: true),
                    MassKg = table.Column<double>(type: "REAL", nullable: true),
                    OriginObject = table.Column<string>(type: "TEXT", nullable: true),
                    EstimatedSizeM = table.Column<double>(type: "REAL", nullable: true),
                    CrewCapacity = table.Column<int>(type: "INTEGER", nullable: true),
                    Agency = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConjunctionEventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Severity = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_ConjunctionEvents_ConjunctionEventId",
                        column: x => x.ConjunctionEventId,
                        principalTable: "ConjunctionEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SpaceObjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SV_Pos_X = table.Column<double>(type: "REAL", nullable: false),
                    SV_Pos_Y = table.Column<double>(type: "REAL", nullable: false),
                    SV_Pos_Z = table.Column<double>(type: "REAL", nullable: false),
                    SV_Vel_X = table.Column<double>(type: "REAL", nullable: false),
                    SV_Vel_Y = table.Column<double>(type: "REAL", nullable: false),
                    SV_Vel_Z = table.Column<double>(type: "REAL", nullable: false),
                    SV_UncertaintyKm = table.Column<double>(type: "REAL", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelemetryReadings_SpaceObjects_SpaceObjectId",
                        column: x => x.SpaceObjectId,
                        principalTable: "SpaceObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ConjunctionEventId",
                table: "Alerts",
                column: "ConjunctionEventId");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryReadings_SpaceObjectId",
                table: "TelemetryReadings",
                column: "SpaceObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "TelemetryReadings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ConjunctionEvents");

            migrationBuilder.DropTable(
                name: "SpaceObjects");
        }
    }
}
