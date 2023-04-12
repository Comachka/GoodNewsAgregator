using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myProject.Core;

namespace myProject.Data.Entities;

public class UserCategory : IBaseEntity
{
    [Key]
    public int Id { get; set; }
    //nav prop
    public int UserId { get; set; }
    public User User { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}
