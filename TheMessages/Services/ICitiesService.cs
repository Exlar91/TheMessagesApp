

using TheMessages.EntityModels;

namespace TheMessages.Services
{
    public interface ICitiesService
    {
        Task<List<City>> FindCitiesAsync(string request);
        Task<List<City>> GetAllCitiesAsync();
        Task<City> FindCityAsync(string id);
        Task<string> GetCityNameByIdAsync(string id);

    }
}
