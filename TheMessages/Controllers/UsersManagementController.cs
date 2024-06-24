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
using TheMessagesWebApi.ModelsDTO.ContactRequest;
using TheMessagesWebApi.Services;

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
        public readonly IUserService _userService;


        public UsersManagementController(UserManager<AppUser> userManager,
                                         IUserManagementService userManagementService,
                                         IMapper mapper,
                                         ICitiesService cityService,
                                         IUserService userService
                                         )
        {
            _userManager = userManager;
            _userManagementService = userManagementService;
            _mapper = mapper;
            _cityService = cityService;
            _userService = userService;

        }



        //Вывод информации и пользователе
        [HttpGet("UserInfo")]
        [Authorize]
        public async Task<ActionResult<UserSearchDTO>> GetUserInfo(int? id)
        {
            //Получение ID вошедшего профиля из JWT
            var currentUserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (currentUserId == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.Users.Include(u => u.LivingPlace).FirstOrDefaultAsync(i => i.Id == currentUserId);

            AppUser user=null;
            if (id!=null)
            {
                user = await _userManager.Users.Include(u => u.LivingPlace).FirstOrDefaultAsync(i => i.Id == id);

                if (user == null) 
                {
                    return NotFound();
                }
            }            
            return await _userService.GetUserDTOAsync(user, currentUser);
          
        }

        //Создание запроса на установления контакта
        [HttpPost("CreateContactRequest")]
        [Authorize]
        public async Task<ActionResult> CreateContactRequest(string id)
        {

            string userId = User.FindFirst(ClaimTypes.Name)?.Value;

            if (userId == null)
            {
                return BadRequest("User ID not found in claims.");
            }

            if (userId == id)
            {
                return BadRequest("Selfrequest");
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

            if (userTo == userFrom)
            {
                return BadRequest("Selfrequest");
            }

            try
            {
                await _userManagementService.CreateContactRequestAsync(userFrom, userTo);
                return Ok();
            }

            catch (Exception ex) 
            {
                return BadRequest("такой запрос существует");
            }
  
        }

        //Вывод списка всех неподтвержденных контактов
        [HttpGet("GetContactRequests")]
        [Authorize]
        public async Task<ActionResult<List<RequestViewDTO>>> GetContactRequests()
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
 
            {
                return Ok(await _userManagementService.GetRequestsAsync(userFrom));
            }



        }

        //Поиск пользователей
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


        [HttpPut("ApplyContactRequest")]
        [Authorize]
        public async Task<ActionResult> ApplyRequest(string id)
        {
            var currentUserName = User.FindFirst(ClaimTypes.Name)?.Value;

            if (currentUserName == null)
            {
                return BadRequest("User ID not found in claims.");
            }

            var currentUser = await _userManager.FindByNameAsync(currentUserName);

            if (currentUser == null) 
            {
                return BadRequest("User does no exist");
            }

            try
            {
                await _userManagementService.ApplyContactAsync(currentUser, id);
                return Ok();
            }

            catch (Exception ex) 
            {
                return BadRequest();
            }

        }


    }
}
