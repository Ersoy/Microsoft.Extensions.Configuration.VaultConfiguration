using System.Security;
using System.Threading.Tasks;

namespace HashiCorp.Vault.Authentication {

    /// <summary>
    /// Represents a token authentication.
    /// </summary>
    public class TokenAuthentication : IVaultAuthentication {

        private readonly SecureString _vaultToken = new SecureString();

        /// <summary>
        /// Sets the vault authentication token.
        /// </summary>
        /// <param name="vaultToken">The vault token.</param>
        public TokenAuthentication(string vaultToken) {
            foreach (var letter in vaultToken) {
                _vaultToken.AppendChar(letter);
            }
        }

        public Task<SecureString> AuthenticateAsync() {
            return Task.FromResult(_vaultToken);
        }

    }

}
