namespace Cle.Identity.Extensions.Types;

/// <summary>
/// Access token
/// </summary>
public interface IAccessToken
{
    /// <summary>
    /// Token type
    /// </summary>
    string TokenType { get; set; }
    /// <summary>
    /// Access token
    /// </summary>
    string Token { get; set; }
    /// <summary>
    /// Expires date
    /// </summary>
    DateTimeOffset ExpiresAt { get; set; }
}