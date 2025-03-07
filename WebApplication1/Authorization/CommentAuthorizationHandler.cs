using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;  
using WebApplication1.Models;
public class CommentOperations
{
    public static readonly string Create = "CommentCreate";
    public static readonly string Update = "CommentUpdate";
    public static readonly string Delete = "CommentDelete";
}

public class CommentAuthorizationHandler : 
    AuthorizationHandler<OperationAuthorizationRequirement, Comment>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        OperationAuthorizationRequirement requirement, 
        Comment resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Editors can edit/delete any comment
        if (context.User.IsInRole("Editor"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        // Users can create comments if they are subscribers or writers
        if (requirement.Name == CommentOperations.Create && 
            (context.User.IsInRole("Subscriber") || context.User.IsInRole("Writer")))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        // Users can only edit/delete their own comments
        if ((requirement.Name == CommentOperations.Update || 
             requirement.Name == CommentOperations.Delete) && 
            resource.UserId == userId)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
        
        return Task.CompletedTask;
    }
}