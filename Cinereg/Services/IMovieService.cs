using Cinereg.Models;

namespace Cinereg.Services
{
    public interface IMovieService
    {
        Task<List<MovieWithGenres>> GetAllMovies(string userId);
        Task<MovieWithGenres> GetMovieById(string id);
        Task<MovieWithGenres> AddMovie(MovieWithGenres movie);
        Task<MovieWithGenres> UpdateMovie(string id, MovieWithGenres movie);
        Task<bool> DeleteMovie(string id);
    }
}
