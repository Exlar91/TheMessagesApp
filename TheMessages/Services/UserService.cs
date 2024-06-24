using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheMessages.EntityModels;
using TheMessages.ModelsDTO.Users;

namespace TheMessagesWebApi.Services
{
    public class UserService: IUserService
    {
        private readonly DBContext _dBContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(DBContext dBContext, UserManager<AppUser> userManager, IMapper mapper)
        {
            _dBContext = dBContext;
            _userManager = userManager;
            _mapper = mapper;            
        }

        public async Task<AppUser> CreateAsync(AppUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<UserSearchDTO> GetUserDTOAsync(AppUser? user, AppUser currentUser)
        {
            UserSearchDTO userToReturn;
            if ((user == null)||(user==currentUser))
            {             
                userToReturn = _mapper.Map<UserSearchDTO>(currentUser);
                userToReturn.DisplayName = currentUser.Name + " " + currentUser.SecondName;
                userToReturn.CurrentUser = true;
                userToReturn.City = currentUser.LivingPlace.Name;
                return userToReturn;
            }
            else
            {                
                userToReturn = _mapper.Map<UserSearchDTO>(user);
                userToReturn.DisplayName = user.Name + " " + user.SecondName;
                userToReturn.CurrentUser = false;
                userToReturn.City =user.LivingPlace.Name;
                return userToReturn;
            }           
            
        }
    }
}
