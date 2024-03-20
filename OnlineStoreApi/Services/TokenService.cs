using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineStoreApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;
        public TokenService(UserManager<ApplicationUser> userManager, IOptions<JwtConfig> jwtConfig, IOptions<TokenValidationParameters> tokenValidationParameters)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
            _tokenValidationParameters = tokenValidationParameters.Value;
        }
        public async Task<TokenModel> CreateJwtTokenAsync(ApplicationUser user)
        {
            if (user is null) return null;
            // get user claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            // create jwt claims
            var jwtClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            };
            // merge both claims lists and jwtClaims to allClaims
            var allClaims = jwtClaims.Union(userClaims);

            // specify the signing key and algorithm
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            // finally create the access token
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: allClaims,
                expires: DateTime.Now.AddHours(_jwtConfig.DurationInHours),
                signingCredentials: signingCredentials
                );

            //Create the refresh token
            var refreshToken = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: new[] { new Claim("uid", user.Id) },
                expires: DateTime.Now.AddHours(_jwtConfig.RefreshTokenDurationInHours),
                signingCredentials: signingCredentials
                );

            return new TokenModel
            {
                AccessToken = jwtSecurityToken,
                RefreshToken = refreshToken,
            };
        }

        public async Task<TokenModel> RefreshAccessTokenAsync(string accessToken, string refreshToken)
        {
            // Extract user information from the token
            var refreshTokenPrincipal = GetPrincipalFromExpiredToken(refreshToken);
            var accessTokenPrincipal = GetPrincipalFromExpiredToken(accessToken);

            // Make sure the user id in the refreshToken and accessToken are the same
            var refreshUserId = refreshTokenPrincipal.Claims.First(c => c.Type == "uid").Value;
            var userId = refreshTokenPrincipal.Claims.First(c => c.Type == "uid").Value;

            if (refreshUserId != userId)
                throw new SecurityTokenException("Invalid refresh token");

            // Find  user id from token and search for user
            var user = await _userManager.FindByIdAsync(userId);
            var jwtTokenModel = await CreateJwtTokenAsync(user);

            return jwtTokenModel;
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out _);
            if (!(principal is ClaimsPrincipal validatedUser))
                throw new SecurityTokenException("Invalid token");
            return validatedUser;
        }
    }
}
