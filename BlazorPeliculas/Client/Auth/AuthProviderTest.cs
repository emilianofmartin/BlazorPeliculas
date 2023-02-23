using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorPeliculas.Client.Auth {
    public class AuthProviderTest : AuthenticationStateProvider {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
            var anonymous = new ClaimsIdentity();
            var user = new ClaimsIdentity(new List<Claim> {
                new Claim("Key1", "Value1"),
                new Claim("Age", "45"),
                new Claim(ClaimTypes.Name, "Emiliano"),
                new Claim(ClaimTypes.Role, "admin")
            },
            authenticationType: "test");
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(user)));
        }
    }
}
