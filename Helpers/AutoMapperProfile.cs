using AutoMapper;
using ImmageAggregatorAPI.Entities;
using ImmageAggregatorAPI.Models.Users;

namespace ImmageAggregatorAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
        }
    }
}