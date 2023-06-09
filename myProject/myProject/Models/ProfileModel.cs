using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class ProfileModel
    {
        public string Name { get; set; }

        public string? Avatar { get; set; }
        
        public string? AboutMyself { get; set; }
        public int? Raiting { get; set; }
        public int MyLikes { get; set; }
        public int OnMeLikes { get; set; }
    }
}
