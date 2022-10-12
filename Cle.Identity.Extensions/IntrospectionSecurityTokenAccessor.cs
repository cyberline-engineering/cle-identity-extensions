using System;
using Cle.Identity.Extensions.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cle.Identity.Extensions
{
    /// <summary>
    /// Get access token using client credential grant flow
    /// </summary>
    public class IntrospectionSecurityTokenAccessor: ISecurityTokenAccessor
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<IdentityOAuthConfig> authOptions;
        private readonly ILogger<IntrospectionSecurityTokenAccessor> logger;
        private readonly IOptions<IdentityResourceConfig> resourceOptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="authOptions"></param>
        /// <param name="resourceOptions"></param>
        /// <param name="logger"></param>
        public IntrospectionSecurityTokenAccessor(HttpClient httpClient, IOptions<IdentityOAuthConfig> authOptions,
            IOptions<IdentityResourceConfig> resourceOptions, ILogger<IntrospectionSecurityTokenAccessor> logger)
        {
            this.httpClient = httpClient;
            this.authOptions = authOptions;
            this.resourceOptions = resourceOptions;
            this.logger = logger;
        }

        /// <inheritdoc />
        public bool ValidateAccessToken(IAccessToken? accessToken)
        {
            return accessToken != null && accessToken.ExpiresAt > DateTimeOffset.UtcNow - TimeSpan.FromSeconds(10);
        }

        /// <inheritdoc />
        public async ValueTask<IAccessToken> RenewAccessTokenAsync()
        {
            var config = resourceOptions.Value;
            var accessToken = await httpClient.Authorize(authOptions.Value, new ClientCredentials()
            {
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret
            }, logger, config.Scopes);

            return accessToken ?? AccessToken.Empty;
        }
    }
}
