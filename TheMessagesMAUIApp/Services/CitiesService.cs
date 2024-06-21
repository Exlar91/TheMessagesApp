using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMessagesModels;

namespace TheMessagesMAUIApp.Services
{
    public class CitiesService
    {
       public async Task<List<City>> GetCities()
        {
            var toreturn = new List<City>();

            using (var client = new HttpClient())
            {
                using var result = await client.GetAsync("https://localhost:7189/api/Cities/GetCities");
                
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    toreturn = JsonConvert.DeserializeObject<List<City>>(json);
                    
                }
            }
            return toreturn;
        } 
    }
}
