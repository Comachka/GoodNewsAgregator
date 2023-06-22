namespace myProject.WebAPI.Responses
{
    public class TokenResponse
    {
        public string JwtToken { get; set; }
        public string Email { get; set; }

        public string RefreshToken { get; set; }
    }
}
