using System.ComponentModel.DataAnnotations;

namespace myProject.WebAPI.Requests
{
    public class UpdateArticleRequest
    {
        [Required]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public double? PositiveRaiting { get; set; }
        public int NewsResourceId { get; set; }
        public string ArticleSourceUrl { get; set; }
        public int CategoryId { get; set; }
    }
}
