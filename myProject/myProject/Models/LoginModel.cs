using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Укажите Email.")]
        [EmailAddress(ErrorMessage = "Формат не соответствует email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Укажите пароль.")]
        //[RegularExpression()]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
