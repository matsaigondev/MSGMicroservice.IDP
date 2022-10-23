using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MSGMicroservice.IDP.Infrastructure.Domains;
using MSGMicroservice.IDP.Infrastructure.ViewModels;

namespace MSGMicroservice.IDP.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository//, IRepositoryManager
    {
        private readonly DapperContext _dapperContext;
        // private readonly MsgIdentityContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserRepository(IMapper mapper, DapperContext dapperContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));;
            _dapperContext = dapperContext ?? throw new ArgumentNullException(nameof(dapperContext));;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));;
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));;
        }
        
        public async Task<UserDTO?> Register(RegisterRequestDTO registerRequestDto)
        {
            //1. check username exist?
            if (!IsUniqueUser(registerRequestDto.UserName))
                return null;
            //2.Get information from Patients
            var _strUserName = registerRequestDto.UserName;
            var query = $"select top 1 * FROM dbo.Patients where Code=@_strUserName or PhoneNumber=@_strUserName";
            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<PatientDto>(query,
                    new {Code = _strUserName, PhoneNumber = _strUserName}); //
                if (result == null)
                    return null;
                IdentityUser user = new()
                {
                    UserName = string.IsNullOrEmpty(result.PhoneNumber) 
                        ? result.Code : result.PhoneNumber,
                    Email = result.Email,
                    // FirstName = result.FirstName,
                    // LastName = result.LastName,
                    // Address = result.Address,
                    EmailConfirmed = true,
                    Id = Guid.NewGuid().ToString()
                };
                
                try
                {
                    var resultUser = await _userManager.CreateAsync(user, registerRequestDto.Password);
                    if (resultUser.Succeeded)
                    {
                        // if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult()){
                        //     await _roleManager.CreateAsync(new IdentityRole("admin"));
                        //     await _roleManager.CreateAsync(new IdentityRole("customer"));
                        // }
                        await _userManager.AddToRoleAsync(user, "customer");
                        var userToReturn = _userManager.Users
                            .FirstOrDefault(u => u.UserName == registerRequestDto.UserName);
                        return _mapper.Map<UserDTO>(userToReturn);
                    }
                }
                catch(Exception e)
                {
                }

                return new UserDTO();
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
    }
}