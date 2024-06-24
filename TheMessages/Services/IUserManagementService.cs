using Microsoft.AspNetCore.Identity;
using TheMessages.EntityModels;
using TheMessages.ModelsDTO.Users;
using TheMessagesWebApi.ModelsDTO.ContactRequest;

namespace TheMessages.Services
{
    public interface IUserManagementService
    {
        Task<List<UserSearchDTO>> GetContactsAsync(AppUser user);
        Task CreateContactRequestAsync(AppUser UserFrom, AppUser UserTo);
        Task<List<RequestViewDTO>> GetRequestsAsync(AppUser appUser);
        Task ApplyContactAsync(AppUser currentUser, string id);
        Task <List<UserSearchDTO>> GetUsersWithCreatedContactRequest(AppUser appUser);

    }
}
