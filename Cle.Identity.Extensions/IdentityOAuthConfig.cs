using System.ComponentModel.DataAnnotations;

namespace Cle.Identity.Extensions
{
    public class IdentityOAuthConfig
    {
        [Required(ErrorMessage = "Not define IdentityOAuthConfig.Endpoint. Please provide correct url at appsettings.json")]
        public string Endpoint { get; set; }
    }
}
