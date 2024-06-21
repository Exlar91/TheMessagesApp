using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMessages.EntityModels;
using TheMessages.ModelsDTO.Users;

namespace TheMessagesServices.MapperPrifiles
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserCreateDTO>().ReverseMap();
        }
    }
}
