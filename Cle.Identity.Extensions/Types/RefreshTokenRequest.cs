using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cle.Identity.Extensions.Types
{
    class RefreshTokenRequest: HttpRequestMessage
    {
        public RefreshTokenRequest(string host, string refreshToken, params string[] scopes) : base(HttpMethod.Post,
            $"{host}/api/connect/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            });
        }
    }
}
