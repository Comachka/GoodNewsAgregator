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
        [ImgValidation(ErrorMessage = "Пожалуйства выберите аватар формата PNG или JPEG/JPG размером до 200кб")]
        [DataType(DataType.Upload)]
        public IFormFile? AvatarChange { get; set; }
    }
}
