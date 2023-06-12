using Microsoft.AspNetCore.Mvc;
using myProject.Models.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class MyAccountModel
    {
        [Required]
        public string Name { get; set; }

        public string? Avatar { get; set; }
        
        public string? AboutMyself { get; set; }
        public bool MailNotification { get; set; }
        public int? Raiting { get; set; }
        public string? Role { get; set; }
        public int MyLikes { get; set; }
        public int OnMeLikes { get; set; }
        [ImgChangeValidation(ErrorMessage = "Please select a PNG or JPEG/JPG image smaller than 200kb")]
        [DataType(DataType.Upload)]
        public IFormFile? AvatarChange { get; set; }
    }
}
