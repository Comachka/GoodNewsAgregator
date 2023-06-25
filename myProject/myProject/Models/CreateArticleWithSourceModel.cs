using System.ComponentModel.DataAnnotations;
using myProject.Core.DTOs;

namespace myProject.Models
{
    public class CreateArticleWithSourceModel
    {
        [Required(ErrorMessage = "������� ��������� �������.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "������� ������� �������� �������.")]
        public string ShortDescription { get; set; }
        [Required(ErrorMessage = "������� ���������� �������.")]
        public string Content { get; set; }

        [Required]
        public List<CategoryDto> Categories { get; set; }
        public int CategoryId { get; set; }
    }

}