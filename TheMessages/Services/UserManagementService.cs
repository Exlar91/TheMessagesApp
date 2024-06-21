using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TheMessages.EntityModels;
using TheMessages.ModelsDTO.Users;
using TheMessagesWebApi.ModelsDTO.ContactRequest;

namespace TheMessages.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly DBContext _dBContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;


        public UserManagementService(DBContext dBContext, UserManager<AppUser> userManager)
        {
            _dBContext = dBContext;
            _userManager = userManager;
        }

        public async Task CreateContactRequestAsync(AppUser UserFrom, AppUser UserTo)
        {
            bool letreq=true;

            foreach (var r in _dBContext.Requests)
            {
                if ((r.From==UserFrom && r.To== UserTo) || (r.From== UserTo && r.To == UserFrom))
                {
                    letreq = false;
                }
            }

            if (letreq) {
                FriendRequest _request = new FriendRequest();
                _request.From = UserFrom;
                _request.To = UserTo;
                _request.IsApplied = false;
                await _dBContext.Requests.AddAsync(_request);
                await _dBContext.SaveChangesAsync();
            }

            else
            {
                throw new Exception("Similar request exists");
            }

        }

        public async Task<List<UserSearchDTO>> GetContactsAsync(AppUser user)
        {
            var data = _dBContext.Requests
                .Include(u => u.To)
                .Include(u => u.From)
                .ToList();

            var AppliedRequests = from r in data
                                  where r.From == user || r.To == user
                                  orderby r.Id
                                  select r;
            List<UserSearchDTO> result = new List<UserSearchDTO>();

            foreach (var req in AppliedRequests)
            {
                var User = new UserSearchDTO();

                if (req.IsApplied)
                {
                    if (req.To == user)
                    {
                        User.Id = req.From.Id;
                        User.Name = req.From.Name;
                        User.SecondName = req.From.SecondName;
                    }
                    if (req.From == user)
                    {
                        User.Id = req.To.Id;
                        User.Name = req.To.Name;
                        User.SecondName = req.To.SecondName;
                    }
                    result.Add(User);
                }
            }
            return result;
        }

       

        public async Task<UserSearchDTO> GetUserInfoAsync(string? id, string currentUserId)
        {
            
            int userIdToGet = Convert.ToInt32(currentUserId);

            if (id != null)
            {
                userIdToGet = Convert.ToInt32(id);
            }
            
            var user = await _userManager.Users.Include(x => x.LivingPlace).FirstOrDefaultAsync(u => (u.Id) == userIdToGet);

            var usertoreturn = _mapper.Map<UserSearchDTO>(user);
            usertoreturn.DisplayName = user.Name + " " + user.SecondName;
            usertoreturn.CurrentUser = true;
            usertoreturn.City = user.LivingPlace.Name;

            if (id != null)
            {
                if (id != currentUserId)
                {
                    usertoreturn.CurrentUser = false;
                }
            }



            return usertoreturn;
        }



        public async Task<List<FriendRequest>> GetRequestsAsync(AppUser appUser)
        {          
            var requests = _dBContext.Requests.Where(r=>r.To == appUser).ToList();

            return requests;
        }

        public Task<List<RequestViewDTO>> GetRequestsToViewAsync(AppUser appUser)
        {
            var RequestsDTO = new List<RequestViewDTO>();




        }
    }
}
