using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinereg.Migrations
{
    /// <inheritdoc />
    public partial class Directors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieDirectors",
                columns: table => new
                {
                    DirectorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MovieId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieDirectors",
                        columns: sg => new { sg.DirectorId, sg.MovieId });
                    table.ForeignKey(
                        name: "FK_MovieDirectors_Directors_Id",
                        column: x => x.DirectorId,
                        principalTable: "Directors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieDirectors_Movies_Id",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonDirectors",
                columns: table => new
                {
                    DirectorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SeasonId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonDirectors",
                        columns: sg => new { sg.DirectorId, sg.SeasonId });
                    table.ForeignKey(
                        name: "FK_SeasonDirectors_Directors_Id",
                        column: x => x.DirectorId,
                        principalTable: "Directors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeasonDirectors_Seasons_Id",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.DropColumn(
                name: "Director",
                table: "Movies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonDirectors_Seasons_Id",
                table: "SeasonDirectors");
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonDirectors_Directors_Id",
                table: "SeasonDirectors");
            migrationBuilder.DropForeignKey(
                name: "FK_MovieDirectors_Movies_Id",
                table: "MovieDirectors");
            migrationBuilder.DropForeignKey(
                name: "FK_MovieDirectors_Directors_Id",
                table: "MovieDirectors");

            migrationBuilder.DropTable(name: "SeasonDirectors");
            migrationBuilder.DropTable(name: "MovieDirectors");
            migrationBuilder.DropTable(name: "Directors");

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
