using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MSGMicroservice.IDP.Infrastructure.Common;
using MSGMicroservice.IDP.Infrastructure.ViewModels;

namespace MSGMicroservice.IDP.Infrastructure.Repositories
{
    public interface IUserRepository //: IRepositoryBase<IdentityUser, string>
    {
        // UserManager<User> UserManager { get; }
        // RoleManager<IdentityRole> RoleManager { get; }
        Task<UserDTO> Register(RegisterRequestDTO registerRequestDto);
        Task<UserDTO> RegisterV2(RegisterRequestDTO registerRequestDto);//ko ket noi den he thong Patient
        Task<UserDTO?> DangKyTaiKhoan(RegisterRequestDTO registerRequestDto);
        Task<bool> Update(RegisterRequestDTO registerRequestDto);
        Task<bool> Delete(string id);
        Task<UserDTO> RegisterUser(RegisterRequestDTO registerRequestDto);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<bool> ResetPassword(RegisterRequestDTO model);
        Task<bool> ChangePassword(ChangePwdDto model);
        Task<List<UserDTO>> GetUsers();
        Task<UserDTO> GetUserById(string id);
        Task<PagedResult<UserDTO>> GetUsersPaging(GetCommonPaging request);
        Task<List<RoleDTO>> GetRoles();
        Task<RoleDTO> CreateRole(RoleDTO request);
        Task<bool> UpdateRole(RoleDTO request);
        Task<bool> DeleteRole(string id);
    }
}