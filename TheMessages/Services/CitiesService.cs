using TheMessages.EntityModels;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;

namespace TheMessages.Services
{
    public class CitiesService : ICitiesService
    {
        private readonly DBContext _dbContext;

        public CitiesService(DBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<List<City>> FindCitiesAsync(string? request)
        {
            if (request == null)
            {
                return _dbContext.Cities.Take(10).ToList();
            }
            
            var data = from c in _dbContext.Cities
                       where c.Name.ToLower().StartsWith(request.ToLower())
                       orderby c.Name
                       select c;

            return data.ToList();
        }

        public async Task<City> FindCityAsync(string id)
        {
            var data = _dbContext.Cities.FirstOrDefault(i => i.Id.ToString() == id);
            return (data);
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return _dbContext.Cities.ToList();
        }

        public async Task<string> GetCityNameByIdAsync(string id)
        {
            Guid cityId = Guid.Parse(id);
            return (_dbContext.Cities.FirstOrDefault(c=>c.Id==cityId)).Name;
        }
        
    }
}
