namespace Cle.Identity.Extensions.Types
{
    class AuthorizeRequest: HttpRequestMessage
    {
        public AuthorizeRequest(string host, ClientCredentials credentials, params string[] scopes): base(HttpMethod.Post, $"{host}/api/connect/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = credentials.ClientId,
                ["client_secret"] = credentials.ClientSecret,
                ["scope"] = scopes?.Length > 0 ? String.Join(' ', scopes) : String.Empty
            });
        }
    }
}
