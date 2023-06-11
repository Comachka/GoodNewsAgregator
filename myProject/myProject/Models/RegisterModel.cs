using Microsoft.AspNetCore.Mvc;
using myProject.Models.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [Remote("CheckIsUserEmailIsValidAndNotExists",
        "Account",
        ErrorMessage = "This email already used")]
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

        [ImgValidation(ErrorMessage = "Please select a PNG or JPEG/JPG image smaller than 200kb")]
        [DataType(DataType.Upload)]
        public IFormFile? Avatar { get; set; }

        public string AboutMyself { get; set; }
    }
}
