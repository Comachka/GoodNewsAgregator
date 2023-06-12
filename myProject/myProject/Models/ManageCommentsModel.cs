using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace myProject.Models
{
    public class ManageCommentsModel
    {
        public List<CommentModel> Comments { get; set; }
        public int ArticleId { get; set; }

    }
}
