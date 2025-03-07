using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class AuthorizationService : ICustomAuthorizationService
    {
        public bool CanEditArticle(User user, Article article)
        {
            return user.Role == "Editor" || (user.Role == "Writer" && user.Id == article.AuthorId);
        }

        public bool CanDeleteComment(User user, Comment comment)
        {
            return user.Role == "Editor" || user.Id == comment.UserId;
        }
    }
}
