namespace myProject.WebAPI.Responses
{
    public class ProfileResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string? Avatar { get; set; }

        public string? AboutMyself { get; set; }
        public int? Raiting { get; set; }
        public string? Role { get; set; }
        public int MyLikes { get; set; }
        public int OnMeLikes { get; set; }
    }
}
