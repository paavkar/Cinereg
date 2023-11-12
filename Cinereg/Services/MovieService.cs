using Cinereg.Data;
using Cinereg.Entities;
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

        public async Task<List<Movie>> GetAllMovies()
        {
            var movies = await _context.Movies.ToListAsync();
            return movies;
        }
    }
}
