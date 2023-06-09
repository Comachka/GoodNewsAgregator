﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myProject.Core;

namespace myProject.Data.Entities;

public class Article : IBaseEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string? Content { get; set; }
    public double? PositiveRaiting { get; set; }
    [Required]
    public DateTime DatePosting { get; set; }
    public string ArticleSourceUrl { get; set; }
    //nav prop
    public int NewsResourceId { get; set; }
    public NewsResource NewsResource { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<Comment> Comments { get; set; }
}
