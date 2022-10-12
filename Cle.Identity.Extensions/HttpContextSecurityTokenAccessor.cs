using System;
using Cle.Identity.Extensions.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cle.Identity.Extensions
{
    /// <summary>
    /// Get access token using user access token from HttpContext
    /// </summary>
    public class HttpContextSecurityTokenAccessor : ISecurityTokenAccessor
    {
        private readonly ILogger<HttpContextSecurityTokenAccessor> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logger"></param>
        public HttpContextSecurityTokenAccessor(IHttpContextAccessor httpContextAccessor,
            ILogger<HttpContextSecurityTokenAccessor> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        /// <inheritdoc />
        public bool ValidateAccessToken(IAccessToken? accessToken)
        {
            return accessToken != null && accessToken.ExpiresAt > DateTime.UtcNow;
        }

        /// <inheritdoc />
        public ValueTask<IAccessToken> RenewAccessTokenAsync()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var accessToken = new AccessToken()
            {
                Token = httpContext?.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer", String.Empty)
                    .Trim() ?? "",
                TokenType = "Bearer",
                ExpiresAt = DateTime.MaxValue,
                ExpiresIn = int.MaxValue,
            };

            return ValueTask.FromResult<IAccessToken>(accessToken);
        }
    }
}
