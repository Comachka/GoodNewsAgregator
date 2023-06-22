using System.ComponentModel.DataAnnotations;

namespace myProject.WebAPI.Requests
{
    public class SubscribeRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int SubscribeId { get; set; }
    }
}
