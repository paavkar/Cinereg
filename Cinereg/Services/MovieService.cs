using Cinereg.Data;
using Cinereg.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinereg.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;

        public MovieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movie> AddMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

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

        public async Task<List<Movie>> GetAllMovies(string userId)
        {
            var movies = await _context.Movies.Where(m => m.UserId == userId).ToListAsync();
            return movies;
        }

        public async Task<Movie> GetMovieById(string id)
        {
            return await _context.Movies.FindAsync(id);
        }

        public async Task<Movie> UpdateMovie(string id, Movie movie)
        {
            var dbMovie = await _context.Movies.FindAsync(id);
            if (dbMovie != null)
            {
                dbMovie.Name = movie.Name;
                dbMovie.ReleaseYear = movie.ReleaseYear;
                dbMovie.Genre = movie.Genre;
                dbMovie.Director = movie.Director;
                dbMovie.WatchedYear = movie.WatchedYear;
                dbMovie.ViewingForm = movie.ViewingForm;
                dbMovie.Review = movie.Review;
                await _context.SaveChangesAsync();
                return dbMovie;
            }
            return dbMovie;
        }
    }
}
