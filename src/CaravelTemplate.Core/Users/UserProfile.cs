using AutoMapper;
using CaravelTemplate.Entities;

namespace CaravelTemplate.Core.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserModel>();
        }
    }
}