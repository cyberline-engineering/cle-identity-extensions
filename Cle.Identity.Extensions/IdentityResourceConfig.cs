using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Cle.Identity.Extensions
{
    public class IdentityResourceConfig: IOptions<IdentityResourceConfig>
    {
        [Required(ErrorMessage = "Not define IdentityResourceConfig.ClientId. Please provide correct client id at appsettings.json")]
        public string ClientId { get; set; }

        [Required(ErrorMessage = "Not define IdentityResourceConfig.ClientSecret. Please provide correct client secret at appsettings.json")]
        public string ClientSecret { get; set; }

        public string[] Scopes { get; set; }

        public IdentityResourceConfig Value => this;
    }
}
