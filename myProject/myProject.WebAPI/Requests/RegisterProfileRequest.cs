using System.ComponentModel.DataAnnotations;

namespace myProject.WebAPI.Requests
{
    public class RegisterProfileRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[RegularExpression()]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string PasswordConfirmation { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool MailNotification { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? Avatar { get; set; }

        public string AboutMyself { get; set; }
    }
}
