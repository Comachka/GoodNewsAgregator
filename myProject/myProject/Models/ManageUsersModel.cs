using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using myProject.Core.DTOs;

namespace myProject.Models
{
    public class ManageUsersModel
    {
        public List<ProfileModel> Users { get; set; }
    }
}
