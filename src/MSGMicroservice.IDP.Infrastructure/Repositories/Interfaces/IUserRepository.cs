using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MSGMicroservice.IDP.Infrastructure.ViewModels;

namespace MSGMicroservice.IDP.Infrastructure.Repositories
{
    public interface IUserRepository //: IRepositoryBase<IdentityUser, string>
    {
        // UserManager<User> UserManager { get; }
        // RoleManager<IdentityRole> RoleManager { get; }
        Task<UserDTO> Register(RegisterRequestDTO registerRequestDto);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    }
}