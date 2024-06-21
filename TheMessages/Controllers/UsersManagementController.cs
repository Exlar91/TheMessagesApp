using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TheMessages.EntityModels;
using TheMessages.ModelsDTO.Users;
using TheMessages.Services;

namespace TheMessages.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersManagementController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserManagementService _userManagementService;
        private readonly IMapper _mapper;
        public readonly ICitiesService _cityService;


        public UsersManagementController(UserManager<AppUser> userManager,
                                         IUserManagementService userManagementService,
                                         IMapper mapper,
                                         ICitiesService cityService)
        {
            _userManager = userManager;
            _userManagementService = userManagementService;
            _mapper = mapper;
            _cityService = cityService;

        }



        [HttpGet("UserInfo")]
        [Authorize]
        public async Task<ActionResult<UserSearchDTO>> GetUserInfo(string? id)
        {
            //Получение ID вошедшего профиля из JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (userId == null)
            {
                return BadRequest("User ID not found in claims.");
            }

            int userIdToGet = Convert.ToInt32(userId);

            if (id != null)
            {
                userIdToGet = Convert.ToInt32(id);
            }


            var user = await _userManager.Users.Include(x=>x.LivingPlace).FirstOrDefaultAsync(u=>(u.Id)==userIdToGet);
            
            if (user == null)
            {
                return NotFound("User not found.");
            }

            
            var usertoreturn = _mapper.Map<UserSearchDTO>(user);
            usertoreturn.DisplayName = user.Name+" "+user.SecondName;
            usertoreturn.CurrentUser = true;
            usertoreturn.City = user.LivingPlace.Name;
            
            if (id != null)
            {
                if (id != userId) 
                {
                    usertoreturn.CurrentUser = false;
                }        
            }

            return Ok(usertoreturn);
        }

        //Получение списка всех подтвержденных котакттов
        [HttpGet("GetContacts")]
        [Authorize]
        public async Task<ActionResult<List<UserSearchDTO>>> GetFriends()
        {
            //Получение ID пользователя из токена
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (userId == null)
            {
                return BadRequest("User ID not found in claims.");
            }

            var user = await _userManager.FindByNameAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManagementService.GetContactsAsync(user);

            return (Ok(result));


        }


        //Создание запроса на установления контакта
        [HttpPost("CreateContactRequest")]
        [Authorize]
        public async Task<ActionResult> ContactRequest(string id)
        {

            var userId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (userId == null)
            {
                return BadRequest("User ID not found in claims.");
            }
            var userFrom = await _userManager.FindByNameAsync(userId);

            if (userFrom == null)
            {
                return NotFound("User not found.");
            }

 
            var userTo = await _userManager.FindByIdAsync(id);
            if (userTo == null)
            {
                return NotFound("User not found.");
            }

            if (userFrom == userTo)
            {
                return BadRequest("Selfie request");
            }


            await _userManagementService.CreateContactRequestAsync(userFrom, userTo);

            return Ok();
        }

        //[HttpGet("GetFriendRequests")]
        //[Authorize]
        //public async Task<ActionResult<List<FriendRequest>>> GetFriendRequests()
        //{
        //    var userId = User.FindFirst(ClaimTypes.Name)?.Value;

        //    if (userId == null)
        //    {
        //        return BadRequest("User ID not found in claims.");
        //    }

        //    var user = await _userManager.FindByNameAsync(userId);

        //    if (user == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    var toreturn = from item in _dbContext.Requests.Include(u => u.From)
        //                   where item.To == user
        //                   where item.IsApplied == false
        //                   select item;

        //    return Ok(toreturn);

        //}



        [HttpGet("UserSearch")]
        //[Authorize]
        public async Task<ActionResult<List<UserSearchDTO>>> UsersSearch(string? request)
        {
            var UsersToReturn = new List<UserSearchDTO>();

            if (request == null)
            {
                foreach (var item in (_userManager.Users))
                {
                    var us = new UserSearchDTO();
                    us.Name = item.Name;
                    us.SecondName = item.SecondName;
                    us.Id = item.Id;
                    us.DisplayName=item.Name+" "+item.SecondName;
                    UsersToReturn.Add(us);
                }
                return Ok(UsersToReturn);
            }

            else
            {
                var users = _userManager.Users;
                return Ok(
                    from user in users
                    where user.Name.ToLower().Contains(request)
                    orderby user.Id
                    select user
                    );
            }
        }




        //[HttpGet("GetFriendRequests")]
        //[Authorize]
        //public async Task<ActionResult<List<FriendRequest>>> GetFriendRequests()
        //{
        //    var userId = User.FindFirst(ClaimTypes.Name)?.Value;

        //    if (userId == null)
        //    {
        //        return BadRequest("User ID not found in claims.");
        //    }

        //    var user = await _userManager.FindByNameAsync(userId);

        //    if (user == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    var toreturn = from item in _dbContext.Requests.Include(u => u.From)
        //                   where item.To == user
        //                   where item.IsApplied == false
        //                   select item;

        //    return Ok(toreturn);

        //}

        //[HttpPost("ApplyRequest")]
        //[Authorize]

        //public async Task<ActionResult> ApplyRequest(Guid id)
        //{
        //    var req = _dbContext.Requests
        //              .Where(x => x.Id == id)
        //              .FirstOrDefault();
        //    req.IsApplied = true;
        //    _dbContext.Update(req);
        //    _dbContext.SaveChanges();

        //    return Ok();
        //}



    }
}
