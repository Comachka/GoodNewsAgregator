using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using myProject.Core;

namespace myProject.Data.Entities;

public class RefreshToken : IBaseEntity
{
    [Key]
    public int Id { get; set; }
    public Guid Value { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}
