using System.ComponentModel.DataAnnotations;

namespace myProject.WebAPI.Requests
{
    public class UpdateProfileRequest
    {
        public string? Name { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? AboutMyself { get; set; }
        public bool MailNotification { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? AvatarChange { get; set; }
    }
}
