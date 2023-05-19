using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class CommentModel
    {
        public int? UserId { get; set; }
        public string? User { get; set; }
        public string? Avatar { get; set; }
        [Required]
        public string Content { get; set; }
        public int? Raiting { get; set; }
        public DateTime? DateCreated { get; set; }
        public int ArticleId { get; set; }
    }
}