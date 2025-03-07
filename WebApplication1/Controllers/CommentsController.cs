using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthorizationService _authService;

    public CommentsController(ApplicationDbContext context, IAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return Ok(comment);
    }
}
