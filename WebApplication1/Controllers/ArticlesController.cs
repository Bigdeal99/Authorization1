using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.DTOs; 
using WebApplication1.Models;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Authorization.IAuthorizationService _authService;

        public ArticlesController(
            ApplicationDbContext context, 
            Microsoft.AspNetCore.Authorization.IAuthorizationService authService)
        {
            _context = context;
            _authService = authService;
        }

        // Everyone can read articles (no authorization)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            return await _context.Articles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Comments)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        // Only Writers and Editors can create articles
        [HttpPost]
        [Authorize(Roles = "Editor,Writer")]
        public async Task<ActionResult<Article>> CreateArticle(ArticleCreateDto articleDto)
        {
            // Create a new article with only the essential information
            var article = new Article
            {
                Title = articleDto.Title,
                Content = articleDto.Content,
                // Set the current user as the author
                AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, ArticleUpdateDto articleDto)
        {
            var existingArticle = await _context.Articles.FindAsync(id);
            
            if (existingArticle == null)
            {
                return NotFound();
            }

            // Check authorization
            var requirement = new OperationAuthorizationRequirement { Name = ArticleOperations.Update };
            var authResult = await _authService.AuthorizeAsync(User, existingArticle, requirement);

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            
            existingArticle.Title = articleDto.Title;
            existingArticle.Content = articleDto.Content;
            
            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            // Check authorization
            var requirement = new OperationAuthorizationRequirement { Name = ArticleOperations.Delete };
            var authResult = await _authService.AuthorizeAsync(User, article, requirement);

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
