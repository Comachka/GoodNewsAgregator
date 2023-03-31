using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myProject.Data.Entities;

public class NewsResource
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Link { get; set; }
    //nav prop
    public List<Article> Articles { get; set; }
}
