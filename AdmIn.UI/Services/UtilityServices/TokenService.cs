using AdmIn.UI.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;

namespace AdmIn.UI.Services.UtilityServices
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task<bool> IsTokenExpiredAsync();
        Task LogoutIfTokenExpiredAsync();
    }

    public class TokenService : ITokenService
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly CustomAuthenticationStateProvider _authStateProvider;

        public TokenService(ProtectedSessionStorage sessionStorage, AuthenticationStateProvider authStateProvider)
        {
            _sessionStorage = sessionStorage;
            _authStateProvider = authStateProvider as CustomAuthenticationStateProvider;
        }

        public async Task<string> GetTokenAsync()
        {
            var result = await _sessionStorage.GetAsync<UserSession>("UserSession");
            return result.Success ? result.Value?.Token : null;
        }

        public async Task SetTokenAsync(string token)
        {
            var result = await _sessionStorage.GetAsync<UserSession>("UserSession");
            if (result.Success && result.Value != null)
            {
                var session = result.Value;
                session.Token = token;
                await _sessionStorage.SetAsync("UserSession", session);
            }
        }

        public async Task<bool> IsTokenExpiredAsync()
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return true;

            var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(token))
                return true;

            var jwtToken = jwtHandler.ReadJwtToken(token);
            return DateTime.UtcNow >= jwtToken.ValidTo;
        }

        public async Task LogoutIfTokenExpiredAsync()
        {
            if (await IsTokenExpiredAsync() && _authStateProvider != null)
            {
                await _authStateProvider.Logout();
            }
        }
    }
}

