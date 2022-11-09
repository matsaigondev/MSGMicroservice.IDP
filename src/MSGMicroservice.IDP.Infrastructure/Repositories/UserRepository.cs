using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MSGMicroservice.IDP.Infrastructure.Domains;
using MSGMicroservice.IDP.Infrastructure.Entities;
using MSGMicroservice.IDP.Infrastructure.ViewModels;
using MSGMicroservice.IDP.Persistence;

namespace MSGMicroservice.IDP.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository//, IRepositoryManager
    {
        private readonly DapperContext _dapperContext;
        // private readonly MsgIdentityContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private string secretKey;

        public UserRepository(IMapper mapper
            , UserManager<User> userManager
            , RoleManager<IdentityRole> roleManager, DapperContext dapperContext, IConfiguration configuration)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _dapperContext = dapperContext;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }
        
        public async Task<UserDTO?> Register(RegisterRequestDTO registerRequestDto)
        {
            //1. check username exist?
            if (!IsUniqueUser(registerRequestDto.UserName))
                return null;
            //2.Get information from Patients
            var _strUserName = registerRequestDto.UserName;
            var query = $"select top 1 * FROM dbo.Patients where Code=@code or PhoneNumber=@phoneNumber";
            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<PatientDto>(query,
                        new {Code = _strUserName, PhoneNumber = _strUserName}); //
                    _strUserName = string.IsNullOrEmpty(result.PhoneNumber)
                        ? result.Code
                        : result.PhoneNumber;
                    var _strEmail = string.IsNullOrEmpty(result.Email)
                        ? DateTime.Now.ToString("yyyyMMddHHmmsss")+"@noemail.com"
                        : result.Email;
                    if (result == null)
                        return null;
                    User user = new()
                    {
                        UserName = _strUserName,
                        Email = _strEmail,
                        FirstName = result.FirstName,
                        LastName = result.LastName,
                        Address = result.Address,
                        EmailConfirmed = true,
                        Id = Guid.NewGuid().ToString()
                    };
                
                    try
                    {
                        var resultUser = await _userManager.CreateAsync(user, registerRequestDto.Password);
                        if (resultUser.Succeeded)
                        {
                            if (!_roleManager.RoleExistsAsync("Administration").GetAwaiter().GetResult()){
                                await _roleManager.CreateAsync(new IdentityRole("administration"));
                                await _roleManager.CreateAsync(new IdentityRole("customer"));
                            }
                            await _userManager.AddToRoleAsync(user, "customer");
                            var userToReturn = _userManager.Users
                                .FirstOrDefault(u => u.UserName == _strUserName);
                            return _mapper.Map<UserDTO>(userToReturn);
                        }
                    }
                    catch(Exception e)
                    {
                    }
            
                    return new UserDTO();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        
        private bool IsUniqueUser(string username)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }
        
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _userManager.Users
                .FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);


            if (user == null || isValid == false)
            {
                return new LoginResponseDTO()
                {
                    access_token = "",
                    User = null
                };
            }

            //if user was found generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                access_token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                
            };
            return loginResponseDTO;
        }

        public async Task<bool> ResetPassword(RegisterRequestDTO model)
        {
            var _findUser = await _userManager.Users.Where(x => x.UserName == model.UserName).FirstOrDefaultAsync();//.FindByNameAsync(model.UserName);
            if (_findUser == null)
                return false;
            
            await _userManager.RemovePasswordAsync(_findUser);
            await _userManager.AddPasswordAsync(_findUser, model.Password);

            return true;
        }

        public async Task<bool> ChangePassword(ChangePwdDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return false;
            }
            return true;
        }

        public async Task<UserDTO> RegisterUser(RegisterRequestDTO registerRequestDto)
        {
            //1. check username exist?
            if (!IsUniqueUser(registerRequestDto.UserName))
                return null;

            User user = new()
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.Email,
                FirstName = registerRequestDto.FirstName,
                LastName = registerRequestDto.LastName,
                Address = registerRequestDto.Address,
                EmailConfirmed = true,
                Id = Guid.NewGuid().ToString()
            };

            try
            {
                var resultUser = await _userManager.CreateAsync(user, registerRequestDto.Password);
                if (resultUser.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("Administration").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("administration"));
                        await _roleManager.CreateAsync(new IdentityRole("customer"));
                    }
                    await _userManager.AddToRoleAsync(user, "customer");
                    var userToReturn = _userManager.Users
                        .FirstOrDefault(u => u.UserName == registerRequestDto.UserName);
                    return _mapper.Map<UserDTO>(userToReturn);
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return new UserDTO();
        }
    }
}