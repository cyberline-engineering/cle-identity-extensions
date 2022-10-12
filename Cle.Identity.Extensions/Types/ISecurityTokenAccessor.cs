namespace Cle.Identity.Extensions.Types;

/// <summary>
/// Validate and renew Authorize Access Token Interface
/// </summary>
public interface ISecurityTokenAccessor
{
    /// <summary>
    /// Validate access token
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public bool ValidateAccessToken(IAccessToken? accessToken);
    /// <summary>
    /// Get new access token
    /// </summary>
    /// <returns></returns>
    public ValueTask<IAccessToken> RenewAccessTokenAsync();
}