using Cinereg.Models;

namespace Cinereg.Services
{
    public interface ISeriesService
    {
        Task<List<Series>> GetAll(string userId);
        Task<Series> GetById(string id);
        Task<bool> Add(Series series);
        Task<bool> Update(string id, Series series);
        Task<bool> Delete(string id);
    }
}
