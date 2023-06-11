using Microsoft.AspNetCore.Mvc;
using Serilog.Sinks.File;
using System.ComponentModel.DataAnnotations;

namespace myProject.Models.CustomValidationAttributes
{
    public class ImgChangeValidationAttribute: ValidationAttribute
    {
        public override bool IsValid(object file)
        {
            var img = file as IFormFile;
            if (img == null)
            {
                {
                    return true;
                }
            }
       
            string ext = Path.GetExtension(img.FileName);
            if ((img.Length > 204800) || !((ext == ".png") || (ext == ".jpg") || (ext == ".jpeg")))
            {
                return false;
            }
            return true;
        }
    }
}
