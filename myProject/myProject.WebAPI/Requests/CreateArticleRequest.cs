using System.ComponentModel.DataAnnotations;

namespace myProject.WebAPI.Requests
{
    public class CreateArticleRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        [Required]
        public double PositiveRaiting { get; set; }
        [Required]
        public int NewsResourceId { get; set; }
        public string ArticleSourceUrl { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
