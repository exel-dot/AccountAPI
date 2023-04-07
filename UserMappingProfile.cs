using AccountAPI.Entities;
using AccountAPI.Models;
using AutoMapper;

namespace AccountAPI
{
    public class UserMappingProfile :Profile
    { 
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, UserInformationsdto>();
               
        }
       
    }
}
