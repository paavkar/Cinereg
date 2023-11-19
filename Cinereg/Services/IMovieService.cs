using Cinereg.Entities;

namespace Cinereg.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAllMovies(string UserId);
        Task<Movie> AddMovie(Movie movie);
        Task<bool> DeleteMovie(string Id);
    }
}
