using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAuthorizationService
    {
        bool CanEditArticle(User user, Article article);
        bool CanDeleteComment(User user, Comment comment);
    }
}
