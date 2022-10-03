namespace Cle.Identity.Extensions.Types;

public interface ISecurityTokenAccessor
{
    public bool ValidateAccessToken(IAccessToken accessToken);
    public Task<IAccessToken> RenewAccessTokenAsync();
}