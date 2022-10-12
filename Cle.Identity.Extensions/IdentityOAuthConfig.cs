using System.ComponentModel.DataAnnotations;

namespace Cle.Identity.Extensions
{
    /// <summary>
    /// Identity Options
    /// </summary>
    public class IdentityOAuthConfig
    {
        /// <summary>
        /// Identity Service Uri
        /// </summary>
        [Required(ErrorMessage =
            "Not define IdentityOAuthConfig.Endpoint. Please provide correct url at appsettings.json")]
        public string Endpoint { get; set; } = default!;
    }
}
