using Cinereg.Data;
using Cinereg.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<MovieWithGenres>> GetAllMovies(string userId)
        {
            //var movies = await _context.Movies.Where(m => m.UserId == userId).ToListAsync();
            var sql = GetMovieSql();

            var moviesDapper = await QueryMoviesAsync(sql, new { UserId = userId });
            return moviesDapper.Distinct().ToList();
        }

        public async Task<List<string>> AddGenres(MovieWithGenres movie)
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
                    string genreId = Guid.NewGuid().ToString();
                    genreIds.Add(genreId);
                    string insertGenreCommand = @"INSERT INTO Genres (Id, Name) VALUES (@Id, @GenreName)";
                    await connection.ExecuteAsync(insertGenreCommand, new { Id = genreId, GenreName = genre.Name });
                }
                else genreIds.Add(existingGenre.Id);
            }

            return genreIds;
        }

        public async Task<MovieWithGenres> AddMovie(MovieWithGenres movie)
        {
            //_context.Movies.Add(movie);
            //await _context.SaveChangesAsync();

            using var connection = GetConnection();

            List<string> genreIds = await AddGenres(movie);

            movie.Id = Guid.NewGuid().ToString();

            string insertMovieCommand = @"INSERT INTO Movies (Id, Name, ReleaseYear, Director, UserId, WatchedYear, ViewingForm, Review)
                                        VALUES (@Id, @Name, @ReleaseYear, @Director, @UserId, @WatchedYear, @ViewingForm, @Review)";

            await connection.ExecuteAsync(insertMovieCommand, movie);

            foreach (string genreId in genreIds)
            {
                string insertGenreCommand = @"INSERT INTO MovieGenres (GenreId, MovieId) VALUES (@GenreId, @MovieId)";
                await connection.ExecuteAsync(insertGenreCommand, new { GenreId = genreId, MovieId = movie.Id });
            }

            return movie;
        }

        public async Task<bool> DeleteMovie(string id)
        {
            var dbMovie = await _context.Movies.FindAsync(id);
            if (dbMovie != null)
            {
                _context.Remove(dbMovie);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<MovieWithGenres> GetMovieById(string id)
        {
            var sql = GetMovieSql(true);

            var moviesDapper = await QueryMoviesAsync(sql, new { Id = id });
            return moviesDapper.Distinct().ToList().First();
            //return await _context.Movies.FindAsync(id);
        }

        public async Task<MovieWithGenres> UpdateMovie(string id, MovieWithGenres movie)
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
                        SET Name = @Name, ReleaseYear = @ReleaseYear, Director = @Director, WatchedYear = @WatchedYear, ViewingForm = @ViewingForm, Review = @Review
                        WHERE Id = @Id";
            using var connection = GetConnection();
            await connection.ExecuteAsync(sql, movie);

            List<string> genreIds = await AddGenres(movie);
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

            return dbMovie!;
        }

        private string GetMovieSql(bool singleMovie = false)
        {
            var sql = @"
                    SELECT m.*, g.Id AS GenreId, g.Name
                    FROM Movies m
                    LEFT JOIN MovieGenres mg ON m.Id = mg.MovieId
                    LEFT JOIN Genres g ON mg.GenreId = g.Id";

            if (singleMovie) sql += " WHERE m.Id = @Id";
            else sql += " WHERE m.UserId = @UserId";

            return sql;
        }

        private async Task<IEnumerable<MovieWithGenres>> QueryMoviesAsync(string sql, object parameters)
        {
            var movieDictionary = new Dictionary<string, MovieWithGenres>();
            using var connection = GetConnection();
            var movies = await connection.QueryAsync<Movie, Genre, MovieWithGenres>(
                sql,
                (movie, genre) =>
                {
                    if (!movieDictionary.TryGetValue(movie.Id!, out var movieWithGenres))
                    {
                        movieWithGenres = new MovieWithGenres
                        {
                            Id = movie.Id,
                            Name = movie.Name,
                            ReleaseYear = movie.ReleaseYear,
                            Director = movie.Director,
                            UserId = movie.UserId,
                            WatchedYear = movie.WatchedYear,
                            ViewingForm = movie.ViewingForm,
                            Review = movie.Review,
                            MovieGenres = new()
                        };
                        movieDictionary.Add(movieWithGenres!.Id!, movieWithGenres);
                    }

                    if (genre.Name != null)
                    {
                        movieWithGenres.MovieGenres.Add(genre);
                    }
                    return movieWithGenres;
                },
                parameters,
                splitOn: "GenreId");

            return movies;
        }
    }
}
