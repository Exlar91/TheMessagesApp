using TheMessages.EntityModels;

namespace TheMessagesWebApi.Services
{
    public interface IUserService
    {
        Task<AppUser> CreateAsync(AppUser user);
    }
}
