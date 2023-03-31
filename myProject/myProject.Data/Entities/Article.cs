using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myProject.Data.Entities;

public class Article
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Content { get; set; }
    [Required]
    public int PositiveRaiting { get; set; }
    [Required]
    public DateTime DatePosting { get; set; }
    //nav prop
    public int NewsResourceId { get; set; }
    public NewsResource NewsResource { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<Comment> Comments { get; set; }
}
