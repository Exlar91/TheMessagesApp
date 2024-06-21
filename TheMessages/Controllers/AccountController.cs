using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using TheMessages.EntityModels;
using TheMessages.ModelsDTO.Users;
using TheMessages.Options;
using TheMessages.Services;

namespace TheMessages.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public readonly ICitiesService _citiesService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, DBContext dBContext,
            IMapper mapper, ICitiesService citiesService, IWebHostEnvironment environment, IImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _citiesService = citiesService;
            _environment = environment;
            _imageService = imageService;
        }

        [HttpPost("LoadAvatar")]
        public async Task<ActionResult> LoadAvatar(IFormFile file)
        {
            var name = await _imageService.LoadImage(file);
            return Ok();
        }





        //Регистрация нового пользователя
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(UserCreateDTO userDto)
        {
            if (ModelState.IsValid)
            {
                var userCity = await _citiesService.FindCityAsync(userDto.CityId);
                if (userCity == null)
                {
                    return NotFound("City is not exist");
                }

                int id = 1;

                var LastUser = await _userManager.Users.OrderBy(i=>id).LastOrDefaultAsync();


                if (LastUser != null)
                {
                    int newid = LastUser.Id + 1;
                    id = newid;
                }

                if( await _userManager.FindByNameAsync(userDto.Username)!=null)
                {
                    return BadRequest("UserNameExist");
                }

                else
                {
                    var UserToCreate = new AppUser();
                    //UserToCreate.Id = id;
                    UserToCreate.ShortWay = id.ToString();
                    UserToCreate.BirthDate = userDto.BirthDate;
                    UserToCreate.Gender = userDto.Gender;
                    UserToCreate.UserName = userDto.Username;
                    UserToCreate.Name = userDto.Name;
                    UserToCreate.SecondName = userDto.SecondName;
                    UserToCreate.LivingPlace = userCity;


                    var result = await _userManager.CreateAsync(UserToCreate, userDto.Password);

                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                }
                               
                
            }
            return BadRequest();




        }


            //Генерация токена
            [HttpPost("Login")]
            public async Task<ActionResult<JwtKey>> Login(UserLoginDTO model)
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user != null)
                    {
                        var passwordCorrect = await _userManager.CheckPasswordAsync(user, model.Password);
                        if (passwordCorrect)
                        {
                            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: false);
                            if (result.Succeeded)
                            {


                                var claims = new List<Claim> { new Claim(ClaimTypes.Name, model.UserName) };
                                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

                                //foreach (var role in roles)
                                //{
                                //    claims.Add(new Claim(ClaimTypes.Role, role));
                                //}
                                await _signInManager.SignInAsync(user, isPersistent: false);

                                var jwt = new JwtSecurityToken(
                                        issuer: AuthOptions.ISSUER,
                                        audience: AuthOptions.AUDIENCE,
                                        claims: claims,
                                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(20)), // время действия 2 минуты
                                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                                var JWTTo = new JwtKey();
                                JWTTo.Key = new JwtSecurityTokenHandler().WriteToken(jwt).ToString();

                                HttpContext.Response.Cookies.Append("jwtToken", new JwtSecurityTokenHandler().WriteToken(jwt).ToString(),
                                 new CookieOptions
                                 {
                                     MaxAge = TimeSpan.FromMinutes(60)
                                 });


                                return Ok(JWTTo);

                            }
                        }

                    }
                }
                return NotFound();
            }

            [HttpPost("LogOut")]
            [Authorize]

            public async Task<ActionResult> Logout()
            {
                HttpContext.Response.Cookies.Delete(".AspNetCore.Application.Id");
                return Ok();
            }


            //Возврат статуса авторизации
            [HttpGet("IsAuthorized")]
            [Authorize]            
            public async Task<ActionResult> IsAuthorized()
            {
                return Ok();
            }

        }


    } 
