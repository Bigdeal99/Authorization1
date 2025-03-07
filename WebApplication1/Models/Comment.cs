using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;


public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public int ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
