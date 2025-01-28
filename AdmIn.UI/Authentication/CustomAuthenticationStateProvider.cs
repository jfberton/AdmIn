using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace AdmIn.UI.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;

        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionTask = _sessionStorage.GetAsync<UserSession>("UserSession").AsTask();
                var completedTask = await Task.WhenAny(userSessionTask, Task.Delay(TimeSpan.FromSeconds(5)));

                if (completedTask != userSessionTask)
                {
                    Console.Error.WriteLine("Timeout al obtener el estado de autenticación.");
                    return new AuthenticationState(_anonymous);
                }

                var userSessionStorageResult = await userSessionTask;

                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

                if (userSession == null)
                {
                    return new AuthenticationState(_anonymous);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.Email),
                    new Claim("Nombre", userSession.Nombre),
                    new Claim("Token", userSession.Token)
                };

                foreach (var role in userSession.Roles.Split(", "))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "apiauth"));

                return new AuthenticationState(claimsPrincipal);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en GetAuthenticationStateAsync: {ex.Message}");
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            try
            {
                ClaimsPrincipal claimsPrincipal;

                if (userSession != null)
                {
                    await _sessionStorage.SetAsync("UserSession", userSession);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userSession.Email),
                        new Claim("Token", userSession.Token)
                    };

                    foreach (var role in userSession.Roles.Split(", "))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "apiauth"));
                }
                else
                {
                    await _sessionStorage.DeleteAsync("UserSession");
                    claimsPrincipal = _anonymous;
                }

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error en UpdateAuthenticationState: {ex.Message}");
            }
        }

        public async Task Logout()
        {
            try
            {
                // Eliminar la sesión almacenada
                await _sessionStorage.DeleteAsync("UserSession");

                // Cambiar el estado de autenticación a anónimo
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }

    }
}
