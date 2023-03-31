using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myProject.Data.Entities;

public class Comment
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int Raiting { get; set; }
    [Required]
    public string Content { get; set; }
    [Required]
    public DateTime DateCreated { get; set; }
    //nav prop
    public int UserId { get; set; }
    public User User { get; set; }
    public int ArticleId { get; set; }
    public Article Article { get; set; }
}
