using System.IdentityModel.Tokens.Jwt;

namespace OnlineStoreApi.Services.Interfaces
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> CreateJwtTokenAsync(ApplicationUser user);
        Task<AuthModel> RefreshAccessTokenAsync(string refreshToken);
    }
}
