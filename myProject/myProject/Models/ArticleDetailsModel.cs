using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class ArticleDetailsModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "��������� �� ����� ���� ������.")]
        public string Title { get; set; }
        public string Category { get; set; }
        [Required(ErrorMessage = "�������� �������� �� ����� ���� ������.")]
        public string ShortDescription { get; set; }
        [Required(ErrorMessage = "���������� �� ����� ���� ������.")]
        public string Content { get; set; }
        public double PositiveRaiting { get; set; }
        public string SourceName { get; set; }
        public DateTime DatePosting { get; set; }
        public string ArticleSourceUrl { get; set; }
    }

}