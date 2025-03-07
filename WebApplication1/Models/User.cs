using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class User : IdentityUser
{
    // Remove Id as it's inherited from IdentityUser
    // IdentityUser already includes Username as UserName property
    // IdentityUser already includes PasswordHash
    
    // Custom property
    public string Role { get; set; } = "Guest"; // Editor, Writer, Subscriber, Guest
    
    public List<Article>? Articles { get; set; }
    public List<Comment>? Comments { get; set; }
}
