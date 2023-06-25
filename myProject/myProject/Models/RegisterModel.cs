using Microsoft.AspNetCore.Mvc;
using myProject.Models.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Укажите email.")]
        [EmailAddress(ErrorMessage = "Формат не соответствует email.")]
        [Remote("CheckIsUserEmailIsValidAndNotExists",
        "Account",
        ErrorMessage = "Такой email уже уществует.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Укажите пароль.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?!.*\s).{6,16}$",
            ErrorMessage = "Пароль должен содержать одну строчную букву, одну заглавную букву, одну цифру, длина пароля 6-16 символов без пробелов.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Укажите подтверждение пароля.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают.")]
        public string PasswordConfirmation { get; set; }

        [Required(ErrorMessage = "Укажите Имя.")]
        public string Name { get; set; }

        [Required]
        public bool MailNotification { get; set; }

        [ImgValidation(ErrorMessage = "Пожалуйства выберите аватар формата PNG или JPEG/JPG размером до 200кб")]
        [DataType(DataType.Upload)]
        public IFormFile? Avatar { get; set; }

        public string AboutMyself { get; set; }
    }
}
