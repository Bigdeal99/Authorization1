using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class User : IdentityUser
    {
        public string Role { get; set; } = "Guest"; 
        
        public List<Article>? Articles { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
