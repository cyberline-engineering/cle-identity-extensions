using System;
using Cle.Identity.Extensions.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cle.Identity.Extensions
{
    public class HttpContextSecurityTokenAccessor: ISecurityTokenAccessor
    {
        private readonly ILogger<HttpContextSecurityTokenAccessor> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpContextSecurityTokenAccessor(IHttpContextAccessor httpContextAccessor, ILogger<HttpContextSecurityTokenAccessor> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        public bool ValidateAccessToken(IAccessToken accessToken)
        {
            return accessToken != null && accessToken.ExpiresAt > DateTime.UtcNow;
        }

        public Task<IAccessToken> RenewAccessTokenAsync()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var accessToken = new AccessToken()
            {
                Token = httpContext.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer", String.Empty)
                    .Trim(),
                TokenType = "Bearer",
                ExpiresAt = DateTime.MaxValue,
                ExpiresIn = int.MaxValue,
            };

            return Task.FromResult<IAccessToken>(accessToken);
        }
    }
}
