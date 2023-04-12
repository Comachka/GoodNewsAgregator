using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myProject.Core;

namespace myProject.Data.Entities;

public class Subscription : IBaseEntity
{
    [Key]
    public int Id { get; set; }
    //nav prop
    public int FollowOnId { get; set; }
    public User FollowOn { get; set; }
    public int FollowerId { get; set; }
    public User Follower { get; set; }
}
