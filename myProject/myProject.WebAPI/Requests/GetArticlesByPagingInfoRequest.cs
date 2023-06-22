using System.ComponentModel.DataAnnotations;

namespace myProject.WebAPI.Requests
{
    
    public class GetArticlesByPagingInfoRequest
    {
        [Required]
        public int Page { get; set; }
        [Required]
        public int PageSize { get; set; }
        [Required]
        public double MinRaiting { get; set; }
    }
}
