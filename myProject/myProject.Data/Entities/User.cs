using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace myProject.Data.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    //non req если пользователь зашел неавторизованно
    public string? Email { get; set; }
    public string? Password { get; set; }
    [Required]
    public bool MailNotification { get; set; }
    public int? Raiting { get; set; }
    public string? Avatar { get; set; }
    public string? AboutMyself { get; set; }
    //nav prop
    public int RoleId { get; set; }
    public Role Role { get; set; }
    public List<Comment> Comments { get; set; }
    public List<Subscription> MyFollows { get; set; }
    public List<Subscription> MyFollowers { get; set; }
    public List<UserCategory> UserCategories { get; set; }
}
