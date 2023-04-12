using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myProject.Core;

namespace myProject.Data.Entities;

public class Role : IBaseEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    //nav prop
    public List<User> Users { get; set; }
}
