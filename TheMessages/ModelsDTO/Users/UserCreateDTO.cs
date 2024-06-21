using System.ComponentModel.DataAnnotations;
using TheMessages.EntityModels;

namespace TheMessages.ModelsDTO.Users
{
    public class UserCreateDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string SecondName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Username { get; set; }
        
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string CityId { get; set; }
        
    }
}
