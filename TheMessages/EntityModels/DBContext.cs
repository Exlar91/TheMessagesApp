using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace TheMessages.EntityModels
{
    public class DBContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {       
        public DbSet<Message> Messages { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<FriendRequest> Requests { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DBContext(DbContextOptions<DBContext> options)
                : base(options)
        {
            
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .UseLazyLoadingProxies();
        //}

    }
}
