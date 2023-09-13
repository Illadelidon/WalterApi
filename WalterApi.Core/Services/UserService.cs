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

        public UserService(RoleManager<IdentityRole> roleManager, IConfiguration config, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _config = config;
            _roleManager = roleManager;
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
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                return new ServiceResponse
                {
                    Message = "User exists.",
                    Success = false,
                };
            }

            var mappedUser = _mapper.Map<CreateUserDto, AppUser>(model);
            IdentityResult result = await _userManager.CreateAsync(mappedUser, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(mappedUser, model.Role);

                //await SendConfirmationEmail(mappedUser);

                return new ServiceResponse
                {
                    Message = "User successfully created.",
                    Success = true,
                };
            }

            List<IdentityError> errorList = result.Errors.ToList();

            string errors = "";
            foreach (var error in errorList)
            {
                errors = errors + error.Description.ToString();
            }

            return new ServiceResponse
            {
                Message = "User creating error.",
                Success = false,
                Payload = errors
            };

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










        /* public async Task LogoutUserAsync()
         {
             await _signInManager.SignOutAsync();
         }

         public async Task<List<IdentityRole>> LoadRoles()
         {
             var roles = await _roleManager.Roles.ToListAsync();
             return roles;
         }*/
    }
}
