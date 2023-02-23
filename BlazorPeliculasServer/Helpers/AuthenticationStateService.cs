using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorPeliculasServer.Helpers {
    public class AuthenticationStateService {
        private readonly AuthenticationStateProvider authStateProvider;

        public AuthenticationStateService(AuthenticationStateProvider authStateProvider) {
            this.authStateProvider = authStateProvider;
        }

        public async Task<string?> GetCurrentUserID() {
            var userState = await authStateProvider.GetAuthenticationStateAsync();
            if(!userState.User.Identity!.IsAuthenticated)
                return null;

            var claims = userState.User.Claims;
            var claimWithUserId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if(claimWithUserId == null)
                throw new ApplicationException("Could not find User's ID");

            return claimWithUserId.Value;
        }
    }
}
