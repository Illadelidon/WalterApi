using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalterApi.Core.DTO_s.User;
using WalterApi.Core.Services;
using WalterApi.Core.Validations;

namespace WalterApi.Api.Controllers
{



    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        
        private readonly UserService _userService;
        
        public UserController(UserService userService)
        {
            
            _userService = userService;
        }
        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserDto model)
        {
            var validator = new CreateUserValidation();
            var validationResult = await validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {
                var result = await _userService.CreateAsync(model);


                return Ok(result);
            }

            else
            {
                return BadRequest(validationResult.Errors);
            }
    }


       
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserDto model)
        {
           var result = await _userService.LoginUserAsync(model);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUserAsync([FromBody] string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [AllowAnonymous]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserDto model)
        {
            var validator = new UpdateUserValidation();
            var validationResult = await validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {
            var result = await _userService.UpdateUserAsync(model);
            return Ok(result);
            }
            else
            {
                return BadRequest(validationResult.Errors);
            }
        }
    }
}
