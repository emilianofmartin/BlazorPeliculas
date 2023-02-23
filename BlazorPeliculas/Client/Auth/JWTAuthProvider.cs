using BlazorPeliculas.Client.Helpers;
using BlazorPeliculas.Client.Repositories;
using BlazorPeliculas.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace BlazorPeliculas.Client.Auth {
    public class JWTAuthProvider : AuthenticationStateProvider, ILoginService {
        private readonly IJSRuntime js;
        private readonly HttpClient httpClient;
        private readonly IRepository repository;

        public JWTAuthProvider(IJSRuntime js,
            HttpClient httpClient,
            IRepository repository) {
            this.js = js;
            this.httpClient = httpClient;
            this.repository = repository;
        }

        public static readonly string TOKENKEY = "TOKENKEY";
        public static readonly string EXPIRATIONTOKENKEY = "EXPIRATIONTOKENKEY";

        private AuthenticationState anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        public async override Task<AuthenticationState> GetAuthenticationStateAsync() {
            var token = await js.GetFromLocalStorage(TOKENKEY);

            if(token is null) {
                //Es un usuario anónimo.
                return anonymous;
            }

            DateTime expirationTime;
            var expirationTimeObject = await js.GetFromLocalStorage(EXPIRATIONTOKENKEY);

            if(expirationTimeObject is null) {
                //No tiene tiempo de expiración
                await Clean();
                return anonymous;
            }

            if(DateTime.TryParse(expirationTimeObject.ToString(), out expirationTime)) {
                if(expiredToken(expirationTime)) {
                    //El token expiró
                    await Clean();
                    return anonymous;
                }

                if(MustRenewToken(expirationTime)) {
                    token = await RenewToken(token.ToString()!);
                }
            }

            return ConstructAuthenticationState(token.ToString()!);
        }

        private bool expiredToken(DateTime expirationTime) {
            return expirationTime <= DateTime.UtcNow;
        }

        private bool MustRenewToken(DateTime expirationTime) {
            return expirationTime.Subtract(DateTime.UtcNow) < TimeSpan.FromMinutes(5);
        }

        public async Task ManageTokenRenewal() {
            DateTime expirationTime;
            var expirationTimeObject = await js.GetFromLocalStorage(EXPIRATIONTOKENKEY);

            if(DateTime.TryParse(expirationTimeObject.ToString(), out expirationTime)) {
                if(expiredToken(expirationTime)) {
                    //El token expiró
                    await Logout();
                }

                if(MustRenewToken(expirationTime)) {
                    var token = await js.GetFromLocalStorage(TOKENKEY);
                    var newToken = await RenewToken(token.ToString()!);
                    var authState = ConstructAuthenticationState(newToken);

                    //Debo notificar al cliente porque puedo tener nuevos roles en el token.
                    NotifyAuthenticationStateChanged(Task.FromResult(authState));
                }
            }
        }

        private async Task<string> RenewToken(string token) {
            Console.WriteLine("Renewing the token...");
            //Nos aseguramos de que el httpClient tenga el token.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            var newTokenResponse = await repository.Get<UserTokenDTO>("api/accounts/RenewToken");
            var newToken = newTokenResponse.Response!;

            await js.SetInLocalStorage(TOKENKEY, newToken.Token);
            await js.SetInLocalStorage(EXPIRATIONTOKENKEY, newToken.Expiration.ToString());
            return newToken.Token;
        }

        private AuthenticationState ConstructAuthenticationState(string token) {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var claims = ParseClaimsOutOfJWT(token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }

        private IEnumerable<Claim> ParseClaimsOutOfJWT(string token) {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var deserializedToken = jwtSecurityTokenHandler.ReadJwtToken(token);

            return deserializedToken.Claims;
        }

        public async Task Login(UserTokenDTO tokenDTO) {
            await js.SetInLocalStorage(TOKENKEY, tokenDTO.Token);
            await js.SetInLocalStorage(EXPIRATIONTOKENKEY, tokenDTO.Expiration.ToString());
            var authState = ConstructAuthenticationState(tokenDTO.Token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));     //Le notifico a Blazor que cambió el estado.

        }

        public async Task Logout() {
            await Clean();
            httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(anonymous));     //Le notifico a Blazor que cambió el estado.
        }

        private async Task Clean() {
            await js.RemoveFromLocalStorage(TOKENKEY);
            await js.RemoveFromLocalStorage(EXPIRATIONTOKENKEY);
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
