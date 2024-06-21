using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheMessages.EntityModels;
using TheMessages.Services;

namespace TheMessages.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        
        private readonly ICitiesService _citiesService;

        public CitiesController(ICitiesService citiesService)
        {
            _citiesService = citiesService;            
        }

        [HttpGet("GetCities")]

        public async Task<ActionResult<List<City>>> GetCities(string? request) 
        {
             return Ok(await _citiesService.FindCitiesAsync(request));
        }
    }
}
