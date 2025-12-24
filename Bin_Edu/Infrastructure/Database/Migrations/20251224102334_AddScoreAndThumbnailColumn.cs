using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bin_Edu.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreAndThumbnailColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Score",
                table: "ExerciseSubmissions",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<byte[]>(
                name: "ThumbNail",
                table: "Courses",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "ExerciseSubmissions");

            migrationBuilder.DropColumn(
                name: "ThumbNail",
                table: "Courses");
        }
    }
}
