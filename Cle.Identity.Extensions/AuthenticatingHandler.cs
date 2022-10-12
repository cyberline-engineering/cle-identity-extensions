using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Cle.Identity.Extensions.Types;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Cle.Identity.Extensions
{
    /// <summary>
    /// ASP.NET Core Middleware Handler that process Unauthorized (Http Code 401) response and auto OAuth Authenticate 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AuthenticatingHandler<T> : DelegatingHandler where T : ISecurityTokenAccessor
    {
        private readonly T securityTokenAccessor;
        private IAccessToken? accessToken;
        private AuthenticationHeaderValue? authenticationHeader;
        private readonly AsyncRetryPolicy<HttpResponseMessage> policy;
        private readonly ILogger<AuthenticatingHandler<T>> logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="securityTokenAccessor"></param>
        /// <param name="logger"></param>
        public AuthenticatingHandler(T securityTokenAccessor, ILogger<AuthenticatingHandler<T>> logger)
        {
            this.securityTokenAccessor = securityTokenAccessor;
            this.logger = logger;

            // Create a policy that tries to renew the access token if a 401 Unauthorized is received.
            policy = Policy.HandleResult<HttpResponseMessage>(r => r is { StatusCode: HttpStatusCode.Unauthorized })
                .RetryAsync(1, async (response, _) => { await Authenticate(); });
        }

        /// <summary>
        /// Override SendAsync and process Unauthorized response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Request an access token if we don't have one yet or if it has expired.
            if (!securityTokenAccessor.ValidateAccessToken(accessToken))
            {
                logger.LogDebug("ValidateAccessToken return false. Run Authenticate");
                await Authenticate();
            }

            // Try to perform the request, re-authenticating gracefully if the call fails due to an expired or revoked access token.
            var result = await policy.ExecuteAndCaptureAsync(() =>
            {
                request.Headers.Authorization = authenticationHeader;
                return base.SendAsync(request, cancellationToken);
            });

            if (result.Outcome == OutcomeType.Successful) return result.Result ?? result.FinalHandledResult;

            if (result.ExceptionType == ExceptionType.HandledByThisPolicy)
            {
                logger.LogDebug("Fail Authenticate policy");
            }

            logger.LogError(result.FinalException, "Fail request at AuthenticatingHandler");

            throw result.FinalException;
        }

        private async Task Authenticate()
        {
            accessToken = await securityTokenAccessor.RenewAccessTokenAsync();
            authenticationHeader = new AuthenticationHeaderValue(accessToken.TokenType, accessToken.Token);

            logger.LogTrace("Return access token {@accessToken}", accessToken);
        }
    }
}
