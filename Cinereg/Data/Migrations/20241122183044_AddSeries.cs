using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinereg.Migrations
{
    /// <inheritdoc />
    public partial class AddSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    WatchedYear = table.Column<int>(type: "int", nullable: false),
                    ViewingForm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Series_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SeasonNumber = table.Column<int>(type: "int", nullable: false),
                    NumberOfEpisodes = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<double>(type: "decimal", nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    WatchedYear = table.Column<int>(type: "int", nullable: false),
                    ViewingForm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Review = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShowId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seasons_Series_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesGenres",
                columns: table => new
                {
                    GenreId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SeriesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesGenres",
                        columns: sg => new { sg.GenreId, sg.SeriesId });
                    table.ForeignKey(
                        name: "FK_SeriesGenres_Genres_Id",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeriesGenres_Series_Id",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeriesGenres_Genres_Id",
                table: "SeriesGenres");
            migrationBuilder.DropForeignKey(
                name: "FK_SeriesGenres_Series_Id",
                table: "SeriesGenres");
            migrationBuilder.DropForeignKey(
                name: "FK_Seasons_Series_ShowId",
                table: "Seasons");
            migrationBuilder.DropForeignKey(
                name: "FK_Series_AspNetUsers_UserId",
                table: "Series");

            migrationBuilder.DropTable(name: "SeriesGenres");
            migrationBuilder.DropTable(name: "Seasons");
            migrationBuilder.DropTable(name: "Series");
        }
    }
}
