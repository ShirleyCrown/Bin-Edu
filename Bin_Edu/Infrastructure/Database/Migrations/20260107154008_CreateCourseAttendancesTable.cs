using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bin_Edu.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateCourseAttendancesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttendedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseTimetableId = table.Column<int>(type: "int", nullable: false),
                    CourseRegistrationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseAttendances_CourseRegistrations_CourseRegistrationId",
                        column: x => x.CourseRegistrationId,
                        principalTable: "CourseRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseAttendances_CourseTimetables_CourseTimetableId",
                        column: x => x.CourseTimetableId,
                        principalTable: "CourseTimetables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseAttendances_CourseRegistrationId",
                table: "CourseAttendances",
                column: "CourseRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAttendances_CourseTimetableId",
                table: "CourseAttendances",
                column: "CourseTimetableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseAttendances");
        }
    }
}
