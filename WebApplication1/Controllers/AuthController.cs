using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TokenService _tokenService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if (user == null) return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Invalid password");

            return Ok(new { token = _tokenService.CreateToken(user) });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto registerDto)
        {
            // Validate role
            var validRoles = new[] { "Editor", "Writer", "Subscriber", "Guest" };
            if (!validRoles.Contains(registerDto.Role))
            {
                return BadRequest("Invalid role specified");
            }

            var user = new User
            {
                UserName = registerDto.Username,
                Role = registerDto.Role
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            // Add user to role
            await _userManager.AddToRoleAsync(user, registerDto.Role);

            return Ok(new { token = _tokenService.CreateToken(user) });
        }
    }
}