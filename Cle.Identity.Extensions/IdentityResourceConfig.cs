using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Cle.Identity.Extensions
{
    /// <summary>
    /// Options for ResourceServer Identity Introspection Client 
    /// </summary>
    public class IdentityResourceConfig: IOptions<IdentityResourceConfig>
    {
        /// <summary>
        /// OAuth Client Id
        /// </summary>
        [Required(ErrorMessage =
            "Not define IdentityResourceConfig.ClientId. Please provide correct client id at appsettings.json")]
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// OAuth Client Secret
        /// </summary>
        [Required(ErrorMessage = "Not define IdentityResourceConfig.ClientSecret. Please provide correct client secret at appsettings.json")]
        public string ClientSecret { get; set; } = default!;

        /// <summary>
        /// List of required scopes
        /// </summary>
        public string[]? Scopes { get; set; }

        /// <summary>
        /// IOptions Value
        /// </summary>
        public IdentityResourceConfig Value => this;
    }
}
