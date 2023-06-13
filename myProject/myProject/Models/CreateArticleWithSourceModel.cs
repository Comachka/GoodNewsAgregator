using System.ComponentModel.DataAnnotations;
using myProject.Core.DTOs;

namespace myProject.Models
{
    public class CreateArticleWithSourceModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string Content { get; set; }

        [Required]
        public List<CategoryDto> Categories { get; set; }
        public int CategoryId { get; set; }
    }

}