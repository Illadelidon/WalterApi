using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalterApi.Core.DTO_s.User;
using WalterApi.Core.Services;
using WalterApi.Core.Validations;

namespace WalterApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        UserService _userService;
        
        public UserController(UserService userService)
        {
            
            _userService = userService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result.Payload);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto model)
        {
            var validator = new CreateUserValidation();
            var validationResult = await validator.ValidateAsync(model);
            if (validationResult.IsValid)
            {
                var result = await _userService.CreateAsync(model);
                if (result.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                
                return Ok(model);
            }
          
            return Ok(model);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await _userService.GetByIdAsync(Id);
            return Ok(user.Payload);
        }
    }
}
