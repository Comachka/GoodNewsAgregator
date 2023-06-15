namespace myProject.WebAPI.Responses
{
    public class ArticleResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public double? PositiveRaiting { get; set; }
        public DateTime DatePosting { get; set; }
        public int NewsResourceId { get; set; }
        public string ArticleSourceUrl { get; set; }
        public string SourceName { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
    }
}
