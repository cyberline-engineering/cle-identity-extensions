using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace Cle.Identity.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetIdentityId(this HttpContext context)
        {
            var principal = context.User;

            return GetIdentityId(principal);
        }

        public static string GetIdentityId(this ClaimsPrincipal principal)
        {
            var claim = principal.GetClaim(OpenIddictConstants.Claims.Subject);
            if (claim == default) throw new Exception("Not found user identity id");

            return claim;
        }

        public static async Task<JwtSecurityToken> GetJwtToken(this HttpContext context)
        {
            var accessToken = await context.GetTokenAsync(tokenName: "access_token").ConfigureAwait(false);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);

            return token;
        }
    }
}
