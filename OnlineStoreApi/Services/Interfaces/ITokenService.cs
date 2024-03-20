using System.IdentityModel.Tokens.Jwt;

namespace OnlineStoreApi.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenModel> CreateJwtTokenAsync(ApplicationUser user);
        Task<TokenModel> RefreshAccessTokenAsync(string accessToken, string refreshToken);
    }
}
