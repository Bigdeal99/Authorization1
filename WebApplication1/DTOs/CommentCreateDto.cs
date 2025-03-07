namespace WebApplication1.DTOs
{
    public class CommentCreateDto
    {
        public int ArticleId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}