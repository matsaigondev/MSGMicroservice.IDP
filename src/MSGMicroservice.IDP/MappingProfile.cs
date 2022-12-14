using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MSGMicroservice.IDP.Infrastructure.Entities;
using MSGMicroservice.IDP.Infrastructure.ViewModels;

namespace MSGMicroservice.IDP
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Permission, PermissionUserViewModel>();
            CreateMap<User, UserDTO>();
            CreateMap<IdentityRole, RoleDTO>();
        }
    }
}