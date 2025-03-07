public class AuthorizationService : IAuthorizationService
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
