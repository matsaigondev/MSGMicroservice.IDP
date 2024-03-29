﻿using System;
using System.Collections.Generic;
using System.Data;
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
using MSGMicroservice.IDP.Infrastructure.Common;
using MSGMicroservice.IDP.Infrastructure.Domains;
using MSGMicroservice.IDP.Infrastructure.Entities;
using MSGMicroservice.IDP.Infrastructure.Exceptions;
using MSGMicroservice.IDP.Infrastructure.Repositories.Interfaces;
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
        
        public async Task<UserDTO?> RegisterV2(RegisterRequestDTO registerRequestDto)
        {
            //1. check username exist?
            if (!IsUniqueUser(registerRequestDto.UserName))
                return null;
            //2.Get information from Patients
            try
            {
                if (string.IsNullOrEmpty(registerRequestDto.PhoneNumber) &&
                    string.IsNullOrEmpty(registerRequestDto.Email))
                    return null;
                var _strUserName = string.IsNullOrEmpty(registerRequestDto.PhoneNumber)
                    ? registerRequestDto.Email
                    : registerRequestDto.PhoneNumber;
                var _strEmail = string.IsNullOrEmpty(registerRequestDto.Email)
                    ? DateTime.Now.ToString("yyyyMMddHHmmsss") + "@noemail.com"
                    : registerRequestDto.Email;

                User user = new()
                {
                    UserName = _strUserName,
                    Email = _strEmail,
                    FirstName = registerRequestDto.FirstName.ToUpper(),
                    LastName = registerRequestDto.LastName.ToUpper(),
                    Address = string.IsNullOrEmpty(registerRequestDto.Address)?"":registerRequestDto.Address,
                    EmailConfirmed = true,
                    PhoneNumber = registerRequestDto.PhoneNumber,
                    Id = Guid.NewGuid().ToString(),
                    HospitalId = registerRequestDto.HospitalId
                };

                try
                {
                    registerRequestDto.Password = "P@ssw0rd321!";
                    var resultUser = await _userManager.CreateAsync(user, registerRequestDto.Password);
                    if (resultUser.Succeeded)
                    {
                        if (!_roleManager.RoleExistsAsync("Administration").GetAwaiter().GetResult())
                        {
                            await _roleManager.CreateAsync(new IdentityRole("administration"));
                            await _roleManager.CreateAsync(new IdentityRole("customer"));
                        }

                        if (registerRequestDto.Roles.Count() == 0)
                            await _userManager.AddToRoleAsync(user, "customer");
                        else
                        {
                            foreach (var i in registerRequestDto.Roles)
                            {
                                var roleName = _roleManager.Roles.Where(x => x.Id.Equals(i)).FirstOrDefault();
                                _userManager.AddToRoleAsync(user, roleName.Name);
                            }
                        }

                        var userToReturn = _userManager.Users
                            .FirstOrDefault(u => u.UserName == _strUserName);
                        
                        //update name
                        var sql = "update [MSGIdentity].[Identity].[Users] set FirstName = @firstName, LastName = @lastName where Id = @id";
                        using (var connection = _dapperContext.CreateConnection())
                        {
                            var dynamicParam = new DynamicParameters();
                            dynamicParam.Add("@firstName", registerRequestDto.FirstName.ToUpper());
                            dynamicParam.Add("@lastName", registerRequestDto.LastName.ToUpper());
                            dynamicParam.Add("@id", user.Id);
                            await connection.ExecuteAsync(sql, dynamicParam);
                        }
                        return _mapper.Map<UserDTO>(userToReturn);
                    }
                }
                catch (Exception e)
                {
                }

                return new UserDTO();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new UserDTO();
            }
        }

        public async Task<UserDTO?> DangKyTaiKhoan(RegisterRequestDTO registerRequestDto)
        {
            //1. check username exist?
            if (!IsUniqueUser(registerRequestDto.UserName))
                return null;
            //2.Get information from Patients
            try
            {
                if (string.IsNullOrEmpty(registerRequestDto.PhoneNumber) &&
                    string.IsNullOrEmpty(registerRequestDto.Email))
                    return null;
                var _strUserName = string.IsNullOrEmpty(registerRequestDto.PhoneNumber)
                    ? registerRequestDto.Email
                    : registerRequestDto.PhoneNumber;
                var _strEmail = string.IsNullOrEmpty(registerRequestDto.Email)
                    ? DateTime.Now.ToString("yyyyMMddHHmmsss") + "@noemail.com"
                    : registerRequestDto.Email;

                User user = new()
                {
                    UserName = _strUserName,
                    Email = _strEmail,
                    FirstName = registerRequestDto.FirstName.ToUpper(),
                    LastName = registerRequestDto.LastName.ToUpper(),
                    Address = string.IsNullOrEmpty(registerRequestDto.Address) ? "" : registerRequestDto.Address,
                    EmailConfirmed = true,
                    PhoneNumber = registerRequestDto.PhoneNumber,
                    Id = Guid.NewGuid().ToString(),
                    HospitalId = registerRequestDto.HospitalId
                };

                try
                {
                    registerRequestDto.Password = "P@ssw0rd321!";
                    var resultUser = await _userManager.CreateAsync(user, registerRequestDto.Password);
                    if (resultUser.Succeeded)
                    {
                        //if (!_roleManager.RoleExistsAsync("Administration").GetAwaiter().GetResult())
                        //{
                        //    await _roleManager.CreateAsync(new IdentityRole("administration"));
                        //    await _roleManager.CreateAsync(new IdentityRole("customer"));
                        //}

                        //if (registerRequestDto.Roles.Count() == 0)
                        //    await _userManager.AddToRoleAsync(user, "customer");
                        //else
                        //{
                        //    foreach (var i in registerRequestDto.Roles)
                        //    {
                        //        var roleName = _roleManager.Roles.Where(x => x.Id.Equals(i)).FirstOrDefault();
                        //        _userManager.AddToRoleAsync(user, roleName.Name);
                        //    }
                        //}

                        var userToReturn = _userManager.Users
                            .FirstOrDefault(u => u.UserName == _strUserName);

                        //update name
                        var sql = "update [MSGIdentity].[Identity].[Users] set FirstName = @firstName, LastName = @lastName where Id = @id";
                        using (var connection = _dapperContext.CreateConnection())
                        {
                            var dynamicParam = new DynamicParameters();
                            dynamicParam.Add("@firstName", registerRequestDto.FirstName.ToUpper());
                            dynamicParam.Add("@lastName", registerRequestDto.LastName.ToUpper());
                            dynamicParam.Add("@id", user.Id);
                            await connection.ExecuteAsync(sql, dynamicParam);
                        }
                        userToReturn.PhoneNumber = registerRequestDto.Password;
                        return _mapper.Map<UserDTO>(userToReturn);
                    }
                }
                catch (Exception e)
                {
                }

                return new UserDTO();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new UserDTO();
            }
        }

        public async Task<bool> Update(RegisterRequestDTO registerRequestDto)
        {
            try
            {
                var user = await _userManager.Users.Where(x => x.UserName.Equals(registerRequestDto.UserName))
                    .FirstOrDefaultAsync();
                // user.FirstName = registerRequestDto.FirstName.ToUpper();
                // user.LastName = registerRequestDto.LastName.ToUpper();
                user.Address = registerRequestDto.Address;
                user.Email = registerRequestDto.Email;
                user.PhoneNumber = registerRequestDto.PhoneNumber;
                user.HospitalId = registerRequestDto.HospitalId;
                // if (!IsUniqueUser(registerRequestDto.PhoneNumber) || !IsUniqueEmail(registerRequestDto.Email))
                //     return false;
                var result = await _userManager.UpdateAsync(user);
                //if role <> current
                //for multi roles
                if (!ListExtensions.SetwiseEquivalentTo(registerRequestDto.OldRoles, registerRequestDto.Roles))
                {
                    if (registerRequestDto.OldRoles != null)
                    {
                        foreach (var i in registerRequestDto.OldRoles)
                        {
                            //for a role
                            //var _userRole = _userManager.IsInRoleAsync(user, registerRequestDto.Role).Result;
                            var getCurrentRoleOld = await _roleManager.Roles.Where(x => x.Id.Equals(i))
                                .FirstOrDefaultAsync();
                            if (getCurrentRoleOld != null)
                                await _userManager.RemoveFromRoleAsync(user, getCurrentRoleOld.Name);
                            //var _userRole = _userManager.IsInRoleAsync(user, registerRequestDto.Role).Result;
                            //if (!_userRole)
                            //{
                            //    //changed another role need to update
                            //    //1.remove first
                            //    var getRoleOld = await _roleManager.Roles.Where(x => x.Id.Equals(registerRequestDto.OldRole))
                            //        .FirstOrDefaultAsync();
                            //    if (getRoleOld != null)
                            //        await _userManager.RemoveFromRoleAsync(user, getRoleOld.Name);

                            //    //2.add new role
                            //    var getRole = await _roleManager.Roles.Where(x => x.Id.Equals(registerRequestDto.Role))
                            //        .FirstOrDefaultAsync();
                            //    await _userManager.AddToRoleAsync(user, getRole.Name);
                            //}
                        }
                    }
                    foreach (var j in registerRequestDto.Roles)
                    {
                        //2.add new role
                        var getNewRole = await _roleManager.Roles.Where(x => x.Id.Equals(j))
                            .FirstOrDefaultAsync();
                        await _userManager.AddToRoleAsync(user, getNewRole.Name);
                    }
                }
                if (result.Succeeded)
                {
                    var sql = "update [MSGIdentity].[Identity].[Users] set FirstName = @firstName, LastName = @lastName where Id = @id";
                    using (var connection = _dapperContext.CreateConnection())
                    {
                        var dynamicParam = new DynamicParameters();
                        dynamicParam.Add("@firstName", registerRequestDto.FirstName.ToUpper());
                        dynamicParam.Add("@lastName", registerRequestDto.LastName.ToUpper());
                        dynamicParam.Add("@id", user.Id);
                        await connection.ExecuteAsync(sql, dynamicParam);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return false;
        }

        public async Task<bool> Delete(string id)
        {
            try
            {
                var user = await _userManager.Users.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
                //delete role user:
                var roles = await _roleManager.Roles.ToListAsync();
                foreach (var item in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, item.Name);
                }
                //delete user:
                await _userManager.DeleteAsync(user);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
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
        
        private bool IsUniqueEmail(string email)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                return true;
            }
            return false;
        }
        
        private bool IsUniqueRole(string roleName)
        {
            var role = _roleManager.Roles.FirstOrDefault(x => x.Name == roleName);
            if (role == null)
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
                Expires = DateTime.Now.AddDays(7),
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
            if (model.PhoneNumber != "MSG@2024")
                return false;

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

        public async Task<bool> CreatePinCode(ChangePwdDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            bool isValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!isValid)
            {
                return false;
            }

            var _strEncrypt = StringHeplers.Encrypt(model.OldPassword, "J@s0n1984!");
            var _strDecrypt = StringHeplers.Decrypt(_strEncrypt,"J@s0n1984!");
            var _mahoaPinCode = StringHeplers.MD5Hash(model.NewPassword);
            var _sqlIns = $@"DELETE MSG_DWH.dbo.Users WHERE userid='{user.Id}'
                            INSERT INTO MSG_DWH.dbo.Users
                            (
                                userid,
                                secret,
                                pincode
                            )
                            VALUES
                            (   '{user.Id}', -- userid - uniqueidentifier
                                '{_strEncrypt}', -- secret - varchar(250)
                                '{_mahoaPinCode}'  -- pincode - varchar(150)
                                )";
            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {
                    await connection.ExecuteAsync(_sqlIns);
                }    
            }
            catch { }
            return true;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            var user = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<UserDTO>>(user);
        }

        public async Task<UserDTO> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = _mapper.Map<UserDTO>(user);
            var roles = _userManager.GetRolesAsync(user).Result;
            if (roles.Count > 0)
            {
                result.Roles = roles;
                result.Role = roles[0];
                result.OldRole = roles[0];
            }

            return result;
        }

        public async Task<PagedResult<UserDTO>> GetUsersPaging(GetCommonPaging request)
        {
            var user = await _userManager.Users.Where(x=>x.UserName!="alice@example.com")
                .ToListAsync();

            var result = _mapper.Map<List<UserDTO>>(user);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                result = result.Where(x => x.UserName.ToLower().Contains(request.Keyword.ToLower())
                                           ||x.FirstName.ToLower().Contains(request.Keyword.ToLower())||x.LastName.ToLower().Contains(request.Keyword.ToLower())).ToList();
            }

            int totalRow = result.Count();

            result = result.OrderByDescending(x => x.UserName)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize).ToList();

            var _data = result;
            foreach (var item in result)
            {
                var usr = await _userManager.Users.Where(x => x.UserName == item.UserName)
                .FirstOrDefaultAsync();
                IList<string> userRoles = await _userManager.GetRolesAsync(usr);
                List<string> roles = new List<string>();
                foreach (var role in userRoles)
                {
                    var getRole = await _roleManager.Roles.Where(x => x.Id.Equals(role)).FirstOrDefaultAsync();
                    roles.Add(getRole.Name);
                }
                item.OldRoles = roles;
            }


            var pagedResult = new PagedResult<UserDTO>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Data = result
            };

            return pagedResult;
        }

        public async Task<List<RoleDTO>> GetRoles()
        {
            var roles = await _roleManager.Roles.OrderBy(x=>x.Name).ToListAsync();
            var result = _mapper.Map<List<RoleDTO>>(roles);
            return result;
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

        public async Task<RoleDTO> CreateRole(RoleDTO request)
        {
            //1. check rolename exist?
            if (!IsUniqueRole(request.Name))
                return null;
            
            try
            {
                var resultRole = await _roleManager.CreateAsync(new IdentityRole(request.Name));
                if (resultRole.Succeeded)
                {
                    var roleToReturn = _roleManager.Roles
                        .FirstOrDefault(u => u.Name == request.Name);
                    return _mapper.Map<RoleDTO>(roleToReturn);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            return new RoleDTO();
        }

        public async Task<bool> UpdateRole(RoleDTO request)
        {
            try
            {
                var getRole = await _roleManager.Roles.Where(x => x.Id.Equals(request.Id)).FirstOrDefaultAsync();
                getRole.Name = request.Name;
                var resultRole = await _roleManager.UpdateAsync(getRole);
                
                if (resultRole.Succeeded)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return false;
        }

        public async Task<bool> DeleteRole(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                var user = _userManager.GetUsersInRoleAsync(role.Name).Result;
                if (user.Count() == 0)
                    await _roleManager.DeleteAsync(role);
                else 
                    return false;
                
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}