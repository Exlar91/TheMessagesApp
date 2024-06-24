using TheMessages.EntityModels;
using TheMessages.ModelsDTO.Users;

namespace TheMessagesWebApi.Services
{
    public interface IUserService
    {
        Task<AppUser> CreateAsync(AppUser user);
        Task<UserSearchDTO> GetUserDTOAsync(AppUser? user, AppUser currentUser);
        

    }
}
