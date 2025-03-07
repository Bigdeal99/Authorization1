namespace WebApplication1.DTOs
{
    public class UserRegistrationDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Guest"; 
    }
}