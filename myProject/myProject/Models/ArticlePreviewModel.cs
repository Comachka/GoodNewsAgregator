using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class ArticlePreviewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SourceName { get; set; }
        [Range(-5, 5)]
        public int PositiveRaiting { get; set; }
    }
    
}