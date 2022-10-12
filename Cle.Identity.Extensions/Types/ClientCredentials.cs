namespace Cle.Identity.Extensions.Types
{
    /// <summary>
    /// Client credentials
    /// </summary>
    public class ClientCredentials
    {
        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Client secret
        /// </summary>
        public string ClientSecret { get; set; } = default!;
    }
}