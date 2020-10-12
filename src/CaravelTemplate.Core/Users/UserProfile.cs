using AutoMapper;
using CaravelTemplate.Infrastructure.Identity;

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