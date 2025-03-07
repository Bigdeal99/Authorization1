public interface IAuthorizationService
{
    bool CanEditArticle(User user, Article article);
    bool CanDeleteComment(User user, Comment comment);
}
