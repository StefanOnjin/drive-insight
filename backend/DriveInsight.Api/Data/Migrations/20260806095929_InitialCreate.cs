using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveInsight.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "drives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_drives_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detected_events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DriveId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Severity = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detected_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_detected_events_drives_DriveId",
                        column: x => x.DriveId,
                        principalTable: "drives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "drive_analyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DriveId = table.Column<Guid>(type: "uuid", nullable: false),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    DistanceMeters = table.Column<double>(type: "double precision", nullable: false),
                    AverageSpeedKmh = table.Column<double>(type: "double precision", nullable: false),
                    MaxSpeedKmh = table.Column<double>(type: "double precision", nullable: false),
                    HarshAccelerationCount = table.Column<int>(type: "integer", nullable: false),
                    HarshBrakingCount = table.Column<int>(type: "integer", nullable: false),
                    SharpTurnCount = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drive_analyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_drive_analyses_drives_DriveId",
                        column: x => x.DriveId,
                        principalTable: "drives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gps_readings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DriveId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    SpeedMps = table.Column<double>(type: "double precision", nullable: true),
                    AccuracyMeters = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gps_readings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gps_readings_drives_DriveId",
                        column: x => x.DriveId,
                        principalTable: "drives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "motion_readings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DriveId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AccelerometerX = table.Column<double>(type: "double precision", nullable: false),
                    AccelerometerY = table.Column<double>(type: "double precision", nullable: false),
                    AccelerometerZ = table.Column<double>(type: "double precision", nullable: false),
                    GyroscopeX = table.Column<double>(type: "double precision", nullable: true),
                    GyroscopeY = table.Column<double>(type: "double precision", nullable: true),
                    GyroscopeZ = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_motion_readings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_motion_readings_drives_DriveId",
                        column: x => x.DriveId,
                        principalTable: "drives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_detected_events_DriveId_Timestamp",
                table: "detected_events",
                columns: new[] { "DriveId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_drive_analyses_DriveId",
                table: "drive_analyses",
                column: "DriveId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_drives_UserId",
                table: "drives",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_gps_readings_DriveId_Timestamp",
                table: "gps_readings",
                columns: new[] { "DriveId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_motion_readings_DriveId_Timestamp",
                table: "motion_readings",
                columns: new[] { "DriveId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detected_events");

            migrationBuilder.DropTable(
                name: "drive_analyses");

            migrationBuilder.DropTable(
                name: "gps_readings");

            migrationBuilder.DropTable(
                name: "motion_readings");

            migrationBuilder.DropTable(
                name: "drives");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
