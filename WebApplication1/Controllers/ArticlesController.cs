using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;  // Add this import
using WebApplication1.Data;    // Add this import

namespace WebApplication1.Controllers  // Add namespace
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
        [Authorize(Roles = "Writer,Editor")]
        [HttpPost]
        public async Task<ActionResult<Article>> CreateArticle(Article article)
        {
            // Set the author ID to the current user
            article.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, Article article)
        {
            if (id != article.Id)
            {
                return BadRequest();
            }

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

            // Update properties
            existingArticle.Title = article.Title;
            existingArticle.Content = article.Content;

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
