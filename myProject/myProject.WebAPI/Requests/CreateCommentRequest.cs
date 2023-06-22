using System.ComponentModel.DataAnnotations;

namespace myProject.WebAPI.Requests
{
    public class CreateCommentRequest
    {
        public string Content { get; set; }
        [Required]
        public int UserId { get; set; }
        //public string User { get; set; }
        //public string Avatar { get; set; }
        [Required]
        public int ArticleId { get; set; }
    }
}
