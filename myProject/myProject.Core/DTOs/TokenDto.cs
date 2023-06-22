namespace myProject.Core.DTOs
{
    public class TokenDto
    {
        public string JwtToken { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}
