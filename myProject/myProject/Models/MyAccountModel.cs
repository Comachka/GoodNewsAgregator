using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class MyAccountModel
    {
        public string Name { get; set; }

        public string Avatar { get; set; }

        public string AboutMyself { get; set; }
    }
}
