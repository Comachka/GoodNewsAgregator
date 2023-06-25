using System.ComponentModel.DataAnnotations;
using myProject.Core.DTOs;

namespace myProject.Models
{
    public class CreateArticleWithSourceModel
    {
        [Required(ErrorMessage = "Укажите заголовок новости.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Укажите краткое описание новости.")]
        public string ShortDescription { get; set; }
        [Required(ErrorMessage = "Укажите содержимое новости.")]
        public string Content { get; set; }

        [Required]
        public List<CategoryDto> Categories { get; set; }
        public int CategoryId { get; set; }
    }

}