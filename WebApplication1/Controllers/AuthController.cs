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
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TokenService tokenService,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
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
            // Validate the input
            if (string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password))
            {
                return BadRequest("Username and password are required");
            }

            // Normalize the role to proper case
            string normalizedRole = NormalizeRoleName(registerDto.Role);
            
            // Check if role exists
            if (!await _roleManager.RoleExistsAsync(normalizedRole))
            {
                return BadRequest($"Role '{registerDto.Role}' does not exist. Valid roles are: Editor, Writer, Subscriber, Guest.");
            }
            
            var user = new User
            {
                UserName = registerDto.Username,
                Role = normalizedRole 
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Assign role using the normalized role name
            await _userManager.AddToRoleAsync(user, normalizedRole);

            return Ok(new { message = "User registered successfully" });
        }

        // Helper method to normalize role names
        private string NormalizeRoleName(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return "Guest"; // Default role
            
            // Convert to lowercase first
            roleName = roleName.ToLower();
            
            // Then capitalize first letter
            return char.ToUpper(roleName[0]) + roleName.Substring(1);
        }
    }
}