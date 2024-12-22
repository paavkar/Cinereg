using Cinereg.Data;
using Cinereg.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Cinereg.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public MovieService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<List<Movie>> GetAllMovies(string userId)
        {
            //var movies = await _context.Movies.Where(m => m.UserId == userId).ToListAsync();
            var sql = GetMovieSql();

            var moviesDapper = await QueryMoviesAsync(sql, new { UserId = userId });
            return moviesDapper.Distinct().ToList();
        }

        public async Task<List<string>> AddGenres(Movie movie)
        {
            using var connection = GetConnection();
            string sql = @"SELECT * FROM Genres WHERE Name = @GenreName";

            List<string> genreIds = new();

            foreach (Genre genre in movie.MovieGenres)
            {
                genre.Name = genre.Name.Trim();
                genre.Name = genre.Name.Substring(0, 1).ToUpper() + genre.Name.Substring(1).ToLower();
                var existingGenre = await connection.QueryFirstOrDefaultAsync<Genre>(sql, new { GenreName = genre.Name });

                if (existingGenre is null)
                {
                    string genreId = genre.Id;
                    if (genre.Id.IsNullOrEmpty()) genreId = Guid.CreateVersion7().ToString();
                    genreIds.Add(genreId);
                    string insertGenreCommand = @"INSERT INTO Genres (Id, Name) VALUES (@Id, @GenreName)";
                    await connection.ExecuteAsync(insertGenreCommand, new { Id = genreId, GenreName = genre.Name });
                }
                else genreIds.Add(existingGenre.Id);
            }

            return genreIds;
        }

        public async Task<List<string>> AddDirectors(Movie movie)
        {
            using var connection = GetConnection();
            string sql = @"SELECT * FROM Directors WHERE Name = @DirectorName";

            List<string> directorIds = new();

            foreach (Director director in movie.Directors)
            {
                director.Name = director.Name.Trim();
                if (director.Name.IsNullOrEmpty()) continue;
                var existingDirector = await connection.QueryFirstOrDefaultAsync<Director>(sql, new { DirectorName = director.Name });

                if (existingDirector is null)
                {
                    string directorId = director.Id;
                    if (director.Id.IsNullOrEmpty()) directorId = Guid.CreateVersion7().ToString();
                    directorIds.Add(directorId);
                    string insertGenreCommand = @"INSERT INTO Directors (Id, Name) VALUES (@Id, @DirectorName)";
                    await connection.ExecuteAsync(insertGenreCommand, new { Id = directorId, DirectorName = director.Name });
                }
                else directorIds.Add(existingDirector.Id);
            }

            return directorIds;
        }

        public async Task<Movie> AddMovie(Movie movie)
        {
            //_context.Movies.Add(movie);
            //await _context.SaveChangesAsync();

            using var connection = GetConnection();

            List<string> genreIds = await AddGenres(movie);
            List<string> directorIds = await AddDirectors(movie);

            movie.Id = Guid.CreateVersion7().ToString();

            string insertMovieCommand = @"INSERT INTO Movies (Id, Name, ReleaseYear, UserId, WatchedYear, ViewingForm, Review)
                                        VALUES (@Id, @Name, @ReleaseYear, @UserId, @WatchedYear, @ViewingForm, @Review)";

            await connection.ExecuteAsync(insertMovieCommand, movie);

            foreach (string genreId in genreIds)
            {
                string insertGenreCommand = @"INSERT INTO MovieGenres (GenreId, MovieId) VALUES (@GenreId, @MovieId)";
                await connection.ExecuteAsync(insertGenreCommand, new { GenreId = genreId, MovieId = movie.Id });
            }

            foreach (string directorId in directorIds)
            {
                string insertGenreCommand = @"INSERT INTO MovieDirectors (DirectorId, MovieId) VALUES (@DirectorId, @MovieId)";
                await connection.ExecuteAsync(insertGenreCommand, new { DirectorId = directorId, MovieId = movie.Id });
            }

            return movie;
        }

        public async Task<bool> DeleteMovie(string id)
        {
            var sql = @"
                    DELETE FROM Movies
                    WHERE Id = @Id";

            using var connection = GetConnection();
            int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

            return rowsAffected > 0;
        }

        public async Task<Movie> GetMovieById(string id)
        {
            var sql = GetMovieSql(true);

            var moviesDapper = await QueryMoviesAsync(sql, new { Id = id });
            return moviesDapper.Distinct().ToList().First();
            //return await _context.Movies.FindAsync(id);
        }

        public async Task<Movie> UpdateMovie(string id, Movie movie)
        {
            //var dbMovie = await _context.Movies.FindAsync(id);
            //if (dbMovie != null)
            //{
            //    dbMovie.Name = movie.Name;
            //    dbMovie.ReleaseYear = movie.ReleaseYear;
            //    //dbMovie.Genre = movie.Genre;
            //    dbMovie.Director = movie.Director;
            //    dbMovie.WatchedYear = movie.WatchedYear;
            //    dbMovie.ViewingForm = movie.ViewingForm;
            //    dbMovie.Review = movie.Review;
            //    await _context.SaveChangesAsync();
            //    return dbMovie;
            //}
            //return dbMovie;

            var dbMovie = await GetMovieById(id);
            if (dbMovie == null) return null;
            string sql = @"
                        UPDATE Movies
                        SET Name = @Name, ReleaseYear = @ReleaseYear, WatchedYear = @WatchedYear, ViewingForm = @ViewingForm, Review = @Review
                        WHERE Id = @Id";
            using var connection = GetConnection();
            await connection.ExecuteAsync(sql, movie);

            List<string> genreIds = await AddGenres(movie);
            List<string> directorIds = await AddDirectors(movie);

            string deleteSql = @"
                             DELETE FROM MovieGenres
                             WHERE GenreId NOT IN @GenreIds
                             AND MovieId = @MovieId";
            await connection.ExecuteAsync(deleteSql, new { GenreIds = genreIds, MovieId = id });

            string addMovieGenresSql = @"
                                    INSERT INTO MovieGenres (GenreId, MovieId) 
                                    SELECT @GenreId, @MovieId
                                    WHERE NOT EXISTS (SELECT 1
                                                      FROM MovieGenres
                                                      WHERE MovieId = @MovieId
                                                      AND GenreId = @GenreId)";

            foreach (var genreId in genreIds)
            {
                await connection.ExecuteAsync(addMovieGenresSql, new { GenreId = genreId, MovieId = id });
            }

            string addMovieDirectorsSql = @"
                                    INSERT INTO MovieDirectors (DirectorId, MovieId) 
                                    SELECT @DirectorId, @MovieId
                                    WHERE NOT EXISTS (SELECT 1
                                                      FROM MovieDirectors
                                                      WHERE MovieId = @MovieId
                                                      AND DirectorId = @DirectorId)";

            foreach (var directorId in directorIds)
            {
                await connection.ExecuteAsync(addMovieDirectorsSql, new { DirectorId = directorId, MovieId = id });
            }

            return dbMovie!;
        }

        private string GetMovieSql(bool singleMovie = false)
        {
            var sql = @"
                    SELECT m.*, g.Id AS GenreId, g.*, d.Id AS DirectorId, d.*
                    FROM Movies m
                    LEFT JOIN MovieGenres mg ON m.Id = mg.MovieId
                    LEFT JOIN Genres g ON mg.GenreId = g.Id
                    LEFT JOIN MovieDirectors md ON m.Id = md.MovieId
                    LEFT JOIN Directors d ON md.DirectorId = d.Id";

            if (singleMovie) sql += " WHERE m.Id = @Id";
            else sql += " WHERE m.UserId = @UserId ORDER BY m.Name";

            return sql;
        }

        private async Task<IEnumerable<Movie>> QueryMoviesAsync(string sql, object parameters)
        {
            var movieDictionary = new Dictionary<string, Movie>();
            using var connection = GetConnection();

            var movies = await connection.QueryAsync<Movie, Genre, Director, Movie>(
                sql,
                (movie, genre, director) =>
                {
                    if (!movieDictionary.TryGetValue(movie.Id!, out var dbMovie))
                    {
                        dbMovie = new Movie
                        {
                            Id = movie.Id,
                            Name = movie.Name,
                            ReleaseYear = movie.ReleaseYear,
                            UserId = movie.UserId,
                            WatchedYear = movie.WatchedYear,
                            ViewingForm = movie.ViewingForm,
                            Review = movie.Review,
                            MovieGenres = new(),
                            Directors = new()
                        };
                        movieDictionary.Add(dbMovie!.Id!, dbMovie);
                    }

                    if (!genre.Name.IsNullOrEmpty() && dbMovie.MovieGenres.Find(g => g.Name == genre.Name) is null)
                    {
                        dbMovie.MovieGenres.Add(genre);
                    }
                    if (!director.Name.IsNullOrEmpty() && dbMovie.Directors.Find(d => d.Name == director.Name) is null)
                    {
                        dbMovie.Directors.Add(director);
                    }
                    return dbMovie;
                },
                parameters,
                splitOn: "GenreId,DirectorId");

            return movies;
        }
    }
}
