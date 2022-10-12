using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cle.Identity.Extensions.Types;
using Microsoft.Extensions.Logging;

namespace Cle.Identity.Extensions
{
    /// <summary>
    /// Identity Service HttpClient Extensions
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Authorize 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="authConfig"></param>
        /// <param name="credentials"></param>
        /// <param name="logger"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public static async Task<AccessToken?> Authorize(this HttpClient httpClient, IdentityOAuthConfig authConfig, ClientCredentials credentials, ILogger logger, params string[]? scopes)
        {
            logger.LogInformation("Authorize cle identity. Endpoint: {endpoint}", authConfig.Endpoint);

            using var request = new AuthorizeRequest(authConfig.Endpoint, credentials, scopes);
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                return default;
            }

            var accessToken = await response.Content
                .ReadFromJsonAsync<AccessToken>(Extensions.SerializerOptions).ConfigureAwait(false);

            if (accessToken == default)
            {
                logger.LogError("Return empty response. Content: {content}",
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                return default;
            }

            accessToken.ExpiresAt = DateTime.UtcNow.AddSeconds(accessToken.ExpiresIn);

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(accessToken.TokenType, accessToken.Token);

            return accessToken;
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="authConfig"></param>
        /// <param name="accessToken"></param>
        /// <param name="logger"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public static async Task<AccessToken?> RefreshToken(this HttpClient httpClient, IdentityOAuthConfig authConfig,
            AccessToken accessToken, ILogger logger, params string[] scopes)
        {
            logger.LogInformation("Refresh Token cle identity. Endpoint: {endpoint}", authConfig.Endpoint);

            if (accessToken.RefreshToken == default)
            {
                logger.LogError("Need refresh token");
                return default;
            }

            using var request = new RefreshTokenRequest(authConfig.Endpoint, accessToken.RefreshToken, scopes);
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                return default;
            }

            var token = await response.Content
                .ReadFromJsonAsync<AccessToken>(Extensions.SerializerOptions).ConfigureAwait(false);

            if (token == default)
            {
                logger.LogError("Return empty response. Content: {content}",
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                return default;
            }

            accessToken.ExpiresAt = DateTime.UtcNow.AddSeconds(accessToken.ExpiresIn);

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(token.TokenType, token.Token);

            return token;
        }
    }
}
