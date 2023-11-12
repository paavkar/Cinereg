using Cinereg.Shared.Entities;

namespace Cinereg.Shared.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAllMovies();
        Task<Movie> AddMovie(Movie movie);
    }
}
