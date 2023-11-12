using Cinereg.Entities;

namespace Cinereg.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAllMovies();
        Task<Movie> AddMovie(Movie movie);
        Task<bool> DeleteMovie(string Id);
    }
}
