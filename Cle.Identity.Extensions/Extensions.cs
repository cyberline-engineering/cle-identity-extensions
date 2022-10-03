using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cle.Identity.Extensions.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Validation.SystemNetHttp;

namespace Cle.Identity.Extensions
{
    public static class Extensions
    {
        internal static readonly JsonSerializerOptions SerializerOptions =
            new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };

        public static OpenIddictBuilder AddCleIdentity(this IServiceCollection services, IConfiguration configuration,
            Action<AuthorizationOptions> authorizationConfigure = default)
        {
            services.AddHttpClient(typeof(OpenIddictValidationSystemNetHttpOptions).Assembly.GetName().Name)
                .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });

            var section = configuration.GetSection(nameof(IdentityOAuthConfig));
            services.AddOptions<IdentityOAuthConfig>()
                .Bind(section)
                .ValidateDataAnnotations();
            var oAuthConfig = section.Get<IdentityOAuthConfig>();

            section = configuration.GetSection(nameof(IdentityResourceConfig));
            services.AddOptions<IdentityResourceConfig>()
                .Bind(section)
                .ValidateDataAnnotations();
            var resourceConfig = section.Get<IdentityResourceConfig>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

            if (authorizationConfigure != default)
            {
                services.AddAuthorization(authorizationConfigure);
            }
            else
            {
                services.AddAuthorization();
            }

            // Register the OpenIddict validation components.
            return services.AddOpenIddict()
                .AddValidation(options =>
                {
                    // Note: the validation handler uses OpenID Connect discovery
                    // to retrieve the address of the introspection endpoint.
                    options.SetIssuer(oAuthConfig.Endpoint);
                    options.AddAudiences(resourceConfig.ClientId);

                    // Configure the validation handler to use introspection and register the client
                    // credentials used when communicating with the remote introspection endpoint.
                    options.UseIntrospection()
                        .SetClientId(resourceConfig.ClientId)
                        .SetClientSecret(resourceConfig.ClientSecret);

                    // Register the System.Net.Http integration.
                    options.UseSystemNetHttp();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });
        }

        public static IApplicationBuilder UseCleIdentity(this IApplicationBuilder app)
        {
            return app
                .UseAuthentication()
                .UseAuthorization();
        }

        public static IServiceCollection AddCleIdentityClient(this IServiceCollection services,
            IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(IdentityOAuthConfig));
            services.AddOptions<IdentityOAuthConfig>()
                .Bind(section)
                .ValidateDataAnnotations();

            return services;
        }

        public static IServiceCollection AddIntrospectionSecurityTokenAccessor(this IServiceCollection services,
            IConfiguration configuration)
        {
            var config = configuration.GetSection(nameof(IdentityResourceConfig)).Get<IdentityResourceConfig>();
            Validator.ValidateObject(config, new ValidationContext(config), true);

            services
                .AddTransient(provider =>
                {
                    var ta = new IntrospectionSecurityTokenAccessor(
                        provider.GetRequiredService<HttpClient>(),
                        provider.GetRequiredService<IOptions<IdentityOAuthConfig>>(),
                        config, provider.GetRequiredService<ILogger<IntrospectionSecurityTokenAccessor>>());

                    return ta;
                });

            return services;
        }

        public static IServiceCollection AddHttpContextSecurityTokenAccessor(this IServiceCollection services)
        {
            services.AddTransient<HttpContextSecurityTokenAccessor>();

            return services;
        }

        public static IHttpClientBuilder UseCleIdentityClient<T>(this IHttpClientBuilder builder) where T : ISecurityTokenAccessor
        {
            builder.Services.AddTransient<AuthenticatingHandler<T>>();
            builder.AddHttpMessageHandler<AuthenticatingHandler<T>>();
            
            return builder;
        }
    }
}