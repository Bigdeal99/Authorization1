using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public interface ICustomAuthorizationService
    {
        bool CanEditArticle(User user, Article article);
        bool CanDeleteComment(User user, Comment comment);
    }
}