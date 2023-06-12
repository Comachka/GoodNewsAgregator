using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class ArticlePreviewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string ShortDescription { get; set; }
        public string SourceName { get; set; }
        [Range(-5, 5)]
        public int PositiveRaiting { get; set; }
        public DateTime DatePosting { get; set; }
    }
    
}