namespace OnlineStoreApi
{
    public class JwtConfig
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInHours { get; set; }
        public double RefreshTokenDurationInHours { get; set; }    
    }
}
