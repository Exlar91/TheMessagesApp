using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TheMessages.EntityModels
{
    public class AppUser: IdentityUser<int>
    {
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string ShortWay { get; set; }
        public Avatar? Avatar { get; set; }
        public City? LivingPlace { get; set; } 
        public DateTime? BirthDate { get; set; } 
        public bool Gender { get; set; }
        
    }
}
