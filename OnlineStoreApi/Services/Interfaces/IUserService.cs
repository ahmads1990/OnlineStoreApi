namespace OnlineStoreApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthModel> RegisterUserAsync(RegisterModel registerModel);
        Task<AuthModel> LoginUserAsync(LoginModel loginModel);
        Task<TokenModel> RefreshUserTokenAsync(string accessToken, string refreshToken);
    }
}
