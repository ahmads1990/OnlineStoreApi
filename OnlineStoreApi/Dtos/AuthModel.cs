namespace OnlineStoreApi.Dtos
{
    public class AuthModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserTypeClaim { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessExpiresOn { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshExpiresOn { get; set; }
    }
}
