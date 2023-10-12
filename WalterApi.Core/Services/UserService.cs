using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalterApi.Core.DTO_s.Token;
using WalterApi.Core.DTO_s.User;
using WalterApi.Core.Entities.User;

namespace WalterApi.Core.Services
{
    public class UserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;

        public UserService(RoleManager<IdentityRole> roleManager, IConfiguration config, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _config = config;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }


        public async Task<ServiceResponse> GetAllAsync()
        {
            List<AppUser> users = await _userManager.Users.ToListAsync();
            List<UsersDto> mappedUsers = users.Select(u => _mapper.Map<AppUser, UsersDto>(u)).ToList();

            for (int i = 0; i < users.Count; i++)
            {
                mappedUsers[i].Role = (await _userManager.GetRolesAsync(users[i])).FirstOrDefault();
            }


            return new ServiceResponse
            {
                Success = true,
                Message = "All users loaded.",
                Payload = mappedUsers
            };
        }



        public async Task<ServiceResponse> CreateAsync(CreateUserDto model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                return new ServiceResponse
                {
                    Message = "Confirm pssword do not match",
                    Success = false
                };
            }

            var newUser = _mapper.Map<CreateUserDto, AppUser>(model);
            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, model.Role);

                //await SendConfirmationEmailAsync(newUser);

                var tokens = await _jwtService.GenerateJwtTokensAsync(newUser);

                return new ServiceResponse
                {
                    Message = "User successfully created.",
                    Success = true
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Message = "Error user not created.",
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

        }


        public async Task<ServiceResponse> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new ServiceResponse { Success = false, Message = "User not found." };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var mappedUser = _mapper.Map<AppUser, EditUserDto>(user);
            mappedUser.Role = roles[0];

            return new ServiceResponse
            {
                Success = true,
                Message = "User loaded!",
                Payload = mappedUser
            };
        }





        public async Task<ServiceResponse> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "User successfullt deleted."
                };
            }

            return new ServiceResponse
            {
                Success = false,
                Message = "Sonething wring. Connect with your admin.",
                Errors = result.Errors.Select(e => e.Description)
            };
        }





        public async Task<ServiceResponse> LoginUserAsync(LoginUserDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Message = "Login or password incorrect.",
                    Success = false
                };
            }

            var signinResult = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: true);
            if (signinResult.Succeeded)
            {
                var tokens = await _jwtService.GenerateJwtTokensAsync(user);
                return new ServiceResponse
                {
                    AccessToken = tokens.Token,
                    RefreshToken = tokens.refreshToken.Token,
                    Message = "User signed in successfully.",
                    Success = true
                };
            }

            if (signinResult.IsNotAllowed)
            {
                return new ServiceResponse
                {
                    Message = "Confirm your email please.",
                    Success = false
                };
            }

            if (signinResult.IsLockedOut)
            {
                return new ServiceResponse
                {
                    Message = "User is blocked connect to support.",
                    Success = false
                };
            }

            return new ServiceResponse
            {
                Message = "Login or password incorrect.",
                Success = false
            };
        }

        public async Task<ServiceResponse> RefreshTokenAsync(TokenRequestDto model)
        {
            return await _jwtService.VerifyTokenAsync(model);
        }

        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }
        /* public async Task LogoutUserAsync()
         {
             await _signInManager.SignOutAsync();
         }

         public async Task<List<IdentityRole>> LoadRoles()
         {
             var roles = await _roleManager.Roles.ToListAsync();
             return roles;
         }*/

        public async Task<ServiceResponse> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new ServiceResponse
                {
                    Message = "User not found.",
                    Success = false
                };
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new ServiceResponse
                {
                    Message = "User successfully deleted.",
                    Success = true
                };
            }

            return new ServiceResponse
            {
                Message = "User NOT deleted.",
                Success = false
            };
        }
    }
}
