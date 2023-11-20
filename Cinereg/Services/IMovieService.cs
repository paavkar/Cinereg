using Cinereg.Entities;

namespace Cinereg.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAllMovies(string userId);
        Task<Movie> GetMovieById(string id);
        Task<Movie> AddMovie(Movie movie);
        Task<Movie> UpdateMovie(string id, Movie movie);
        Task<bool> DeleteMovie(string id);
    }
}
