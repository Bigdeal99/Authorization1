using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;

public class ArticleOperations
{
    public static readonly string Create = "ArticleCreate";
    public static readonly string Read = "ArticleRead";
    public static readonly string Update = "ArticleUpdate";
    public static readonly string Delete = "ArticleDelete";
}

public class ArticleAuthorizationHandler : 
    AuthorizationHandler<OperationAuthorizationRequirement, Article>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        OperationAuthorizationRequirement requirement, 
        Article resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (context.User.IsInRole("Editor"))
        {
            // Editors can do everything
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        // Writers can create new articles and edit/delete their own
        if (context.User.IsInRole("Writer"))
        {
            if (requirement.Name == ArticleOperations.Create)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            
            if ((requirement.Name == ArticleOperations.Update || 
                 requirement.Name == ArticleOperations.Delete) && 
                resource.AuthorId == userId)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        
        // Everyone can read articles
        if (requirement.Name == ArticleOperations.Read)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        return Task.CompletedTask;
    }
}