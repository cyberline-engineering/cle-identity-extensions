using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Cle.Identity.Extensions.Types
{
    public class AccessToken: IAccessToken
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        public DateTime ExpiresAt { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        public static readonly AccessToken Empty = new()
            { TokenType = "Bearer", ExpiresAt = DateTime.MaxValue, ExpiresIn = int.MaxValue };
    }
}
