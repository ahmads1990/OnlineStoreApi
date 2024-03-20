using System.IdentityModel.Tokens.Jwt;

namespace OnlineStoreApi.Models
{
    public class TokenModel
    {
        public JwtSecurityToken AccessToken { get; set; }
        public JwtSecurityToken RefreshToken { get; set; }
    }
}
