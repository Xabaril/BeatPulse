using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BeatPulse.UI.Core.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LivenessConfiguration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LivenessName = table.Column<string>(maxLength: 500, nullable: false),
                    LivenessUri = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivenessConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LivenessExecutionHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsHealthy = table.Column<bool>(nullable: false),
                    LastExecuted = table.Column<DateTime>(nullable: false),
                    LivenessName = table.Column<string>(maxLength: 500, nullable: false),
                    LivenessResult = table.Column<string>(maxLength: 2000, nullable: false),
                    LivenessUri = table.Column<string>(maxLength: 500, nullable: false),
                    OnStateFrom = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivenessExecutionHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LivenessFailuresNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastNotified = table.Column<DateTime>(nullable: false),
                    LivenessName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivenessFailuresNotifications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LivenessConfiguration");

            migrationBuilder.DropTable(
                name: "LivenessExecutionHistory");

            migrationBuilder.DropTable(
                name: "LivenessFailuresNotifications");
        }
    }
}
