using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!; // Editor, Writer, Subscriber, Guest

    public List<Article>? Articles { get; set; }
    public List<Comment>? Comments { get; set; }
}
