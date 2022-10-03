using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Cle.Identity.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Cle.Identity.Tests
{
    public class AuthTests
    {
        private readonly IHost host;

        public AuthTests()
        {
            host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder => builder.AddUserSecrets(Assembly.GetExecutingAssembly(), true))
                .ConfigureServices((context, services) =>
                {
                    services.AddCleIdentityClient(context.Configuration);
                    services.AddIntrospectionSecurityTokenAccessor(context.Configuration.GetSection("test"));

                    services
                        .AddHttpClient("test")
                        .UseCleIdentityClient<IntrospectionSecurityTokenAccessor>();
                })
                .Build();
        }

        [Fact]
        public async Task AuthenticatingHandlerTest()
        {
            var httpClient = host.Services.GetRequiredService<IHttpClientFactory>().CreateClient("test");

            var response = await httpClient.GetStringAsync("http://localhost:6080/api/user/");

            Assert.NotNull(response);
        }
    }
}