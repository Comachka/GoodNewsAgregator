using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class ArticleDetailsModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public double PositiveRaiting { get; set; }
        public string SourceName { get; set; }
        public string ArticleSourceUrl { get; set; }
    }

}