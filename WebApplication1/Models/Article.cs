using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        
        [ForeignKey("Author")]
        public string AuthorId { get; set; } = null!;
        public User Author { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
        public List<Comment>? Comments { get; set; }
    }
}
