using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace Cle.Identity.Extensions
{
    /// <summary>
    /// Http Context Extensions
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Get identity id from HttpContext current User at 'sub' claim
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetIdentityId(this HttpContext context)
        {
            var principal = context.User;

            return GetIdentityId(principal);
        }

        /// <summary>
        /// Get identity id from 'sub' claim
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetIdentityId(this ClaimsPrincipal principal)
        {
            var claim = principal.GetClaim(OpenIddictConstants.Claims.Subject);
            if (claim == default) throw new Exception("Not found user identity id");

            return claim;
        }

        /// <summary>
        /// Get access token from Http Context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<JwtSecurityToken> GetJwtToken(this HttpContext context)
        {
            var accessToken = await context.GetTokenAsync(tokenName: "access_token").ConfigureAwait(false);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);

            return token;
        }
    }
}
