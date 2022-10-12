using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Cle.Identity.Extensions.Types
{
    /// <summary>
    /// Access token
    /// </summary>
    public class AccessToken: IAccessToken
    {
        /// <inheritdoc />
        [JsonPropertyName("access_token")]
        public string Token { get; set; } = default!;

        /// <inheritdoc />
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = default!;

        /// <summary>
        /// Expire in seconds
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <inheritdoc />
        public DateTimeOffset ExpiresAt { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Id token
        /// </summary>
        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }

        /// <summary>
        /// Empty value
        /// </summary>
        public static readonly AccessToken Empty = new()
            { TokenType = "Bearer", ExpiresAt = DateTime.MaxValue, ExpiresIn = int.MaxValue };
    }
}
