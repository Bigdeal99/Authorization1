using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ArticlesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthorizationService _authService;

    public ArticlesController(ApplicationDbContext context, IAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateArticle(Article article)
    {
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
        return Ok(article);
    }
}
