using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class ArticleDetailsModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Заголовок не может быть пустым.")]
        public string Title { get; set; }
        public string Category { get; set; }
        [Required(ErrorMessage = "Короткое описание не может быть пустым.")]
        public string ShortDescription { get; set; }
        [Required(ErrorMessage = "Содержимое не может быть пустым.")]
        public string Content { get; set; }
        public double PositiveRaiting { get; set; }
        public string SourceName { get; set; }
        public DateTime DatePosting { get; set; }
        public string ArticleSourceUrl { get; set; }
    }

}