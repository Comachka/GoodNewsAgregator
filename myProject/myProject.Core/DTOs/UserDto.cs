namespace myProject.Core.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool MailNotification { get; set; }
        public int? Raiting { get; set; }
        public string? Avatar { get; set; }
        public string? AboutMyself { get; set; }
        public int RoleId { get; set; }
    }
}