using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TheMessages.EntityModels;
using TheMessages.ModelsDTO.Users;
using TheMessagesWebApi.ModelsDTO.ContactRequest;
using TheMessagesWebApi.Services;

namespace TheMessages.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly DBContext _dBContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;


        public UserManagementService(DBContext dBContext, UserManager<AppUser> userManager, IUserService userService)
        {
            _dBContext = dBContext;
            _userManager = userManager;
            _userService = userService;
        }

        public async Task ApplyContactAsync(AppUser currentUser, string id)
        {
            var _id= Guid.Parse(id);
            var request = await _dBContext.Requests.Include(u=>u.From).Include(u=>u.To).FirstAsync(r=>r.Id==_id);
            if (request == null) 
            {
                throw new Exception("Request Not Found");
            }
            else
            {
                if (request.From == currentUser)
                {
                    throw new Exception("User Error");
                }
                else
                {
                    request.IsApplied= true;
                    _dBContext.Requests.Update(request);
                    await _dBContext.SaveChangesAsync();

                }
            }

        }

        //Создание запроса на добавление к контакты
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

        public async Task<List<RequestViewDTO>> GetRequestsAsync(AppUser appUser)
        {
            List<RequestViewDTO> result = new List<RequestViewDTO>();

            var res = from r in (_dBContext.Requests
                      .Include(u => u.From)
                      .Include(u => u.From.LivingPlace)
                      .Include(u => u.To))
                      .Include(u => u.To.LivingPlace)
                      .ToList()
                     where r.From == appUser || r.To == appUser
                     select (r);

            foreach (var req in res) 
            {
                RequestViewDTO request = new RequestViewDTO();
                request.Id = req.Id;
                request.isApplied = req.IsApplied;
                if (req.From == appUser)
                {
                    request.isInput = false;
                    request.User = await _userService.GetUserDTOAsync(req.To, appUser);
                }

                else 
                {
                    request.isInput = true;
                    request.User = await _userService.GetUserDTOAsync(req.From, appUser);
                }

                result.Add(request);
            }
            return result.ToList();
        }

        public async Task<List<UserSearchDTO>> GetUsersWithCreatedContactRequest(AppUser appUser)
        {
            List<UserSearchDTO> result = new List<UserSearchDTO>();

            var list = from r in _dBContext.Requests
                       where (r.From == appUser || r.To == appUser)
                       select r;

            foreach (var req in list) 
            {
                var user = new UserSearchDTO();
                if (req.From == appUser) 
                {
                    user = _mapper.Map<UserSearchDTO>(req.To);

                }
            }
        }
    }
}
