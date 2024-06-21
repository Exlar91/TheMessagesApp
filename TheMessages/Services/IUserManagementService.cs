using Microsoft.AspNetCore.Identity;
using TheMessages.EntityModels;
using TheMessages.ModelsDTO.Users;
using TheMessagesWebApi.ModelsDTO.ContactRequest;

namespace TheMessages.Services
{
    public interface IUserManagementService
    {
        public Task<List<UserSearchDTO>> GetContactsAsync(AppUser user);
        public Task CreateContactRequestAsync(AppUser UserFrom, AppUser UserTo);
        public Task<UserSearchDTO> GetUserInfoAsync(string? id, string currentUserId);
        public Task<List<FriendRequest>> GetRequestsAsync(AppUser appUser);
        public Task<List<RequestViewDTO>> GetRequestsToViewAsync(AppUser appUser);
    }
}
