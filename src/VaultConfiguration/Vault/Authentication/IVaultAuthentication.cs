using System.Security;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration.VaultConfiguration.Vault.Authentication {

    /// <summary>
    /// Represents a vault authenticationn.
    /// </summary>
    public interface IVaultAuthentication {

        /// <summary>
        /// Authenticates to the vault system.
        /// </summary>
        /// <returns>Returns a vault authentication token.</returns>
        Task<SecureString> AuthenticateAsync();

    }

}
