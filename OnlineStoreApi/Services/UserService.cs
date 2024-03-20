
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace OnlineStoreApi.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public UserService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }
        public async Task<AuthModel> RegisterUserAsync(RegisterModel registerModel)
        {
            // first check if user email is already exists in database
            if (await _userManager.FindByEmailAsync(registerModel.Email) is not null)
                return new AuthModel { Message = "Email already exists" };
            // check if username is already exists in database
            if (await _userManager.FindByNameAsync(registerModel.UserName) is not null)
                return new AuthModel { Message = "Username already exists" };

            // create new user object
            var user = new ApplicationUser();
            user.Email = registerModel.Email;
            user.UserName = registerModel.UserName;

            // try to create user using password in db
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            // if creating user failed return auth model with message of what went wrong
            if (!result.Succeeded)
            {
                string errorMessage = string.Empty;
                foreach (var error in result.Errors)
                {
                    errorMessage += $"{error.Description} | ";
                }
                return new AuthModel { Message = errorMessage };
            }
            // user creation went ok then create token and send it back
            var jwtToken = await _tokenService.CreateJwtTokenAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Username = user.UserName,
                Email = user.Email,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken.AccessToken),
                AccessExpiresOn = jwtToken.AccessToken.ValidTo,
                RefreshToken = new JwtSecurityTokenHandler().WriteToken(jwtToken.AccessToken),
                RefreshExpiresOn = jwtToken.RefreshToken.ValidTo,
            };
        }
        public async Task<AuthModel> LoginUserAsync(LoginModel loginModel)
        {
            AuthModel authModel = new AuthModel();
            // return if email doesn't exist OR email+password don't match
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtToken = await _tokenService.CreateJwtTokenAsync(user);
            var claim = await _userManager.GetClaimsAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Username = user.UserName;
            authModel.Email = user.Email;
            authModel.UserTypeClaim = claim.First().Type;
            authModel.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken.AccessToken);
            authModel.AccessExpiresOn = jwtToken.AccessToken.ValidTo;
            authModel.RefreshToken = new JwtSecurityTokenHandler().WriteToken(jwtToken.RefreshToken);
            authModel.RefreshExpiresOn = jwtToken.RefreshToken.ValidTo;

            return authModel;
        }
        public async Task<TokenModel> RefreshUserTokenAsync(string accessToken, string refreshToken)
        {
            return await _tokenService.RefreshAccessTokenAsync(accessToken, refreshToken);
        }
    }
}
