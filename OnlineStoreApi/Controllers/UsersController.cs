using Microsoft.AspNetCore.Mvc;

namespace OnlineStoreApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterModel registerModel)
        {
            var result = await _userService.RegisterUserAsync(registerModel);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginModel loginModel)
        {
            var result = await _userService.LoginUserAsync(loginModel);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            //Todo send confirmation mail
            return Ok(result);
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshUserTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var result = await _userService.RefreshUserTokenAsync(refreshTokenDto.AccessToken, refreshTokenDto.RefreshToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
                throw;
            }
        }
    }
}
