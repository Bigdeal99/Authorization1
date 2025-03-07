using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;

        public UsersController(
            ApplicationDbContext context, 
            UserManager<User> userManager,
            TokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto registerDto)
        {
            // Update this line to include Editor
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

            return Ok(new { token = _tokenService.CreateToken(user) });
        }
    }
}
