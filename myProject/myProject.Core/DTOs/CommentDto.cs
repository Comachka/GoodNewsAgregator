namespace myProject.Core.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int Raiting { get; set; }
        public string Content { get; set; }
        public string DateCreated { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public string Avatar { get; set; }
        public int ArticleId { get; set; }
    }
}