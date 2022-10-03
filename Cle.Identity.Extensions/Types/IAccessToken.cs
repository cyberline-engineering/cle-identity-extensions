namespace Cle.Identity.Extensions.Types;

public interface IAccessToken
{
    string TokenType { get; set; }
    string Token { get; set; }
    DateTime ExpiresAt { get; set; }
}