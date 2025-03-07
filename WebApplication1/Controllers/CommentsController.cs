using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Models;  
using WebApplication1.Data;   
using WebApplication1.DTOs;
namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Authorization.IAuthorizationService _authService;

        public CommentsController(
            ApplicationDbContext context, 
            Microsoft.AspNetCore.Authorization.IAuthorizationService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        [Authorize(Roles = "Subscriber,Writer,Editor")]
        [HttpPost]
        public async Task<ActionResult<Comment>> AddComment(CommentCreateDto commentDto)
        {
            // Check if the article exists
            var article = await _context.Articles.FindAsync(commentDto.ArticleId);
            if (article == null)
            {
                return BadRequest("Article not found");
            }

            // Create the comment from the DTO
            var comment = new Comment
            {
                Content = commentDto.Content,
                ArticleId = commentDto.ArticleId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
                CreatedAt = DateTime.UtcNow
            };
            
            // Check authorization
            var requirement = new OperationAuthorizationRequirement { Name = CommentOperations.Create };
            var authResult = await _authService.AuthorizeAsync(User, comment, requirement);
                
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, CommentUpdateDto commentDto)
        {
            var existingComment = await _context.Comments.FindAsync(id);
            
            if (existingComment == null)
            {
                return NotFound();
            }

            // Check authorization
            var requirement = new OperationAuthorizationRequirement { Name = CommentOperations.Update };
            var authResult = await _authService.AuthorizeAsync(User, existingComment, requirement);

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            // Update only the content
            existingComment.Content = commentDto.Content;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            // Check authorization
            var requirement = new OperationAuthorizationRequirement { Name = CommentOperations.Delete };
            var authResult = await _authService.AuthorizeAsync(User, comment, requirement);

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
