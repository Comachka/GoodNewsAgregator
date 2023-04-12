namespace myProject.Core.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int Raiting { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public int ArticleId { get; set; }
    }
}