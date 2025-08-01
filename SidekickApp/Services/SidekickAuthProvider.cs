using MAS_Shared.MASConstants;
using MAS_Shared.Security;
using Microsoft.AspNetCore.Components.Authorization;
using SidekickApp.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SidekickApp.Services
{
    public class SidekickAuthProvider : AuthenticationStateProvider
    {
        public async Task<JwtSecurityToken?> TryGetTokenAsync()
        {
            var rawToken = await JWTService.RetrieveTokenAsync();
            if (string.IsNullOrEmpty(rawToken)) return null;

            if (!JwtHelperUtil.ClientSideJWTPrevalidation(
                rawToken, MASConstants.ExpectedAudiences, MASConstants.ExpectedIssuer))
                return null;

            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(rawToken);
        }

        public async Task<(string? Token, bool IsLongLived)> GetValidatedTokenWithSourceAsync()
        {
            var longToken = await SecureStorage.GetAsync("long_lived_jwt");
            if (!string.IsNullOrEmpty(longToken)) return (longToken, true);

            var shortToken = await SecureStorage.GetAsync("short_lived_jwt");
            if (!string.IsNullOrEmpty(shortToken)) return (shortToken, false);

            return (null, false);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await TryGetTokenAsync();
            if (token is null)
            {
                var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
                return new AuthenticationState(anonymous);
            }

            var claims = token.Claims;
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            return new AuthenticationState(principal);
        }

        public void NotifyAuthenticationStateChanged()
        {
            var authState = GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(authState);
        }
    }
}