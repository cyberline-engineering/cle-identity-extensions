using System;
using Cle.Identity.Extensions.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cle.Identity.Extensions
{
    public class IntrospectionSecurityTokenAccessor: ISecurityTokenAccessor
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<IdentityOAuthConfig> authOptions;
        private readonly ILogger<IntrospectionSecurityTokenAccessor> logger;
        private readonly IOptions<IdentityResourceConfig> resourceOptions;

        public IntrospectionSecurityTokenAccessor(HttpClient httpClient, IOptions<IdentityOAuthConfig> authOptions,
            IOptions<IdentityResourceConfig> resourceOptions, ILogger<IntrospectionSecurityTokenAccessor> logger)
        {
            this.httpClient = httpClient;
            this.authOptions = authOptions;
            this.resourceOptions = resourceOptions;
            this.logger = logger;
        }

        public bool ValidateAccessToken(IAccessToken accessToken)
        {
            return accessToken != null && accessToken.ExpiresAt > DateTime.UtcNow;
        }

        public async Task<IAccessToken> RenewAccessTokenAsync()
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
